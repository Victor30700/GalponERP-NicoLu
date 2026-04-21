using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.ActualizarTitulo;

public record ActualizarTituloCommand(Guid ConversacionId, string NuevoTitulo) : IRequest, IAuditableCommand;
