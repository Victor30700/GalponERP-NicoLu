using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IMortalidadRepository
{
    void Agregar(MortalidadDiaria mortalidad);
    Task<IEnumerable<MortalidadDiaria>> ObtenerPorLoteAsync(Guid loteId);
}
