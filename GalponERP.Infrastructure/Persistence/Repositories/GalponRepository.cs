using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class GalponRepository : IGalponRepository
{
    private readonly GalponDbContext _context;

    public GalponRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Galpon?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Galpones.FindAsync(id);
    }

    public async Task<IEnumerable<Galpon>> ObtenerTodosAsync()
    {
        return await _context.Galpones.ToListAsync();
    }

    public void Agregar(Galpon galpon)
    {
        _context.Galpones.Add(galpon);
    }

    public void Actualizar(Galpon galpon)
    {
        _context.Galpones.Update(galpon);
    }
}
