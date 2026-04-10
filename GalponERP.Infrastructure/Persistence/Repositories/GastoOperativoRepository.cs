using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class GastoOperativoRepository : IGastoOperativoRepository
{
    private readonly GalponDbContext _context;

    public GastoOperativoRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<GastoOperativo?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.GastosOperativos.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<GastoOperativo>> ObtenerPorGalponAsync(Guid galponId)
    {
        return await _context.GastosOperativos
            .Where(g => g.GalponId == galponId)
            .ToListAsync();
    }

    public async Task<IEnumerable<GastoOperativo>> ObtenerPorLoteAsync(Guid loteId)
    {
        return await _context.GastosOperativos
            .Where(g => g.LoteId == loteId)
            .ToListAsync();
    }

    public void Agregar(GastoOperativo gasto)
    {
        _context.GastosOperativos.Add(gasto);
    }

    public void Actualizar(GastoOperativo gasto)
    {
        _context.GastosOperativos.Update(gasto);
    }
}
