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

    public async Task<IEnumerable<AuditoriaLog>> ObtenerFiltradosAsync(DateTime? desde, DateTime? hasta, Guid? usuarioId, string? entidad)
    {
        var query = _context.AuditoriaLogs.AsQueryable();

        if (desde.HasValue)
        {
            var start = desde.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(desde.Value, DateTimeKind.Utc) : desde.Value.ToUniversalTime();
            query = query.Where(l => l.Fecha >= start);
        }

        if (hasta.HasValue)
        {
            var end = hasta.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(hasta.Value, DateTimeKind.Utc) : hasta.Value.ToUniversalTime();
            query = query.Where(l => l.Fecha <= end);
        }

        if (usuarioId.HasValue && usuarioId != Guid.Empty)
        {
            query = query.Where(l => l.UsuarioId == usuarioId.Value);
        }

        if (!string.IsNullOrWhiteSpace(entidad))
        {
            query = query.Where(l => l.Entidad == entidad);
        }

        return await query
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }
}
