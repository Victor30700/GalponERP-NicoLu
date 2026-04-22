namespace GalponERP.Application.Interfaces;

/// <summary>
/// Define el servicio de WhatsApp que actúa como puente para la orquestación en n8n.
/// </summary>
public interface IWhatsAppService
{
    /// <summary>
    /// Envía un mensaje de texto a través del gateway configurado en n8n.
    /// </summary>
    Task<bool> EnviarMensajeTextoAsync(string telefono, string mensaje);

    /// <summary>
    /// Envía una notificación basada en plantilla usando el orquestador externo.
    /// </summary>
    Task<bool> EnviarNotificacionPlantillaAsync(string telefono, string nombrePlantilla, string lenguaje = "es_ES");

    /// <summary>
    /// Descarga un archivo multimedia desde los servidores de Meta vía n8n.
    /// </summary>
    Task<byte[]> DescargarMediaAsync(string mediaId);
}
