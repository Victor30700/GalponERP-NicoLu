using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.CrearConversacion;

public class CrearConversacionCommandHandler : IRequestHandler<CrearConversacionCommand, Guid>
{
    private readonly IConversacionRepository _conversacionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearConversacionCommandHandler(IConversacionRepository conversacionRepository, IUnitOfWork unitOfWork)
    {
        _conversacionRepository = conversacionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearConversacionCommand request, CancellationToken cancellationToken)
    {
        var conversacion = new Conversacion(Guid.NewGuid(), request.UsuarioId);
        
        _conversacionRepository.Agregar(conversacion);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return conversacion.Id;
    }
}
