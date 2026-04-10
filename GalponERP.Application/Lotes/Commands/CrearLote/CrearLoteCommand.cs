using MediatR;

namespace GalponERP.Application.Lotes.Commands.CrearLote;

public record CrearLoteCommand(
    DateTime FechaIngreso,
    int CantidadInicial,
    decimal CostoUnitarioPollito) : IRequest<Guid>;
