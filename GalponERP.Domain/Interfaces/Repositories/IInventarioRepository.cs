using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de Movimientos de Inventario.
/// </summary>
public interface IInventarioRepository
{
    Task<IEnumerable<MovimientoInventario>> ObtenerPorLoteIdAsync(Guid loteId);
    Task<IEnumerable<MovimientoInventario>> ObtenerPorVariosLotesAsync(IEnumerable<Guid> loteIds);
    Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoIdAsync(Guid productoId);
    Task<IEnumerable<MovimientoInventario>> ObtenerTodosAsync();
    Task<decimal> ObtenerStockPorProductoIdAsync(Guid productoId);
    void RegistrarMovimiento(MovimientoInventario movimiento);
    void RegistrarLote(InventarioLote lote);
    Task<IEnumerable<InventarioLote>> ObtenerLotesActivosPorProductoAsync(Guid productoId);
}
