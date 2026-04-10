using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de Movimientos de Inventario.
/// </summary>
public interface IInventarioRepository
{
    Task<IEnumerable<MovimientoInventario>> ObtenerPorLoteIdAsync(Guid loteId);
    Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoIdAsync(Guid productoId);
    Task<IEnumerable<MovimientoInventario>> ObtenerTodosAsync();
    void RegistrarMovimiento(MovimientoInventario movimiento);
}
