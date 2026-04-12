using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerPagosPorVenta;

public record ObtenerPagosPorVentaQuery(Guid VentaId) : IRequest<IEnumerable<PagoResponse>>;
