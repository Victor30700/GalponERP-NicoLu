using MediatR;

namespace GalponERP.Application.Usuarios.Commands.EliminarUsuario;

public record EliminarUsuarioCommand(Guid Id) : IRequest;
