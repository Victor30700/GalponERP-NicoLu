namespace GalponERP.Application.Interfaces;

public interface IWhatsAppService
{
    Task<bool> EnviarMensajeTextoAsync(string telefono, string mensaje);
    Task<bool> EnviarNotificacionPlantillaAsync(string telefono, string nombrePlantilla, string lenguaje = "es_ES");
    Task<byte[]> DescargarMediaAsync(string mediaId);
}
