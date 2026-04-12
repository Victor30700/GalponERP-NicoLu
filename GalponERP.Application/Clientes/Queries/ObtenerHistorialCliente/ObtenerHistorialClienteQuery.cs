using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using MediatR;

namespace GalponERP.Application.Clientes.Queries.ObtenerHistorialCliente;

public record ObtenerHistorialClienteQuery(Guid ClienteId) : IRequest<IEnumerable<VentaResponse>>;
