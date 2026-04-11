using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Entidad que representa un usuario del sistema autenticado por Firebase y enriquecido con perfil de Recursos Humanos.
/// </summary>
public class Usuario : Entity
{
    public string FirebaseUid { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string Apellidos { get; private set; } = string.Empty;
    public DateTime FechaNacimiento { get; private set; }
    public string Direccion { get; private set; } = string.Empty;
    public string Profesion { get; private set; } = string.Empty;
    public string Rol { get; private set; } = null!;

    public Usuario(Guid id, string firebaseUid, string email, string nombre, string apellidos, DateTime fechaNacimiento, string direccion, string profesion, string rol) : base(id)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
            throw new ArgumentException("El FirebaseUid es obligatorio.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(rol))
            throw new ArgumentException("El rol es obligatorio.");

        FirebaseUid = firebaseUid;
        Email = email;
        Nombre = nombre;
        Apellidos = apellidos;
        FechaNacimiento = fechaNacimiento;
        Direccion = direccion;
        Profesion = profesion;
        Rol = rol;
    }

    // Constructor para EF Core
    private Usuario() : base() { }

    public void ActualizarPerfil(string email, string nombre, string apellidos, DateTime fechaNacimiento, string direccion, string profesion, string rol)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(rol))
            throw new ArgumentException("El rol es obligatorio.");

        Email = email;
        Nombre = nombre;
        Apellidos = apellidos;
        FechaNacimiento = fechaNacimiento;
        Direccion = direccion;
        Profesion = profesion;
        Rol = rol;
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nuevo nombre no puede estar vacío.");
        Nombre = nuevoNombre;
    }

    public void ActualizarRol(string nuevoRol)
    {
        if (string.IsNullOrWhiteSpace(nuevoRol))
            throw new ArgumentException("El rol es obligatorio.");
        Rol = nuevoRol;
    }
}
