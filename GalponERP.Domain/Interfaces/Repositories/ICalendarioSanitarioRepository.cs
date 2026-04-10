using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de Calendario Sanitario.
/// </summary>
public interface ICalendarioSanitarioRepository
{
    Task<CalendarioSanitario?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<CalendarioSanitario>> ObtenerPorLoteIdAsync(Guid loteId);
    void Agregar(CalendarioSanitario calendario);
    void Actualizar(CalendarioSanitario calendario);
}
