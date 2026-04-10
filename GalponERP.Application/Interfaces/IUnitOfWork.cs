namespace GalponERP.Application.Interfaces;

/// <summary>
/// Interfaz para manejar la persistencia atómica de los cambios.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
