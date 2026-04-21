using GalponERP.Application.Interfaces;

namespace GalponERP.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly GalponDbContext _context;

    public UnitOfWork(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
