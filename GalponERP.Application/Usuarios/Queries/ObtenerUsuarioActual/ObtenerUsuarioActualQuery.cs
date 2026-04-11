using GalponERP.Domain.Entities;
using MediatR;
using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;

namespace GalponERP.Application.Usuarios.Queries.ObtenerUsuarioActual;

public record ObtenerUsuarioActualQuery(string FirebaseUid) : IRequest<UsuarioResponse?>;
