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
        return await _context.Set<Producto>().FindAsync(id);
    }

    public async Task<IEnumerable<Producto>> ObtenerPorTipoAsync(TipoProducto tipo)
    {
        return await _context.Set<Producto>()
            .Where(p => p.Tipo == tipo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
    {
        return await _context.Set<Producto>().ToListAsync();
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
