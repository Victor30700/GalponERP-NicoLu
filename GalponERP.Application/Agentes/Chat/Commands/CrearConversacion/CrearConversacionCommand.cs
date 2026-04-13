using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.CrearConversacion;

public record CrearConversacionCommand(Guid UsuarioId) : IRequest<Guid>;
