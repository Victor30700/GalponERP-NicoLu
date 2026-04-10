using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Producto>> ObtenerPorTipoAsync(TipoProducto tipo);
    Task<IEnumerable<Producto>> ObtenerTodosAsync();
}
