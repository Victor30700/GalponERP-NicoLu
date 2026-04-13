using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Queries.ObtenerConversacionesUsuario;

public class ObtenerConversacionesUsuarioQueryHandler : IRequestHandler<ObtenerConversacionesUsuarioQuery, IEnumerable<ConversacionResumenResponse>>
{
    private readonly IConversacionRepository _conversacionRepository;

    public ObtenerConversacionesUsuarioQueryHandler(IConversacionRepository conversacionRepository)
    {
        _conversacionRepository = conversacionRepository;
    }

    public async Task<IEnumerable<ConversacionResumenResponse>> Handle(ObtenerConversacionesUsuarioQuery request, CancellationToken cancellationToken)
    {
        var conversaciones = await _conversacionRepository.ObtenerPorUsuarioAsync(request.UsuarioId);
        
        return conversaciones
            .OrderByDescending(c => c.FechaInicio)
            .Select(c => new ConversacionResumenResponse(
                c.Id,
                c.Titulo,
                c.FechaInicio,
                c.ResumenActual,
                c.Mensajes?.Count ?? 0));
    }
}
