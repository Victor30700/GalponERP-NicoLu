using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id);
    Task<Usuario?> ObtenerPorFirebaseUidAsync(string firebaseUid);
    Task<IEnumerable<Usuario>> ObtenerPorRolAsync(string rol);
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    void Agregar(Usuario usuario);
}
