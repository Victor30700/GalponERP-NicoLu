using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
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
    string? Telefono,
    RolGalpon Rol,
    int Active,
    string? Version = null) : IRequest, IAuditableCommand;
