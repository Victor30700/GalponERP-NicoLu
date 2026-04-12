using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface ICompraInventarioRepository
{
    Task<CompraInventario?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<CompraInventario>> ObtenerPorProveedorIdAsync(Guid proveedorId);
    Task<IEnumerable<CompraInventario>> ObtenerTodasAsync();
    void Agregar(CompraInventario compra);
    void Actualizar(CompraInventario compra);
}
