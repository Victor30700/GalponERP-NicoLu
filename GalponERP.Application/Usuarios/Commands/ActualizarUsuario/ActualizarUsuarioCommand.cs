using MediatR;

namespace GalponERP.Application.Usuarios.Commands.ActualizarUsuario;

public record ActualizarUsuarioCommand(
    Guid Id,
    string Nombre,
    string Rol) : IRequest;
