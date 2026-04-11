using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IAuditoriaRepository
{
    void Agregar(AuditoriaLog log);
    Task<IEnumerable<AuditoriaLog>> ObtenerTodosAsync();
}
