using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IClienteRepository
{
    Task<Cliente?> ObtenerPorIdAsync(Guid id);
    Task<Cliente?> ObtenerPorIdIncluyendoInactivosAsync(Guid id);
    Task<IEnumerable<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorRucAsync(string ruc);
    void Agregar(Cliente cliente);
    void Actualizar(Cliente cliente);
}
