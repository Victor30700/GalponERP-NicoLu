using MediatR;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;

namespace GalponERP.Application.Usuarios.Commands.DesvincularWhatsApp;

public record DesvincularWhatsAppCommand() : IRequest, IAuditableCommand;

public class DesvincularWhatsAppCommandHandler : IRequestHandler<DesvincularWhatsAppCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public DesvincularWhatsAppCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserContext currentUserContext)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task Handle(DesvincularWhatsAppCommand request, CancellationToken cancellationToken)
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

        usuario.DesvincularWhatsApp();
        _usuarioRepository.Actualizar(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
