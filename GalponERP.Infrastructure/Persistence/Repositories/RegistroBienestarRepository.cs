using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class RegistroBienestarRepository : IRegistroBienestarRepository
{
    private readonly GalponDbContext _context;

    public RegistroBienestarRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<RegistroBienestar?> ObtenerPorLoteYFechaAsync(Guid loteId, DateTime fecha)
    {
        var fechaSolo = fecha.Date;
        return await _context.RegistroBienestar
            .FirstOrDefaultAsync(r => r.LoteId == loteId && r.Fecha == fechaSolo);
    }

    public async Task<IEnumerable<RegistroBienestar>> ObtenerPorLoteAsync(Guid loteId)
    {
        return await _context.RegistroBienestar
            .Where(r => r.LoteId == loteId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();
    }

    public void Agregar(RegistroBienestar registro)
    {
        _context.RegistroBienestar.Add(registro);
    }

    public void Actualizar(RegistroBienestar registro)
    {
        _context.RegistroBienestar.Update(registro);
    }
}
