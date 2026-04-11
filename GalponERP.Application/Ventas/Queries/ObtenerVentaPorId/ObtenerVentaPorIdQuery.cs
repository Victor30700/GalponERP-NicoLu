using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using MediatR;

namespace GalponERP.Application.Ventas.Queries.ObtenerVentaPorId;

public record ObtenerVentaPorIdQuery(Guid Id) : IRequest<VentaResponse?>;
