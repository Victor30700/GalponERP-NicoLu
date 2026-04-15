using MediatR;

namespace GalponERP.Application.Sanidad.Commands.EliminarBienestar;

public record EliminarBienestarCommand(Guid Id, Guid UsuarioId) : IRequest;
