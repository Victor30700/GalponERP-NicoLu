using GalponERP.Domain.Primitives;
using GalponERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Application.Interfaces;

public interface IGalponDbContext
{
    DbSet<Conversacion> Conversaciones { get; }
    DbSet<IntencionPendiente> IntencionesPendientes { get; }
    Task<T?> ObtenerEntidadPorIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : Entity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
