using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa un proveedor que suministra insumos y productos.
/// </summary>
public class Proveedor : Entity
{
    public string RazonSocial { get; private set; } = null!;
    public string NitRuc { get; private set; } = null!;
    public string? Telefono { get; private set; }
    public string? Email { get; private set; }
    public string? Direccion { get; private set; }

    public Proveedor(Guid id, string razonSocial, string nitRuc, string? telefono = null, string? email = null, string? direccion = null) 
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ArgumentException("La razón social del proveedor es requerida.", nameof(razonSocial));

        if (string.IsNullOrWhiteSpace(nitRuc))
            throw new ArgumentException("El NIT/RUC del proveedor es requerido.", nameof(nitRuc));

        RazonSocial = razonSocial;
        NitRuc = nitRuc;
        Telefono = telefono;
        Email = email;
        Direccion = direccion;
    }

    // Constructor para EF Core
    private Proveedor() : base() { }

    public void Actualizar(string razonSocial, string nitRuc, string? telefono, string? email, string? direccion)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ArgumentException("La razón social no puede ser vacía.", nameof(razonSocial));

        if (string.IsNullOrWhiteSpace(nitRuc))
            throw new ArgumentException("El NIT/RUC no puede ser vacío.", nameof(nitRuc));

        RazonSocial = razonSocial;
        NitRuc = nitRuc;
        Telefono = telefono;
        Email = email;
        Direccion = direccion;
    }
}
