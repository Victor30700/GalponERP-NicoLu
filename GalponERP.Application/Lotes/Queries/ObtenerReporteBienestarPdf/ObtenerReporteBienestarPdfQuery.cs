using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerReporteBienestarPdf;

public record ObtenerReporteBienestarPdfQuery(Guid LoteId, DateTime? Fecha = null) : IRequest<byte[]>;
