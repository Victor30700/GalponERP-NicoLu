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
    decimal StockActualKg,
    string UnidadMedida,
    DateTime? FechaVencimientoProxima = null);

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
        var productos = (await _productoRepository.ObtenerTodosAsync()).ToList();
        
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
                Stock = g.Sum(m => (m.Tipo == TipoMovimiento.Entrada || 
                                   m.Tipo == TipoMovimiento.Compra || 
                                   m.Tipo == TipoMovimiento.AjusteEntrada) 
                                  ? m.Cantidad : -m.Cantidad)
            })
            .ToDictionary(x => x.ProductoId, x => x.Stock);

        // Obtener vencimientos próximos
        var vencimientos = new Dictionary<Guid, DateTime?>();
        foreach (var prod in productos)
        {
            var lotes = await _inventarioRepository.ObtenerLotesActivosPorProductoAsync(prod.Id);
            vencimientos[prod.Id] = lotes.OrderBy(l => l.FechaVencimiento).FirstOrDefault()?.FechaVencimiento;
        }

        return productos.Select(p => {
            decimal stockActual = stockPorProducto.ContainsKey(p.Id) ? stockPorProducto[p.Id] : 0;
            return new StockProductoResponse(
                p.Id,
                p.Nombre,
                p.Categoria?.Nombre ?? "Sin Categoria",
                stockActual,
                Math.Round(stockActual * p.PesoUnitarioKg, 2),
                p.Unidad?.Nombre ?? "Sin Unidad",
                vencimientos.ContainsKey(p.Id) ? vencimientos[p.Id] : null);
        });
    }
}
