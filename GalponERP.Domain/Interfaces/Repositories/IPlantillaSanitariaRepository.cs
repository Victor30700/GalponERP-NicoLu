using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IPlantillaSanitariaRepository
{
    Task<PlantillaSanitaria?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<PlantillaSanitaria>> ObtenerTodasAsync();
    void Agregar(PlantillaSanitaria plantilla);
    void Actualizar(PlantillaSanitaria plantilla);
}
