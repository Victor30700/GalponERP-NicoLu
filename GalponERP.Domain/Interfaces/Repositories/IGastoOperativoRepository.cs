using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de Gastos Operativos.
/// </summary>
public interface IGastoOperativoRepository
{
    Task<GastoOperativo?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<GastoOperativo>> ObtenerPorGalponAsync(Guid galponId);
    Task<IEnumerable<GastoOperativo>> ObtenerPorLoteAsync(Guid loteId);
    void Agregar(GastoOperativo gasto);
    void Actualizar(GastoOperativo gasto);
}
