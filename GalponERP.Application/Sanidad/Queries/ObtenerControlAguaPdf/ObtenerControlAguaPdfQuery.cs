using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerControlAguaPdf;

public record ObtenerControlAguaPdfQuery(Guid LoteId, DateTime Mes) : IRequest<byte[]>;
