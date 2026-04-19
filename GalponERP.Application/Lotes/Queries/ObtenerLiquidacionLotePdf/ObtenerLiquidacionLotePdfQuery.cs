using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerLiquidacionLotePdf;

public record ObtenerLiquidacionLotePdfQuery(Guid LoteId) : IRequest<byte[]>;
