using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de Lotes.
/// </summary>
public interface ILoteRepository
{
    Task<Lote?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Lote>> ObtenerActivosAsync();
    Task<IEnumerable<Lote>> ObtenerTodosAsync();
    Task<IEnumerable<Lote>> ObtenerFiltradosAsync(string? busqueda, int? mes, int? anio, bool soloActivos);
    void Agregar(Lote lote);
    void Actualizar(Lote lote);
    void Eliminar(Lote lote);
}
