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
                    var loteRepository = scope.ServiceProvider.GetRequiredService<ILoteRepository>();
                    var allLotes = await loteRepository.ObtenerTodosAsync();
                    var lotesActivos = allLotes.Where(l => l.Estado == EstadoLote.Activo).ToList();

                    foreach (var lote in lotesActivos)
                    {
                        var tendencias = await mediator.Send(new ObtenerTendenciasMortalidadQuery(lote.Id), stoppingToken);
                        if (tendencias != null && tendencias.Any())
                        {
                            var ultimaSemana = tendencias.Last();
                            if (ultimaSemana.Porcentaje > 2) // Umbral de ejemplo: 2% semanal
                            {
                                anomaliasEncontradas.Add($"Mortalidad alta en {lote.Nombre}: {ultimaSemana.Porcentaje}% en la semana {ultimaSemana.Semana}.");
                            }
                        }

                        // 3. Alertas de Bienestar (NUEVO - Sprint 80 Paso 3)
                        var bienestar = await mediator.Send(new ObtenerUltimoRegistroBienestarQuery(lote.Id), stoppingToken);
                        if (bienestar != null && bienestar.Fecha >= DateTime.Today.AddDays(-1))
                        {
                            // Lógica simplificada de rangos por edad
                            if (lote.EdadSemanas <= 1) // Pollitos BB
                            {
                                if (bienestar.Temperatura < 30) anomaliasEncontradas.Add($"Frío detectado en {lote.Nombre} (Semana 1): {bienestar.Temperatura}°C.");
                            }
                            else if (lote.EdadSemanas > 4) // Cerca de saca
                            {
                                if (bienestar.Temperatura > 26) anomaliasEncontradas.Add($"Calor excesivo en {lote.Nombre} (Semana {lote.EdadSemanas}): {bienestar.Temperatura}°C.");
                            }

                            if (bienestar.ConsumoAgua < 10) // Umbral mínimo genérico para ejemplo
                                anomaliasEncontradas.Add($"Bajo consumo de agua en {lote.Nombre}: {bienestar.ConsumoAgua} L.");
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

                    // 6. Auditoría de Eficiencia Alimenticia (FCR) - NUEVO Fase 4
                    var inventarioRepo = scope.ServiceProvider.GetRequiredService<IInventarioRepository>();
                    var pesajeRepo = scope.ServiceProvider.GetRequiredService<IPesajeLoteRepository>();
                    var productoRepo = scope.ServiceProvider.GetRequiredService<IProductoRepository>();
                    var categoriaRepo = scope.ServiceProvider.GetRequiredService<ICategoriaProductoRepository>();

                    // Obtener IDs de categorías tipo Alimento
                    var todasCategorias = await categoriaRepo.ObtenerTodasAsync();
                    var categoriasAlimentoIds = todasCategorias
                        .Where(c => c.Tipo == TipoCategoria.Alimento)
                        .Select(c => c.Id)
                        .ToList();

                    // Obtener productos que pertenecen a esas categorías
                    var todosProductos = await productoRepo.ObtenerTodosAsync();
                    var productosAlimentoIds = todosProductos
                        .Where(p => categoriasAlimentoIds.Contains(p.CategoriaProductoId))
                        .Select(p => p.Id)
                        .ToHashSet();

                    foreach (var lote in lotesActivos)
                    {
                        var movimientos = await inventarioRepo.ObtenerPorLoteIdAsync(lote.Id);
                        // Sumar solo salidas de productos identificados como Alimento
                        var totalAlimentoKg = movimientos
                            .Where(m => m.Tipo == TipoMovimiento.Salida && productosAlimentoIds.Contains(m.ProductoId))
                            .Sum(m => m.Cantidad * m.PesoUnitarioHistorico);

                        var pesajes = await pesajeRepo.ObtenerPorLoteIdAsync(lote.Id);
                        var ultimoPesaje = pesajes.OrderByDescending(p => p.Fecha).FirstOrDefault();

                        if (totalAlimentoKg > 0 && ultimoPesaje != null)
                        {
                            // Convertir gramos a Kg para el cálculo de FCR
                            decimal pesoKg = ultimoPesaje.PesoPromedioGramos / 1000m;
                            var fcr = lote.CalcularFCRActual(totalAlimentoKg, pesoKg);
                            var (esAlerta, mensajeFcr) = lote.ValidarEficienciaAlimenticia(fcr);
                            if (esAlerta)
                            {
                                anomaliasEncontradas.Add($"Eficiencia en {lote.Nombre}: {mensajeFcr}");
                            }
                        }
                    }

                    // 7. Generar y enviar mensajes proactivos
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
