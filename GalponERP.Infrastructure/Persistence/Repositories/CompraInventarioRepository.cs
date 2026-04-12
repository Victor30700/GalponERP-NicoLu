using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class CompraInventarioRepository : ICompraInventarioRepository
{
    private readonly GalponDbContext _context;

    public CompraInventarioRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<CompraInventario?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.ComprasInventario
            .Include(c => c.Pagos)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }

    public async Task<IEnumerable<CompraInventario>> ObtenerPorProveedorIdAsync(Guid proveedorId)
    {
        return await _context.ComprasInventario
            .Include(c => c.Pagos)
            .Where(c => c.ProveedorId == proveedorId && c.IsActive)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<CompraInventario>> ObtenerTodasAsync()
    {
        return await _context.ComprasInventario
            .Include(c => c.Pagos)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    public void Agregar(CompraInventario compra)
    {
        _context.Set<CompraInventario>().Add(compra);
    }

    public void Actualizar(CompraInventario compra)
    {
        _context.Set<CompraInventario>().Update(compra);
    }
}
