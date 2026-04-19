using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IFormulaRepository
{
    Task<Formula?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Formula>> ObtenerTodasAsync();
    void Agregar(Formula formula);
    void Actualizar(Formula formula);
    void Eliminar(Formula formula);
}
