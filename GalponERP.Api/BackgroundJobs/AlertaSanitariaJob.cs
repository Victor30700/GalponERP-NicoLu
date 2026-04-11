using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GalponERP.Api.BackgroundJobs;

public class AlertaSanitariaJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AlertaSanitariaJob> _logger;

    public AlertaSanitariaJob(IServiceScopeFactory scopeFactory, ILogger<AlertaSanitariaJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AlertaSanitariaJob iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var loteRepository = scope.ServiceProvider.GetRequiredService<ILoteRepository>();
                    var calendarioRepository = scope.ServiceProvider.GetRequiredService<ICalendarioSanitarioRepository>();

                    var lotesActivos = await loteRepository.ObtenerTodosAsync();
                    
                    foreach (var lote in lotesActivos.Where(l => l.Estado == EstadoLote.Activo))
                    {
                        var diaActual = (DateTime.UtcNow - lote.FechaIngreso).Days + 1;
                        var actividades = await calendarioRepository.ObtenerPorLoteIdAsync(lote.Id);
                        
                        var pendientes = actividades.Where(a => a.Estado == EstadoCalendario.Pendiente && a.DiaDeAplicacion <= diaActual);

                        foreach (var pendiente in pendientes)
                        {
                            _logger.LogWarning("¡ALERTA SANITARIA! Lote: {LoteId} - Día: {DiaActual}. Vacuna/Tratamiento pendiente: {Descripcion} (Programado para día {DiaProgramado})", 
                                lote.Id, diaActual, pendiente.DescripcionTratamiento, pendiente.DiaDeAplicacion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando AlertaSanitariaJob.");
            }

            // Esperar 24 horas (en desarrollo tal vez menos, pero el requerimiento dice diariamente)
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
