using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerIngresoLotePdf;

public record ObtenerIngresoLotePdfQuery(Guid LoteId) : IRequest<byte[]>;
