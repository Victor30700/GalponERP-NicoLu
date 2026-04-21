using GalponERP.Domain.Entities;
using MediatR;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GalponERP.Api.BackgroundJobs;

public class AlertaInventarioJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AlertaInventarioJob> _logger;

    public AlertaInventarioJob(IServiceScopeFactory scopeFactory, ILogger<AlertaInventarioJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task EjecutarAlertaAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ejecutando AlertaInventarioJob vía Hangfire.");

        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();

                var resultado = await mediator.Send(new VerificarNivelesAlimentoQuery(), stoppingToken);

                if (resultado.RequiereAlerta)
                {
                    _logger.LogWarning("¡Alerta de inventario! Quedan {DiasRestantes:N1} días de alimento.", resultado.DiasRestantes);

                    // Buscar usuarios con rol Admin
                    var admins = await usuarioRepository.ObtenerPorRolAsync(RolGalpon.Admin);
                    
                    foreach (var admin in admins)
                    {
                        await notificationService.EnviarAlertaPushAsync(
                            admin.FirebaseUid,
                            "Crítico: Inventario de Alimento Bajo",
                            $"Quedan aproximadamente {resultado.DiasRestantes:N1} días de alimento. Stock actual: {resultado.StockActualAlimento} Kg."
                        );
                    }
                }
                else
                {
                    _logger.LogInformation("Inventario de alimento en niveles normales. Dias restantes: {DiasRestantes:N1}", resultado.DiasRestantes);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando AlertaInventarioJob.");
            throw; // Re-lanzar para que Hangfire pueda reintentar
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AlertaInventarioJob (Legacy BackgroundService) en pausa. Hangfire gestionará esta tarea.");
        await Task.CompletedTask;
    }
}
