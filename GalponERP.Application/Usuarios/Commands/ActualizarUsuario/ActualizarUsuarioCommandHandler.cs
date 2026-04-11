using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.ActualizarUsuario;

public class ActualizarUsuarioCommandHandler : IRequestHandler<ActualizarUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.Id);

        if (usuario == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        usuario.ActualizarPerfil(
            request.Email,
            request.Nombre,
            request.Apellidos,
            request.FechaNacimiento,
            request.Direccion,
            request.Profesion,
            request.Rol);

        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
