namespace GalponERP.Application.Interfaces;

public interface ICurrentUserContext
{
    Guid? UsuarioId { get; }
    string? FirebaseUid { get; }
    string? NombreUsuario { get; }
}
