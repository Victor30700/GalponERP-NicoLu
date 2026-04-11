using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id);
    Task<Usuario?> ObtenerPorFirebaseUidAsync(string firebaseUid);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ObtenerPorRolAsync(RolGalpon rol);
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    void Agregar(Usuario usuario);
    void Actualizar(Usuario usuario);
}
