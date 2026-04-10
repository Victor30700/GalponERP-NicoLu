using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IGalponRepository
{
    Task<Galpon?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Galpon>> ObtenerTodosAsync();
    void Agregar(Galpon galpon);
    void Actualizar(Galpon galpon);
}
