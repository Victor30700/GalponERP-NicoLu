using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class GastoOperativoRepository : IGastoOperativoRepository
{
    private readonly GalponDbContext _context;

    public GastoOperativoRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<GastoOperativo?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.GastosOperativos.FirstOrDefaultAsync(g => g.Id == id && g.IsActive);
    }

    public async Task<IEnumerable<GastoOperativo>> ObtenerPorGalponAsync(Guid galponId)
    {
        return await _context.GastosOperativos
            .Where(g => g.GalponId == galponId && g.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<GastoOperativo>> ObtenerPorLoteAsync(Guid loteId)
    {
        return await _context.GastosOperativos
            .Where(g => g.LoteId == loteId && g.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<GastoOperativo>> ObtenerPorRangoFechasAsync(DateTime inicio, DateTime fin)
    {
        var start = inicio.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(inicio, DateTimeKind.Utc) : inicio.ToUniversalTime();
        var end = fin.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fin, DateTimeKind.Utc) : fin.ToUniversalTime();

        return await _context.GastosOperativos
            .Where(g => g.Fecha >= start && g.Fecha <= end && g.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<GastoOperativo>> ObtenerTodosAsync()
    {
        return await _context.GastosOperativos
            .Where(g => g.IsActive)
            .OrderByDescending(g => g.Fecha)
            .ToListAsync();
    }

    public void Agregar(GastoOperativo gasto)
    {
        _context.GastosOperativos.Add(gasto);
    }

    public void Actualizar(GastoOperativo gasto)
    {
        _context.GastosOperativos.Update(gasto);
    }
}
