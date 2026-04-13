using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.EliminarConversacion;

public record EliminarConversacionCommand(Guid ConversacionId) : IRequest;
