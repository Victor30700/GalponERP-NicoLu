using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly GalponDbContext _context;

    public ProveedorRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<Proveedor>().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Proveedor?> ObtenerPorIdIncluyendoInactivosAsync(Guid id)
    {
        return await _context.Set<Proveedor>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Proveedor>> ObtenerTodosAsync()
    {
        return await _context.Set<Proveedor>().ToListAsync();
    }

    public void Agregar(Proveedor proveedor)
    {
        _context.Set<Proveedor>().Add(proveedor);
    }

    public void Actualizar(Proveedor proveedor)
    {
        _context.Set<Proveedor>().Update(proveedor);
    }
}
