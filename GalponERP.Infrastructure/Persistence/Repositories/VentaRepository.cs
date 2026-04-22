using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly GalponDbContext _context;

    public VentaRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Venta?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Ventas
            .IgnoreQueryFilters()
            .Include(v => v.Pagos)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venta>> ObtenerPorLoteAsync(Guid loteId)
    {
        return await _context.Ventas
            .Include(v => v.Pagos)
            .Where(v => v.LoteId == loteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venta>> ObtenerPorClienteAsync(Guid clienteId)
    {
        return await _context.Ventas
            .Include(v => v.Pagos)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venta>> ObtenerPorRangoFechasAsync(DateTime inicio, DateTime fin)
    {
        var start = inicio.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(inicio, DateTimeKind.Utc) : inicio.ToUniversalTime();
        var end = fin.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fin, DateTimeKind.Utc) : fin.ToUniversalTime();

        return await _context.Ventas
            .Include(v => v.Pagos)
            .Where(v => v.Fecha >= start && v.Fecha <= end)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venta>> ObtenerTodasAsync()
    {
        return await _context.Ventas
            .Include(v => v.Pagos)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
    }

    public void Agregar(Venta venta)
    {
        _context.Set<Venta>().Add(venta);
    }

    public void Actualizar(Venta venta)
    {
        _context.Set<Venta>().Update(venta);
    }
}
