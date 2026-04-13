using GalponERP.Domain.Entities;
using MediatR;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Mortalidad.Queries.ObtenerTendenciasMortalidad;
using GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorPagar;
using GalponERP.Application.Inventario.Queries.ListarOrdenesCompraPendientes;
using GalponERP.Application.Sanidad.Queries.ObtenerUltimoRegistroBienestar;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Agentes;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GalponERP.Api.BackgroundJobs;

public class AnalisisDatosJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AnalisisDatosJob> _logger;

    public AnalisisDatosJob(IServiceScopeFactory scopeFactory, ILogger<AnalisisDatosJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AnalisisDatosJob (Proactivo) iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var agenteOrquestador = scope.ServiceProvider.GetRequiredService<IAgenteOrquestadorService>();
                    var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();
                    var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();

                    var anomaliasEncontradas = new List<string>();

                    // 1. Analizar Inventario (Stock Alimento)
                    var inventario = await mediator.Send(new VerificarNivelesAlimentoQuery(), stoppingToken);
                    if (inventario.RequiereAlerta)
                    {
                        anomaliasEncontradas.Add($"Stock de alimento crítico: quedan {inventario.DiasRestantes:N1} días ({inventario.StockActualAlimento} Kg).");
                    }

                    // 2. Analizar Mortalidad
                    var lotesActivos = (await mediator.Send(new ListarLotesQuery(SoloActivos: true), stoppingToken)).ToList();
                    foreach (var lote in lotesActivos)
                    {
                        var tendencias = await mediator.Send(new ObtenerTendenciasMortalidadQuery(lote.Id), stoppingToken);
                        if (tendencias != null && tendencias.Tendencias.Any())
                        {
                            var ultimaSemana = tendencias.Tendencias.Last();
                            if (ultimaSemana.PorcentajeSemanal > 2) // Umbral de ejemplo: 2% semanal
                            {
                                anomaliasEncontradas.Add($"Mortalidad alta en {lote.NombreGalpon}: {ultimaSemana.PorcentajeSemanal}% en la semana {ultimaSemana.SemanaVida}.");
                            }
                        }

                        // 3. Alertas de Bienestar (NUEVO - Sprint 80 Paso 3)
                        var bienestar = await mediator.Send(new ObtenerUltimoRegistroBienestarQuery(lote.Id), stoppingToken);
                        if (bienestar != null && bienestar.Fecha >= DateTime.Today.AddDays(-1))
                        {
                            // Lógica simplificada de rangos por edad
                            if (lote.EdadSemanas <= 1) // Pollitos BB
                            {
                                if (bienestar.Temperatura < 30) anomaliasEncontradas.Add($"Frío detectado en {lote.NombreGalpon} (Semana 1): {bienestar.Temperatura}°C.");
                            }
                            else if (lote.EdadSemanas > 4) // Cerca de saca
                            {
                                if (bienestar.Temperatura > 26) anomaliasEncontradas.Add($"Calor excesivo en {lote.NombreGalpon} (Semana {lote.EdadSemanas}): {bienestar.Temperatura}°C.");
                            }

                            if (bienestar.ConsumoAgua < 10) // Umbral mínimo genérico para ejemplo
                                anomaliasEncontradas.Add($"Bajo consumo de agua en {lote.NombreGalpon}: {bienestar.ConsumoAgua} L.");
                        }
                    }

                    // 4. Analizar Cuentas por Pagar (NUEVO - Sprint 80 Paso 1)
                    var deudas = await mediator.Send(new ObtenerCuentasPorPagarQuery(), stoppingToken);
                    var deudasCriticas = deudas.Where(d => d.FechaVencimiento <= DateTime.Today.AddDays(3)).ToList();
                    foreach (var d in deudasCriticas)
                    {
                        string prefijo = d.FechaVencimiento < DateTime.Today ? "VENCIDA" : "POR VENCER";
                        anomaliasEncontradas.Add($"Deuda {prefijo}: {d.RazonSocialProveedor} (Vence: {d.FechaVencimiento:dd/MM}, Saldo: S/ {d.SaldoPendiente}).");
                    }

                    // 5. Analizar Órdenes de Compra Retrasadas (NUEVO - Sprint 80 Paso 2)
                    var ocsPendientes = await mediator.Send(new ListarOrdenesCompraPendientesQuery(), stoppingToken);
                    var ocsRetrasadas = ocsPendientes.Where(o => (DateTime.Today - o.Fecha).TotalDays > 7).ToList();
                    foreach (var oc in ocsRetrasadas)
                    {
                        anomaliasEncontradas.Add($"OC Retrasada: {oc.RazonSocialProveedor} (Pedida hace {(DateTime.Today - oc.Fecha).TotalDays:N0} días).");
                    }

                    // 6. Generar y enviar mensajes proactivos
                    if (anomaliasEncontradas.Any())
                    {
                        var contexto = string.Join(" | ", anomaliasEncontradas);
                        var mensajeProactivo = await agenteOrquestador.GenerarMensajeProactivoAsync(contexto);

                        // Enviar a todos los administradores con WhatsApp vinculado
                        var admins = await usuarioRepository.ObtenerPorRolAsync(RolGalpon.Admin);
                        foreach (var admin in admins)
                        {
                            if (!string.IsNullOrEmpty(admin.WhatsAppNumero))
                            {
                                await whatsAppService.EnviarMensajeTextoAsync(admin.WhatsAppNumero, mensajeProactivo);
                                _logger.LogInformation("Alerta proactiva enviada a {Admin} via WhatsApp.", admin.Nombre);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando AnalisisDatosJob.");
            }

            // Esperar 12 horas entre análisis
            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }
}
