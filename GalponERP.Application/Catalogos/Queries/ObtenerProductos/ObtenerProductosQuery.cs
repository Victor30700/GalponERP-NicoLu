using MediatR;
using GalponERP.Application.Productos.Queries;

namespace GalponERP.Application.Catalogos.Queries.ObtenerProductos;

public record ObtenerProductosQuery() : IRequest<IEnumerable<ProductoResponse>>;
