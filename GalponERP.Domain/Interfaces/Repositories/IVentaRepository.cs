using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IVentaRepository
{
    Task<Venta?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Venta>> ObtenerPorLoteAsync(Guid loteId);
    void Agregar(Venta venta);
}
