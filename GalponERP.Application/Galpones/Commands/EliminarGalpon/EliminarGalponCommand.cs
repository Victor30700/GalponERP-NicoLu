using MediatR;

namespace GalponERP.Application.Galpones.Commands.EliminarGalpon;

public record EliminarGalponCommand(Guid Id) : IRequest;
