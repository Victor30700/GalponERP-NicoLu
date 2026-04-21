using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Sanidad.Commands.EliminarBienestar;

public record EliminarBienestarCommand(Guid Id, Guid UsuarioId) : IRequest, IAuditableCommand;
