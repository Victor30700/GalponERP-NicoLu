using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IOrdenCompraRepository
{
    Task<OrdenCompra?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<OrdenCompra>> ObtenerPendientesAsync();
    void Agregar(OrdenCompra ordenCompra);
    void Actualizar(OrdenCompra ordenCompra);
}
