using GalponERP.Application.Interfaces;

namespace GalponERP.Infrastructure.Authentication;

public class CurrentUserContext : ICurrentUserContext
{
    public Guid? UsuarioId { get; private set; }
    public string? FirebaseUid { get; private set; }
    public string? NombreUsuario { get; private set; }

    public void SetUser(Guid usuarioId, string firebaseUid, string? nombreUsuario = null)
    {
        UsuarioId = usuarioId;
        FirebaseUid = firebaseUid;
        NombreUsuario = nombreUsuario;
    }
}
