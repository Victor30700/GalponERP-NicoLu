using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IClienteRepository
{
    Task<Cliente?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Cliente>> ObtenerTodosAsync();
    void Agregar(Cliente cliente);
    void Actualizar(Cliente cliente);
}
