using MediatR;

namespace GalponERP.Application.Sanidad.Queries.ObtenerCalendarioSanitarioPdf;

public record ObtenerCalendarioSanitarioPdfQuery(Guid LoteId) : IRequest<byte[]>;
