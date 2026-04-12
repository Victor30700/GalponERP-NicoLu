using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerReporteCierrePdf;

public record ObtenerReporteCierreLotePdfQuery(Guid LoteId) : IRequest<byte[]>;
