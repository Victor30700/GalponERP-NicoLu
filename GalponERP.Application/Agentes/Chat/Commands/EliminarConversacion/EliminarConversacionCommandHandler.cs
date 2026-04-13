using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.EliminarConversacion;

public class EliminarConversacionCommandHandler : IRequestHandler<EliminarConversacionCommand>
{
    private readonly IConversacionRepository _conversacionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarConversacionCommandHandler(IConversacionRepository conversacionRepository, IUnitOfWork unitOfWork)
    {
        _conversacionRepository = conversacionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EliminarConversacionCommand request, CancellationToken cancellationToken)
    {
        var conversacion = await _conversacionRepository.ObtenerPorIdAsync(request.ConversacionId);
        
        if (conversacion == null)
        {
            throw new Exception("Conversación no encontrada.");
        }

        _conversacionRepository.Eliminar(conversacion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
