using GalponERP.Domain.Primitives;

namespace GalponERP.Application.Interfaces;

public interface IGalponDbContext
{
    Task<T?> ObtenerEntidadPorIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : Entity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
