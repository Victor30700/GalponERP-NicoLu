using MediatR;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;

namespace GalponERP.Application.Usuarios.Commands.GenerarCodigoWhatsApp;

public record GenerarCodigoWhatsAppCommand() : IRequest<string>;

public class GenerarCodigoWhatsAppCommandHandler : IRequestHandler<GenerarCodigoWhatsAppCommand, string>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public GenerarCodigoWhatsAppCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserContext currentUserContext)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<string> Handle(GenerarCodigoWhatsAppCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.UsuarioId.HasValue)
        {
            throw new Exception("Usuario no autenticado.");
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(_currentUserContext.UsuarioId.Value);
        if (usuario == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        usuario.GenerarCodigoVinculacion();
        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return usuario.CodigoVinculacion!;
    }
}
