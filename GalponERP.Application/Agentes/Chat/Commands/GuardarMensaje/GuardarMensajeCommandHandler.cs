using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.GuardarMensaje;

public class GuardarMensajeCommandHandler : IRequestHandler<GuardarMensajeCommand>
{
    private readonly IConversacionRepository _conversacionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GuardarMensajeCommandHandler(IConversacionRepository conversacionRepository, IUnitOfWork unitOfWork)
    {
        _conversacionRepository = conversacionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(GuardarMensajeCommand request, CancellationToken cancellationToken)
    {
        var mensaje = new MensajeChat(Guid.NewGuid(), request.ConversacionId, request.Rol, request.Contenido);
        
        _conversacionRepository.AgregarMensaje(mensaje);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
