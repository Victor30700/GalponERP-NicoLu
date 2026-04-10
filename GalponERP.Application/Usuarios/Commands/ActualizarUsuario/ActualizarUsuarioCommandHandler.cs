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
            throw new Exception($"Usuario con ID {request.Id} no encontrado.");
        }

        usuario.ActualizarNombre(request.Nombre);
        usuario.ActualizarRol(request.Rol);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
