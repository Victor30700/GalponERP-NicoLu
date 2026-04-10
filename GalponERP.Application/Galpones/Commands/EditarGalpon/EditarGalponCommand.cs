using MediatR;

namespace GalponERP.Application.Galpones.Commands.EditarGalpon;

public record EditarGalponCommand(
    Guid Id,
    string Nombre,
    int Capacidad,
    string Ubicacion) : IRequest;
