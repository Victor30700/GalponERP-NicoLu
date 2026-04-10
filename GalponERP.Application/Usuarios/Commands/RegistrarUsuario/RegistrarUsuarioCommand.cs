using MediatR;

namespace GalponERP.Application.Usuarios.Commands.RegistrarUsuario;

public record RegistrarUsuarioCommand(
    string FirebaseUid,
    string Nombre,
    string Rol) : IRequest<Guid>;
