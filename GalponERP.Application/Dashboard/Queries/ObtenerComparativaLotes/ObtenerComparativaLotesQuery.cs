using MediatR;

namespace GalponERP.Application.Dashboard.Queries.ObtenerComparativaLotes;

public record ObtenerComparativaLotesQuery() : IRequest<IEnumerable<LoteComparativoResponse>>;

public record LoteComparativoResponse(
    Guid LoteId,
    DateTime FechaIngreso,
    int CantidadInicial,
    int MortalidadTotal,
    decimal FCRFinal,
    decimal TotalVentas,
    decimal TotalGastos,
    decimal UtilidadNeta);
