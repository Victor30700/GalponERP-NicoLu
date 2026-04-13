using MediatR;

namespace GalponERP.Application.Agentes.Chat.Queries.ObtenerConversacionesUsuario;

public record ConversacionResumenResponse(
    Guid Id,
    string Titulo,
    DateTime FechaInicio,
    string? UltimoResumen,
    int TotalMensajes);

public record ObtenerConversacionesUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<ConversacionResumenResponse>>;
