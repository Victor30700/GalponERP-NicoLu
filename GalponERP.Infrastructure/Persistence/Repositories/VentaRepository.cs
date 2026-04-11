using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly GalponDbContext _context;

    public VentaRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Venta?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<Venta>().FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venta>> ObtenerPorLoteAsync(Guid loteId)
    {
        return await _context.Ventas
            .Where(v => v.LoteId == loteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venta>> ObtenerTodasAsync()
    {
        return await _context.Ventas
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
    }

    public void Agregar(Venta venta)

    {
        _context.Set<Venta>().Add(venta);
    }
}
