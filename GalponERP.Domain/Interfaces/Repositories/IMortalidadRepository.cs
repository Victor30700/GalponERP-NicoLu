using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IMortalidadRepository
{
    void Agregar(MortalidadDiaria mortalidad);
    void Actualizar(MortalidadDiaria mortalidad);
    Task<MortalidadDiaria?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<MortalidadDiaria>> ObtenerPorLoteAsync(Guid loteId);
    Task<IEnumerable<MortalidadDiaria>> ObtenerPorRangoFechasAsync(DateTime inicio, DateTime fin);
    Task<IEnumerable<MortalidadDiaria>> ObtenerTodasAsync();
}
