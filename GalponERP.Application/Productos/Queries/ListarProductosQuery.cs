using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using GalponERP.Application.Productos.Queries;

namespace GalponERP.Application.Productos.Queries.ListarProductos;

public record ListarProductosQuery() : IRequest<IEnumerable<ProductoResponse>>;

public class ListarProductosQueryHandler : IRequestHandler<ListarProductosQuery, IEnumerable<ProductoResponse>>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IInventarioRepository _inventarioRepository;

    public ListarProductosQueryHandler(IProductoRepository productoRepository, IInventarioRepository inventarioRepository)
    {
        _productoRepository = productoRepository;
        _inventarioRepository = inventarioRepository;
    }

    public async Task<IEnumerable<ProductoResponse>> Handle(ListarProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        var movimientos = await _inventarioRepository.ObtenerTodosAsync();
        
        var stockPorProducto = movimientos
            .GroupBy(m => m.ProductoId)
            .ToDictionary(
                g => g.Key, 
                g => g.Sum(m => m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada || m.Tipo == TipoMovimiento.Compra 
                    ? m.Cantidad 
                    : -m.Cantidad)
            );

        return productos.Select(p => {
            var stockUnidades = stockPorProducto.ContainsKey(p.Id) ? stockPorProducto[p.Id] : 0;
            
            // Sincronización en caliente para asegurar que el display sea correcto
            // incluso si el valor en la DB aún es 0 por ser un campo nuevo.
            p.SincronizarStockKg(stockUnidades);

            return new ProductoResponse(
                p.Id,
                p.Nombre,
                p.CategoriaProductoId,
                p.Categoria?.Nombre ?? "Sin Categoría",
                p.UnidadMedidaId,
                p.Unidad?.Nombre ?? "Sin Unidad",
                p.PesoUnitarioKg,
                p.UmbralMinimo,
                stockUnidades,
                p.StockActualKg,
                p.IsActive);
        });
    }
}
