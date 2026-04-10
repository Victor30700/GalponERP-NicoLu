using GalponERP.Domain.Entities;
using MediatR;

namespace GalponERP.Application.Gastos.Queries.ObtenerGastos;

public record ObtenerGastosQuery(Guid? GalponId, Guid? LoteId) : IRequest<IEnumerable<GastoOperativo>>;
