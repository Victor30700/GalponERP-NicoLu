using MediatR;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;

namespace GalponERP.Application.Ventas.Queries.ObtenerVentasPorLote;

public record ObtenerVentasPorLoteQuery(Guid LoteId) : IRequest<IEnumerable<VentaResponse>>;
