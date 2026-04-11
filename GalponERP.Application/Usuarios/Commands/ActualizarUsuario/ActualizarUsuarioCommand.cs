using MediatR;

namespace GalponERP.Application.Usuarios.Commands.ActualizarUsuario;

public record ActualizarUsuarioCommand(
    Guid Id,
    string Email,
    string Nombre,
    string Apellidos,
    DateTime FechaNacimiento,
    string Direccion,
    string Profesion,
    string Rol) : IRequest;
