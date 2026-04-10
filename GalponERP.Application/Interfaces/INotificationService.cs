namespace GalponERP.Application.Interfaces;

public interface INotificationService
{
    Task EnviarAlertaPushAsync(string usuarioId, string titulo, string mensaje);
}
