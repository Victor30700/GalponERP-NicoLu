using MediatR;

namespace GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;

public record ObtenerUsuariosQuery() : IRequest<IEnumerable<UsuarioResponse>>;

public record UsuarioResponse(
    Guid Id,
    string FirebaseUid,
    string Email,
    string Nombre,
    string Rol,
    bool IsActive);
