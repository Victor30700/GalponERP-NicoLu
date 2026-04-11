using MediatR;

namespace GalponERP.Application.Pesajes.Queries.ObtenerPesajesPorLote;

public record ObtenerPesajesPorLoteQuery(Guid LoteId) : IRequest<IEnumerable<PesajeResponse>>;

public record PesajeResponse(
    Guid Id,
    Guid LoteId,
    DateTime Fecha,
    decimal PesoPromedioGramos,
    int CantidadMuestreada);
