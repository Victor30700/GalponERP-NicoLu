using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Calendario.Queries.ObtenerCalendarioPorLote;

public record ObtenerCalendarioPorLoteQuery(Guid LoteId) : IRequest<IEnumerable<CalendarioSanitario>>;
