using GalponERP.Application.Interfaces;
using MediatR;

namespace GalponERP.Application.Galpones.Commands.CrearGalpon;

public record CrearGalponCommand(
    string Nombre,
    int Capacidad,
    string Ubicacion) : IRequest<Guid>, IAuditableCommand;
