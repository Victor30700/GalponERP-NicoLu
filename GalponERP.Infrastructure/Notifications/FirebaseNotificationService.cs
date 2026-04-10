using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using GalponERP.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GalponERP.Infrastructure.Notifications;

public class FirebaseNotificationService : INotificationService
{
    private readonly ILogger<FirebaseNotificationService> _logger;

    public FirebaseNotificationService(ILogger<FirebaseNotificationService> logger)
    {
        _logger = logger;
        // ⚠️ ELIMINAMOS la inicialización de FirebaseApp.Create() de aquí.
        // Confiamos ciegamente en que el Program.cs ya levantó la conexión global al iniciar la API.
    }

    public async Task EnviarAlertaPushAsync(string usuarioId, string titulo, string mensaje)
    {
        _logger.LogInformation("[Firebase] Preparando notificación para el Topic user_{usuarioId}: {titulo}", usuarioId, titulo);

        // Fail-Fast: Verificamos si el Program.cs hizo su trabajo
        if (FirebaseApp.DefaultInstance == null)
        {
            _logger.LogError("[Firebase] ERROR CRÍTICO: FirebaseApp no está inicializado. Verifica el Program.cs y el archivo firebase-admin.json.");
            return;
        }

        var message = new Message()
        {
            Notification = new Notification()
            {
                Title = titulo,
                Body = mensaje
            },
            // El uso de Topics facilita el envío a múltiples dispositivos del mismo usuario
            Topic = $"user_{usuarioId}"
        };

        try
        {
            // FirebaseMessaging.DefaultInstance usa automáticamente la conexión global
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("[Firebase] Mensaje enviado con éxito. ID: {response}", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Firebase] Error al enviar mensaje al Topic user_{usuarioId}", usuarioId);
        }
    }
}