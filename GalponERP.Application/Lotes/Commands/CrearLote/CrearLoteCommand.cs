using MediatR;

namespace GalponERP.Application.Lotes.Commands.CrearLote;

public record CrearLoteCommand(
    Guid GalponId,
    DateTime FechaIngreso,
    int CantidadInicial,
    decimal CostoUnitarioPollito,
    Guid? PlantillaSanitariaId = null) : IRequest<Guid>;
