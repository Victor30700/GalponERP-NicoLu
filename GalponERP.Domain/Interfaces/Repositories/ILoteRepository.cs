using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de Lotes.
/// </summary>
public interface ILoteRepository
{
    Task<Lote?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Lote>> ObtenerActivosAsync();
    void Agregar(Lote lote);
    void Actualizar(Lote lote);
}
