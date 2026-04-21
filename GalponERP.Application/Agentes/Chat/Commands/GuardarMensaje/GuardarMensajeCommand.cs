using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Agentes.Chat.Commands.GuardarMensaje;

public record GuardarMensajeCommand(Guid ConversacionId, string Rol, string Contenido) : IRequest, IAuditableCommand;
