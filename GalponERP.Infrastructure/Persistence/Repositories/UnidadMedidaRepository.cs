using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class UnidadMedidaRepository : IUnidadMedidaRepository
{
    private readonly GalponDbContext _context;

    public UnidadMedidaRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<UnidadMedida?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<UnidadMedida>().FindAsync(id);
    }

    public async Task<IEnumerable<UnidadMedida>> ObtenerTodasAsync()
    {
        return await _context.Set<UnidadMedida>().ToListAsync();
    }

    public void Agregar(UnidadMedida unidadMedida)
    {
        _context.Set<UnidadMedida>().Add(unidadMedida);
    }

    public void Actualizar(UnidadMedida unidadMedida)
    {
        _context.Set<UnidadMedida>().Update(unidadMedida);
    }

    public void Eliminar(UnidadMedida unidadMedida)
    {
        _context.Set<UnidadMedida>().Remove(unidadMedida);
    }
}
