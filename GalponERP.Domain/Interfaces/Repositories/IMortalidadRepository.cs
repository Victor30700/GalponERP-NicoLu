using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IMortalidadRepository
{
    void Agregar(MortalidadDiaria mortalidad);
    Task<IEnumerable<MortalidadDiaria>> ObtenerPorLoteAsync(Guid loteId);
    Task<IEnumerable<MortalidadDiaria>> ObtenerPorRangoFechasAsync(DateTime inicio, DateTime fin);
    Task<IEnumerable<MortalidadDiaria>> ObtenerTodasAsync();
}
