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

    public void Actualizar(MortalidadDiaria mortalidad)
    {
        _context.Mortalidades.Update(mortalidad);
    }

    public async Task<MortalidadDiaria?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Mortalidades
            .FirstOrDefaultAsync(m => m.Id == id);
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
        // Asegurar que las fechas sean UTC para evitar errores de Npgsql
        var start = inicio.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(inicio, DateTimeKind.Utc) : inicio.ToUniversalTime();
        var end = fin.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fin, DateTimeKind.Utc) : fin.ToUniversalTime();

        return await _context.Mortalidades
            .Where(m => m.Fecha >= start && m.Fecha <= end)
            .ToListAsync();
    }

    public async Task<IEnumerable<MortalidadDiaria>> ObtenerTodasAsync()
    {
        return await _context.Mortalidades
            .ToListAsync();
    }
}
