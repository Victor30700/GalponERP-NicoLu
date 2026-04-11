using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IPesajeLoteRepository
{
    Task<PesajeLote?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<PesajeLote>> ObtenerPorLoteIdAsync(Guid loteId);
    void Agregar(PesajeLote pesaje);
    void Actualizar(PesajeLote pesaje);
    void Eliminar(PesajeLote pesaje);
}
