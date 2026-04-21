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

    public async Task EjecutarAlertaSanitariaAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ejecutando AlertaSanitariaJob vía Hangfire.");

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
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AlertaSanitariaJob (Legacy BackgroundService) en pausa. Hangfire gestionará esta tarea.");
        await Task.CompletedTask;
    }
}
