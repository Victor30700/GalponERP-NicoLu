using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class PlantillaSanitariaRepository : IPlantillaSanitariaRepository
{
    private readonly GalponDbContext _context;

    public PlantillaSanitariaRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<PlantillaSanitaria?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<PlantillaSanitaria>()
            .IgnoreQueryFilters()
            .Include(p => p.Actividades)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    public async Task<IEnumerable<PlantillaSanitaria>> ObtenerTodasAsync()
    {
        return await _context.Set<PlantillaSanitaria>()
            .Include(p => p.Actividades)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public void Agregar(PlantillaSanitaria plantilla)
    {
        _context.Set<PlantillaSanitaria>().Add(plantilla);
    }

    public void Actualizar(PlantillaSanitaria plantilla)
    {
        if (_context.Entry(plantilla).State == EntityState.Detached)
        {
            _context.Set<PlantillaSanitaria>().Update(plantilla);
        }
    }
}
