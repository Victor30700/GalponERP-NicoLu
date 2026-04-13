using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Queries.ObtenerHistorialChat;

public class ObtenerHistorialChatQueryHandler : IRequestHandler<ObtenerHistorialChatQuery, HistorialChatResponse>
{
    private readonly IConversacionRepository _conversacionRepository;

    public ObtenerHistorialChatQueryHandler(IConversacionRepository conversacionRepository)
    {
        _conversacionRepository = conversacionRepository;
    }

    public async Task<HistorialChatResponse> Handle(ObtenerHistorialChatQuery request, CancellationToken cancellationToken)
    {
        var conversacion = await _conversacionRepository.ObtenerPorIdAsync(request.ConversacionId);
        if (conversacion == null)
        {
            return new HistorialChatResponse(null, 0, new List<MensajeChatResponse>(), false);
        }

        // Recuperar los últimos 10 mensajes
        var mensajes = await _conversacionRepository.ObtenerUltimosMensajesAsync(request.ConversacionId, 10);
        
        var mensajesResponse = mensajes.Select(m => new MensajeChatResponse(m.Id, m.Rol, m.Contenido, m.Fecha));

        return new HistorialChatResponse(
            conversacion.ResumenActual,
            conversacion.UltimoIndiceMensajeResumido,
            mensajesResponse);
    }
}
