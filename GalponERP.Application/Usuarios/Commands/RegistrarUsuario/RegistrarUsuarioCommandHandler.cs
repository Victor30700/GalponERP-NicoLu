using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.RegistrarUsuario;

public class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuarioExistente = await _usuarioRepository.ObtenerPorFirebaseUidAsync(request.FirebaseUid);
        
        if (usuarioExistente != null)
        {
            return usuarioExistente.Id;
        }

        var usuario = new Usuario(
            Guid.NewGuid(),
            request.FirebaseUid,
            request.Nombre,
            request.Rol);

        _usuarioRepository.Agregar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}
