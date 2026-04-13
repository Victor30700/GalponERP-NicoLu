namespace GalponERP.Application.Agentes;

public interface IAgenteOrquestadorService
{
    Task<AgenteResponse> ProcesarMensajeAsync(string mensajeUsuario, Guid? conversacionId = null);
    Task<string> GenerarMensajeProactivoAsync(string contextoAnomalia);
    Task<string> GenerarResumenAsync(Guid conversacionId);
}

public record AgenteResponse(string Respuesta, Guid ConversacionId);
