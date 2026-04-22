using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly GalponDbContext _context;

    public ClienteRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<Cliente>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> ObtenerPorIdIncluyendoInactivosAsync(Guid id)
    {
        return await _context.Set<Cliente>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
    {
        return await _context.Set<Cliente>().ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorRucAsync(string ruc)
    {
        return await _context.Set<Cliente>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Ruc == ruc);
    }

    public void Agregar(Cliente cliente)
    {
        _context.Set<Cliente>().Add(cliente);
    }

    public void Actualizar(Cliente cliente)
    {
        _context.Set<Cliente>().Update(cliente);
    }
}
