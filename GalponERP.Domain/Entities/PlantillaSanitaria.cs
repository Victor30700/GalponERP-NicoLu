using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa una plantilla de plan sanitario (vacunas, vitaminas, etc.) que se puede aplicar a un lote.
/// </summary>
public class PlantillaSanitaria : Entity
{
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }

    private readonly List<ActividadPlantilla> _actividades = new();
    public IReadOnlyCollection<ActividadPlantilla> Actividades => _actividades.AsReadOnly();

    public PlantillaSanitaria(Guid id, string nombre, string? descripcion = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la plantilla es obligatorio.");

        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void AgregarActividad(Guid id, TipoActividad tipo, int dia, string descripcion, Guid? productoId = null)
    {
        if (dia <= 0)
            throw new ArgumentException("El día de aplicación debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción de la actividad es obligatoria.");

        var actividad = new ActividadPlantilla(id, Id, tipo, dia, descripcion, productoId);
        _actividades.Add(actividad);
    }

    public void LimpiarActividades() => _actividades.Clear();

    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la plantilla es obligatorio.");

        Nombre = nombre;
        Descripcion = descripcion;
    }

    // Constructor para EF Core
    private PlantillaSanitaria() : base() { }
}

/// <summary>
/// Representa una actividad específica dentro de una plantilla sanitaria.
/// </summary>
public class ActividadPlantilla : Entity
{
    public Guid PlantillaId { get; private set; }
    public TipoActividad TipoActividad { get; private set; }
    public int DiaDeAplicacion { get; private set; }
    public string Descripcion { get; private set; } = null!;
    public Guid? ProductoIdRecomendado { get; private set; }

    public ActividadPlantilla(Guid id, Guid plantillaId, TipoActividad tipo, int dia, string descripcion, Guid? productoId = null) : base(id)
    {
        PlantillaId = plantillaId;
        TipoActividad = tipo;
        DiaDeAplicacion = dia;
        Descripcion = descripcion;
        ProductoIdRecomendado = productoId;
    }

    // Constructor para EF Core
    private ActividadPlantilla() : base() { }
}
