using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class LoteRepository : ILoteRepository
{
    private readonly GalponDbContext _context;

    public LoteRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Lote?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<Lote>()
            .Include(l => l.Galpon)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Lote>> ObtenerActivosAsync()
    {
        return await _context.Set<Lote>()
            .Include(l => l.Galpon)
            .Where(l => l.Estado == EstadoLote.Activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lote>> ObtenerTodosAsync()
    {
        return await _context.Set<Lote>()
            .Include(l => l.Galpon)
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    public void Agregar(Lote lote)
    {
        _context.Set<Lote>().Add(lote);
    }

    public void Actualizar(Lote lote)
    {
        _context.Set<Lote>().Update(lote);
    }

    public void Eliminar(Lote lote)
    {
        lote.Eliminar();
        _context.Set<Lote>().Update(lote);
    }
}
