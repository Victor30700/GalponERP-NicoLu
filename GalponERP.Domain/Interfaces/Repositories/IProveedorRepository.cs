using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IProveedorRepository
{
    Task<Proveedor?> ObtenerPorIdAsync(Guid id);
    Task<Proveedor?> ObtenerPorIdIncluyendoInactivosAsync(Guid id);
    Task<IEnumerable<Proveedor>> ObtenerTodosAsync();
    void Agregar(Proveedor proveedor);
    void Actualizar(Proveedor proveedor);
}
