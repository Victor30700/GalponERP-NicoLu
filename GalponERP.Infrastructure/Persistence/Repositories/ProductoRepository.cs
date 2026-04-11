using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly GalponDbContext _context;

    public ProductoRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Producto?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<Producto>()
            .Include(p => p.Categoria)
            .Include(p => p.Unidad)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Producto>> ObtenerPorCategoriaAsync(Guid categoriaId)
    {
        return await _context.Set<Producto>()
            .Include(p => p.Categoria)
            .Include(p => p.Unidad)
            .Where(p => p.CategoriaProductoId == categoriaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
    {
        return await _context.Set<Producto>()
            .Include(p => p.Categoria)
            .Include(p => p.Unidad)
            .ToListAsync();
    }

    public void Agregar(Producto producto)
    {
        _context.Set<Producto>().Add(producto);
    }

    public void Actualizar(Producto producto)
    {
        _context.Set<Producto>().Update(producto);
    }
}
