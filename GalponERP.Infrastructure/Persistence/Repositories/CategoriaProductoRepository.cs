using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class CategoriaProductoRepository : ICategoriaProductoRepository
{
    private readonly GalponDbContext _context;

    public CategoriaProductoRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<CategoriaProducto?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<CategoriaProducto>().FindAsync(id);
    }

    public async Task<IEnumerable<CategoriaProducto>> ObtenerTodasAsync()
    {
        return await _context.Set<CategoriaProducto>().ToListAsync();
    }

    public void Agregar(CategoriaProducto categoria)
    {
        _context.Set<CategoriaProducto>().Add(categoria);
    }

    public void Actualizar(CategoriaProducto categoria)
    {
        _context.Set<CategoriaProducto>().Update(categoria);
    }

    public void Eliminar(CategoriaProducto categoria)
    {
        _context.Set<CategoriaProducto>().Remove(categoria);
    }
}
