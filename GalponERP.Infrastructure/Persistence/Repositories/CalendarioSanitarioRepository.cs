using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class CalendarioSanitarioRepository : ICalendarioSanitarioRepository
{
    private readonly GalponDbContext _context;

    public CalendarioSanitarioRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<CalendarioSanitario?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<CalendarioSanitario>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CalendarioSanitario>> ObtenerPorLoteIdAsync(Guid loteId)
    {
        return await _context.Set<CalendarioSanitario>()
            .Where(c => c.LoteId == loteId)
            .OrderBy(c => c.DiaDeAplicacion)
            .ToListAsync();
    }

    public void Agregar(CalendarioSanitario calendario)
    {
        _context.Set<CalendarioSanitario>().Add(calendario);
    }

    public void Actualizar(CalendarioSanitario calendario)
    {
        _context.Set<CalendarioSanitario>().Update(calendario);
    }
}
