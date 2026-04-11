using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly GalponDbContext _context;

    public AuditoriaRepository(GalponDbContext context)
    {
        _context = context;
    }

    public void Agregar(AuditoriaLog log)
    {
        _context.AuditoriaLogs.Add(log);
    }

    public async Task<IEnumerable<AuditoriaLog>> ObtenerTodosAsync()
    {
        return await _context.AuditoriaLogs
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }
}
