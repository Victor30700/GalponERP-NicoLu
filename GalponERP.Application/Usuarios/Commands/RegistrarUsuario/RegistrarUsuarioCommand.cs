using MediatR;

namespace GalponERP.Application.Usuarios.Commands.RegistrarUsuario;

public record RegistrarUsuarioCommand(
    string Email,
    string Password,
    string Nombre,
    string Apellidos,
    DateTime FechaNacimiento,
    string Direccion,
    string Profesion,
    string Rol) : IRequest<Guid>;
