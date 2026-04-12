using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;
using MediatR;

namespace GalponERP.Application.Usuarios.Queries.ObtenerUsuarioActual;

public record ObtenerUsuarioActualQuery(Guid UsuarioId) : IRequest<UsuarioResponse?>;
