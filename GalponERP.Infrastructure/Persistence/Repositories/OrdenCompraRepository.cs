using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class OrdenCompraRepository : IOrdenCompraRepository
{
    private readonly GalponDbContext _context;

    public OrdenCompraRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<OrdenCompra?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.OrdenesCompra
            .Include(o => o.Items)
            .ThenInclude(i => i.Producto)
            .Include(o => o.Proveedor)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<OrdenCompra>> ObtenerPendientesAsync()
    {
        return await _context.OrdenesCompra
            .Include(o => o.Proveedor)
            .Where(o => o.Estado == EstadoOrdenCompra.Pendiente)
            .ToListAsync();
    }

    public void Agregar(OrdenCompra ordenCompra)
    {
        _context.OrdenesCompra.Add(ordenCompra);
    }

    public void Actualizar(OrdenCompra ordenCompra)
    {
        _context.OrdenesCompra.Update(ordenCompra);
    }
}
