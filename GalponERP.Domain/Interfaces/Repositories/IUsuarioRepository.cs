using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id);
    Task<Usuario?> ObtenerPorFirebaseUidAsync(string firebaseUid);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<Usuario?> ObtenerPorTelefonoAsync(string telefono);
    Task<Usuario?> ObtenerPorWhatsAppAsync(string whatsAppNumero);
    Task<Usuario?> ObtenerPorCodigoVinculacionAsync(string codigo);
    Task<IEnumerable<Usuario>> ObtenerPorRolAsync(RolGalpon rol);
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<IEnumerable<Usuario>> ObtenerTodosConInactivosAsync();
    void Agregar(Usuario usuario);
    void Actualizar(Usuario usuario);
}
