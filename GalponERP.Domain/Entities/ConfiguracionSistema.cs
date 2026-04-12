using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Configuración global del sistema/tenant para reportes y visualización.
/// Solo debe existir un registro con ID = 1.
/// </summary>
public class ConfiguracionSistema : Entity
{
    public string NombreEmpresa { get; private set; } = null!;
    public string Nit { get; private set; } = null!;
    public string? Telefono { get; private set; }
    public string? Email { get; private set; }
    public string? Direccion { get; private set; }
    public string MonedaPorDefecto { get; private set; } = "USD";
    public string? LogoUrl { get; private set; }

    // Constructor para creación inicial
    public ConfiguracionSistema(Guid id, string nombreEmpresa, string nit, string? telefono = null, string? email = null, string? direccion = null, string monedaPorDefecto = "USD") 
        : base(id)
    {
        Actualizar(nombreEmpresa, nit, telefono, email, direccion, monedaPorDefecto);
    }

    // Constructor para EF Core
    private ConfiguracionSistema() : base() { }

    public void Actualizar(string nombreEmpresa, string nit, string? telefono, string? email, string? direccion, string monedaPorDefecto)
    {
        if (string.IsNullOrWhiteSpace(nombreEmpresa))
            throw new ArgumentException("El nombre de la empresa es requerido.", nameof(nombreEmpresa));

        if (string.IsNullOrWhiteSpace(nit))
            throw new ArgumentException("El NIT es requerido.", nameof(nit));

        NombreEmpresa = nombreEmpresa;
        Nit = nit;
        Telefono = telefono;
        Email = email;
        Direccion = direccion;
        MonedaPorDefecto = monedaPorDefecto;
    }

    public void SetLogo(string logoUrl) => LogoUrl = logoUrl;
}
