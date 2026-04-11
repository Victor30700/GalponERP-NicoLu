using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IUnidadMedidaRepository
{
    Task<UnidadMedida?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<UnidadMedida>> ObtenerTodasAsync();
    void Agregar(UnidadMedida unidadMedida);
    void Actualizar(UnidadMedida unidadMedida);
    void Eliminar(UnidadMedida unidadMedida);
}
