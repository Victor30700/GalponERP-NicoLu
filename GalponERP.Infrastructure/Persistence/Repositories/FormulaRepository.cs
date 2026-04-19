using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class FormulaRepository : IFormulaRepository
{
    private readonly GalponDbContext _context;

    public FormulaRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Formula?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Formulas
            .Include(f => f.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Formula>> ObtenerTodasAsync()
    {
        return await _context.Formulas
            .Include(f => f.Detalles)
            .ToListAsync();
    }

    public void Agregar(Formula formula)
    {
        _context.Formulas.Add(formula);
    }

    public void Actualizar(Formula formula)
    {
        _context.Formulas.Update(formula);
    }

    public void Eliminar(Formula formula)
    {
        _context.Formulas.Remove(formula);
    }
}
