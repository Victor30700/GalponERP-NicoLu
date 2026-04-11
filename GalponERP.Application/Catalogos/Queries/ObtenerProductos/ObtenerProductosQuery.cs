using MediatR;

namespace GalponERP.Application.Catalogos.Queries.ObtenerProductos;

public record ObtenerProductosQuery() : IRequest<IEnumerable<ProductoResponse>>;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    Guid CategoriaId,
    string CategoriaNombre,
    Guid UnidadId,
    string UnidadMedidaNombre,
    decimal EquivalenciaEnKg);
