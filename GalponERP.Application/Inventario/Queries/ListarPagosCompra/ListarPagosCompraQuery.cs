using MediatR;

namespace GalponERP.Application.Inventario.Queries.ListarPagosCompra;

public record ListarPagosCompraQuery(Guid CompraId) : IRequest<IEnumerable<PagoCompraDto>>;
