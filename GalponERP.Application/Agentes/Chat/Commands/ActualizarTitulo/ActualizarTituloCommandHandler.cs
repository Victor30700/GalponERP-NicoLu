using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.ActualizarTitulo;

public class ActualizarTituloCommandHandler : IRequestHandler<ActualizarTituloCommand>
{
    private readonly IConversacionRepository _conversacionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarTituloCommandHandler(IConversacionRepository conversacionRepository, IUnitOfWork unitOfWork)
    {
        _conversacionRepository = conversacionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarTituloCommand request, CancellationToken cancellationToken)
    {
        var conversacion = await _conversacionRepository.ObtenerPorIdAsync(request.ConversacionId);
        
        if (conversacion != null)
        {
            conversacion.ActualizarTitulo(request.NuevoTitulo);
            _conversacionRepository.Actualizar(conversacion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
