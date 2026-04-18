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
    public string? Telefono { get; private set; }
    public string? WhatsAppNumero { get; private set; }
    public string? CodigoVinculacion { get; private set; }
    public DateTime? FechaExpiracionCodigo { get; private set; }
    public RolGalpon Rol { get; private set; }
    public int Active { get; private set; }

    public Usuario(Guid id, string firebaseUid, string email, string nombre, string apellidos, DateTime fechaNacimiento, string direccion, string profesion, string? telefono, RolGalpon rol, int active = 1) : base(id)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
            throw new ArgumentException("El FirebaseUid es obligatorio.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        FirebaseUid = firebaseUid;
        Email = email;
        Nombre = nombre;
        Apellidos = apellidos;
        FechaNacimiento = fechaNacimiento;
        Direccion = direccion;
        Profesion = profesion;
        Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono;
        Rol = rol;
        Active = active;
        IsActive = active == 1;
    }

    public void GenerarCodigoVinculacion()
    {
        var random = new Random();
        CodigoVinculacion = random.Next(100000, 999999).ToString();
        FechaExpiracionCodigo = DateTime.UtcNow.AddMinutes(15);
    }

    public bool ValidarCodigoVinculacion(string codigo)
    {
        return CodigoVinculacion == codigo && FechaExpiracionCodigo > DateTime.UtcNow;
    }

    public void VincularWhatsApp(string numero)
    {
        WhatsAppNumero = string.IsNullOrWhiteSpace(numero) ? null : numero;
        CodigoVinculacion = null;
        FechaExpiracionCodigo = null;
    }

    public void DesvincularWhatsApp()
    {
        WhatsAppNumero = null;
    }

    // Constructor para EF Core
    private Usuario() : base() { }

    public void ActualizarPerfil(string email, string nombre, string apellidos, DateTime fechaNacimiento, string direccion, string profesion, string? telefono, RolGalpon rol)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        Email = email;
        Nombre = nombre;
        Apellidos = apellidos;
        FechaNacimiento = fechaNacimiento;
        Direccion = direccion;
        Profesion = profesion;
        Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono;
        Rol = rol;
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nuevo nombre no puede estar vacío.");
        Nombre = nuevoNombre;
    }

    public void ActualizarRol(RolGalpon nuevoRol)
    {
        Rol = nuevoRol;
    }

    public void ActualizarEstado(int active)
    {
        Active = active;
        IsActive = active == 1;
    }
}
