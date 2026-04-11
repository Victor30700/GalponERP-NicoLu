using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Unidad de medida para productos (Kg, Unidad, Litro, etc.)
/// </summary>
public class UnidadMedida : Entity
{
    public string Nombre { get; private set; } = null!;
    public string Abreviatura { get; private set; } = null!;

    public UnidadMedida(Guid id, string nombre, string abreviatura) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la unidad de medida es obligatorio.");
        
        if (string.IsNullOrWhiteSpace(abreviatura))
            throw new ArgumentException("La abreviatura es obligatoria.");

        Nombre = nombre;
        Abreviatura = abreviatura;
    }

    // Para EF Core
    private UnidadMedida() : base() { }

    public void Actualizar(string nombre, string abreviatura)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la unidad de medida no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(abreviatura))
            throw new ArgumentException("La abreviatura no puede estar vacía.");

        Nombre = nombre;
        Abreviatura = abreviatura;
    }
}
