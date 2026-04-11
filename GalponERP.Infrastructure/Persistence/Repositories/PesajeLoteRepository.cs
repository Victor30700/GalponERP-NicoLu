using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class PesajeLoteRepository : IPesajeLoteRepository
{
    private readonly GalponDbContext _context;

    public PesajeLoteRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<PesajeLote?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.PesajesLote.FindAsync(id);
    }

    public async Task<IEnumerable<PesajeLote>> ObtenerPorLoteIdAsync(Guid loteId)
    {
        return await _context.PesajesLote
            .Where(p => p.LoteId == loteId)
            .OrderBy(p => p.Fecha)
            .ToListAsync();
    }

    public void Agregar(PesajeLote pesaje)
    {
        _context.PesajesLote.Add(pesaje);
    }

    public void Actualizar(PesajeLote pesaje)
    {
        _context.PesajesLote.Update(pesaje);
    }

    public void Eliminar(PesajeLote pesaje)
    {
        // Aplicamos Soft Delete si la entidad lo soporta (hereda de Entity que tiene IsActive)
        pesaje.Eliminar(); 
        _context.PesajesLote.Update(pesaje);
    }
}
