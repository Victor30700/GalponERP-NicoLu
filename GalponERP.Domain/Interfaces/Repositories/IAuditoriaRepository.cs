using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IAuditoriaRepository
{
    void Agregar(AuditoriaLog log);
    Task<IEnumerable<AuditoriaLog>> ObtenerTodosAsync();
    Task<IEnumerable<AuditoriaLog>> ObtenerFiltradosAsync(DateTime? desde, DateTime? hasta, Guid? usuarioId, string? entidad);
}
