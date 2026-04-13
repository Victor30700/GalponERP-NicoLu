using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Usuarios.Commands.CambiarEstadoBloqueoUsuario;

public record CambiarEstadoBloqueoUsuarioCommand(Guid UsuarioId, bool Bloquear) : IRequest<Unit>;

public class CambiarEstadoBloqueoUsuarioCommandHandler : IRequestHandler<CambiarEstadoBloqueoUsuarioCommand, Unit>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CambiarEstadoBloqueoUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CambiarEstadoBloqueoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        if (request.Bloquear)
        {
            usuario.Desactivar();
        }
        else
        {
            usuario.Activar();
        }

        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
