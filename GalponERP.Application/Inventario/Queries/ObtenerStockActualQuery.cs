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
                // Auditoría Sprint 47: La fórmula debe coincidir exactamente con el Kárdex y Dashboard
                // Entradas: Entrada, AjusteEntrada, Compra
                // Salidas: Salida, AjusteSalida (y cualquier otro no contemplado en entradas)
                Stock = g.Sum(m => (m.Tipo == TipoMovimiento.Entrada || 
                                   m.Tipo == TipoMovimiento.Compra || 
                                   m.Tipo == TipoMovimiento.AjusteEntrada) 
                                  ? m.Cantidad : -m.Cantidad)
            })
            .ToDictionary(x => x.ProductoId, x => x.Stock);

        return productos.Select(p => {
            decimal stockActual = stockPorProducto.ContainsKey(p.Id) ? stockPorProducto[p.Id] : 0;
            return new StockProductoResponse(
                p.Id,
                p.Nombre,
                p.Categoria?.Nombre ?? "Sin Categoria",
                stockActual,
                Math.Round(stockActual * p.EquivalenciaEnKg, 2),
                p.Unidad?.Nombre ?? "Sin Unidad");
        });
    }
}
