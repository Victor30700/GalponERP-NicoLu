using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;

public record ObtenerUsuariosQuery() : IRequest<IEnumerable<UsuarioResponse>>;

public record UsuarioResponse(
    Guid Id,
    string FirebaseUid,
    string Email,
    string Nombre,
    RolGalpon Rol,
    bool IsActive);
