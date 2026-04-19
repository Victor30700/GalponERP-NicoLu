using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerConsumoAlimentoPdf;

public record ObtenerConsumoAlimentoPdfQuery(Guid LoteId) : IRequest<byte[]>;
