using MediatR;

namespace GalponERP.Application.Agentes.Chat.Queries.ObtenerHistorialChat;

public record ObtenerHistorialChatQuery(Guid ConversacionId) : IRequest<HistorialChatResponse>;

public record HistorialChatResponse(
    string? Resumen, 
    int UltimoIndiceResumido, 
    IEnumerable<MensajeChatResponse> Mensajes,
    bool Existe = true);

public record MensajeChatResponse(Guid Id, string Rol, string Contenido, DateTime Fecha);
