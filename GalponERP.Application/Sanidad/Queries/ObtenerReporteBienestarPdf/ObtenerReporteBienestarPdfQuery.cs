using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerReporteBienestarPdf;

public record ObtenerReporteBienestarPdfQuery(Guid RegistroId) : IRequest<byte[]>;
