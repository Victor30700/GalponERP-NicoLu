using MediatR;

namespace GalponERP.Application.Dashboard.Queries.ObtenerProyeccionSacrificio;

public record ObtenerProyeccionSacrificioQuery(Guid LoteId) : IRequest<ProyeccionSacrificioResponse?>;

public record ProyeccionSacrificioResponse(
    Guid LoteId,
    decimal PesoActualGramos,
    decimal FCRActual,
    int DiasDeVida,
    decimal GananciaDiariaEstimadaGramos,
    decimal PesoObjetivoGramos,
    int DiasRestantes,
    DateTime FechaSacrificio);
