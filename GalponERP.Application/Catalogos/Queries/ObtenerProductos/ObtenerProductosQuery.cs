using MediatR;

namespace GalponERP.Application.Catalogos.Queries.ObtenerProductos;

public record ObtenerProductosQuery() : IRequest<IEnumerable<ProductoResponse>>;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    string Tipo,
    string UnidadMedida);
