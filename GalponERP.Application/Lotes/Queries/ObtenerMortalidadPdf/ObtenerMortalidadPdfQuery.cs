using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerMortalidadPdf;

public record ObtenerMortalidadPdfQuery(Guid LoteId) : IRequest<byte[]>;
