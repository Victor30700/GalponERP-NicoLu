using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IVentaRepository
{
    Task<Venta?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Venta>> ObtenerPorLoteAsync(Guid loteId);
    Task<IEnumerable<Venta>> ObtenerPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Venta>> ObtenerPorRangoFechasAsync(DateTime inicio, DateTime fin);
    Task<IEnumerable<Venta>> ObtenerTodasAsync();
    void Agregar(Venta venta);
    void Actualizar(Venta venta);
}
