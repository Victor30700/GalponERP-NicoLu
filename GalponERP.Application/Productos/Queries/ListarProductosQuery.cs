using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Productos.Queries.ListarProductos;

public record ListarProductosQuery() : IRequest<IEnumerable<ProductoResponse>>;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    Guid CategoriaProductoId,
    string CategoriaNombre,
    Guid UnidadMedidaId,
    string UnidadMedidaNombre,
    decimal EquivalenciaEnKg,
    bool IsActive);

public class ListarProductosQueryHandler : IRequestHandler<ListarProductosQuery, IEnumerable<ProductoResponse>>
{
    private readonly IProductoRepository _productoRepository;

    public ListarProductosQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<ProductoResponse>> Handle(ListarProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        return productos.Select(p => new ProductoResponse(
            p.Id,
            p.Nombre,
            p.CategoriaProductoId,
            p.Categoria?.Nombre ?? "Sin Categoría",
            p.UnidadMedidaId,
            p.Unidad?.Nombre ?? "Sin Unidad",
            p.EquivalenciaEnKg,
            p.IsActive));
    }
}
