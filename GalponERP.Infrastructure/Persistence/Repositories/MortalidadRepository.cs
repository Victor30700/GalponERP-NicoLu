using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class MortalidadRepository : IMortalidadRepository
{
    private readonly GalponDbContext _context;

    public MortalidadRepository(GalponDbContext context)
    {
        _context = context;
    }

    public void Agregar(MortalidadDiaria mortalidad)
    {
        _context.Mortalidades.Add(mortalidad);
    }

    public async Task<IEnumerable<MortalidadDiaria>> ObtenerPorLoteAsync(Guid loteId)
    {
        return await _context.Mortalidades
            .Where(m => m.LoteId == loteId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<MortalidadDiaria>> ObtenerPorRangoFechasAsync(DateTime inicio, DateTime fin)
    {
        return await _context.Mortalidades
            .Where(m => m.Fecha >= inicio && m.Fecha <= fin)
            .ToListAsync();
    }

    public async Task<IEnumerable<MortalidadDiaria>> ObtenerTodasAsync()
    {
        return await _context.Mortalidades.ToListAsync();
    }
}
