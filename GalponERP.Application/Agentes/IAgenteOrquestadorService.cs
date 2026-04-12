namespace GalponERP.Application.Agentes;

public interface IAgenteOrquestadorService
{
    Task<string> ProcesarMensajeAsync(string mensajeUsuario);
}
