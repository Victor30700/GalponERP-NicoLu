using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa un cliente que compra pollos.
/// </summary>
public class Cliente : Entity
{
    public string Nombre { get; private set; } = null!;
    public string Ruc { get; private set; } = null!;
    public string? Direccion { get; private set; }
    public string? Telefono { get; private set; }

    public Cliente(Guid id, string nombre, string ruc, string? direccion = null, string? telefono = null) 
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del cliente es requerido.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(ruc))
            throw new ArgumentException("El RUC del cliente es requerido.", nameof(ruc));

        Nombre = nombre;
        Ruc = ruc;
        Direccion = direccion;
        Telefono = telefono;
    }

    // Constructor para EF Core
    private Cliente() : base() { }

    public void Actualizar(string nombre, string ruc, string? direccion, string? telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del cliente no puede ser vacío.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(ruc))
            throw new ArgumentException("El RUC del cliente no puede ser vacío.", nameof(ruc));

        Nombre = nombre;
        Ruc = ruc;
        Direccion = direccion;
        Telefono = telefono;
    }
}
