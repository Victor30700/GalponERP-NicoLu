using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface ICategoriaProductoRepository
{
    Task<CategoriaProducto?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<CategoriaProducto>> ObtenerTodasAsync();
    void Agregar(CategoriaProducto categoria);
    void Actualizar(CategoriaProducto categoria);
    void Eliminar(CategoriaProducto categoria);
}
