using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Entidad que representa un usuario del sistema autenticado por Firebase.
/// </summary>
public class Usuario : Entity
{
    public string FirebaseUid { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string Rol { get; private set; } = null!;

    public Usuario(Guid id, string firebaseUid, string nombre, string rol) : base(id)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
            throw new ArgumentException("El FirebaseUid es obligatorio.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(rol))
            throw new ArgumentException("El rol es obligatorio.");

        FirebaseUid = firebaseUid;
        Nombre = nombre;
        Rol = rol;
    }

    // Constructor para EF Core
    private Usuario() : base() { }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nuevo nombre no puede estar vacío.");
        Nombre = nuevoNombre;
    }
}
