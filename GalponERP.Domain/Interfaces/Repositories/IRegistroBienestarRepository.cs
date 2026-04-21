using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IRegistroBienestarRepository
{
    Task<RegistroBienestar?> ObtenerPorLoteYFechaAsync(Guid loteId, DateTime fecha);
    Task<IEnumerable<RegistroBienestar>> ObtenerPorLoteAsync(Guid loteId);
    Task<IEnumerable<RegistroBienestar>> ObtenerHistorialPorLoteAsync(Guid loteId);
    void Agregar(RegistroBienestar registro);
    void Actualizar(RegistroBienestar registro);
}
