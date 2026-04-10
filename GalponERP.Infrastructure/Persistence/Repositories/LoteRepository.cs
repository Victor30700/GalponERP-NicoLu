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
        return await _context.Lotes.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Lote>> ObtenerActivosAsync()
    {
        return await _context.Lotes
            .Where(l => l.Estado == EstadoLote.Activo)
            .ToListAsync();
    }

    public void Agregar(Lote lote)
    {
        _context.Lotes.Add(lote);
    }

    public void Actualizar(Lote lote)
    {
        _context.Lotes.Update(lote);
    }
}
