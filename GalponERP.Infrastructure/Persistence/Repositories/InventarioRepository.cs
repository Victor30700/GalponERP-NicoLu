using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class InventarioRepository : IInventarioRepository
{
    private readonly GalponDbContext _context;

    public InventarioRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorLoteIdAsync(Guid loteId)
    {
        return await _context.MovimientosInventario
            .Where(m => m.LoteId == loteId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoIdAsync(Guid productoId)
    {
        return await _context.MovimientosInventario
            .Where(m => m.ProductoId == productoId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerTodosAsync()
    {
        return await _context.MovimientosInventario
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<decimal> ObtenerStockPorProductoIdAsync(Guid productoId)
    {
        var entradas = await _context.MovimientosInventario
            .Where(m => m.ProductoId == productoId && (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.AjusteEntrada || m.Tipo == TipoMovimiento.Compra))
            .SumAsync(m => m.Cantidad);

        var salidas = await _context.MovimientosInventario
            .Where(m => m.ProductoId == productoId && (m.Tipo == TipoMovimiento.Salida || m.Tipo == TipoMovimiento.AjusteSalida))
            .SumAsync(m => m.Cantidad);

        return entradas - salidas;
    }

    public void RegistrarMovimiento(MovimientoInventario movimiento)
    {
        _context.MovimientosInventario.Add(movimiento);
    }
}
