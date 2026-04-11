using MediatR;

namespace GalponERP.Application.Lotes.Commands.CerrarLote;

public record CerrarLoteResponse(
    Guid LoteId,
    decimal TotalIngresos,
    decimal TotalCostos,
    decimal UtilidadNeta,
    decimal FCR,
    decimal PorcentajeMortalidad);

public record CerrarLoteCommand(Guid LoteId) : IRequest<CerrarLoteResponse>;
