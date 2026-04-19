using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerFichaSemanalPdf;

public record ObtenerFichaSemanalPdfQuery(Guid LoteId) : IRequest<byte[]>;
