using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerStockActual;

public record ObtenerStockActualQuery(Guid? ProductoId = null) : IRequest<IEnumerable<StockProductoResponse>>;

public record StockProductoResponse(
    Guid ProductoId,
    string NombreProducto,
    string TipoProducto,
    decimal StockActual,
    string UnidadMedida);

public class ObtenerStockActualQueryHandler : IRequestHandler<ObtenerStockActualQuery, IEnumerable<StockProductoResponse>>
{
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;

    public ObtenerStockActualQueryHandler(IInventarioRepository inventarioRepository, IProductoRepository productoRepository)
    {
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<StockProductoResponse>> Handle(ObtenerStockActualQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        if (request.ProductoId.HasValue)
        {
            var p = await _productoRepository.ObtenerPorIdAsync(request.ProductoId.Value);
            productos = p != null ? new List<Producto> { p } : new List<Producto>();
        }

        var movimientos = await _inventarioRepository.ObtenerTodosAsync();

        var stockPorProducto = movimientos
            .GroupBy(m => m.ProductoId)
            .Select(g => new
            {
                ProductoId = g.Key,
                Stock = g.Sum(m => (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada) ? m.Cantidad : -m.Cantidad)
            })
            .ToDictionary(x => x.ProductoId, x => x.Stock);

        return productos.Select(p => new StockProductoResponse(
            p.Id,
            p.Nombre,
            p.Categoria?.Nombre ?? "Sin Categoria",
            stockPorProducto.ContainsKey(p.Id) ? stockPorProducto[p.Id] : 0,
            p.Unidad?.Nombre ?? "Sin Unidad"));
    }
}
