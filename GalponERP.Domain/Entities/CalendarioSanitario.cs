using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

public enum EstadoCalendario
{
    Pendiente,
    Aplicado,
    Cancelado
}

/// <summary>
/// Representa una actividad o tratamiento sanitario programado para un lote.
/// </summary>
public class CalendarioSanitario : Entity
{
    public Guid LoteId { get; private set; }
    public int DiaDeAplicacion { get; private set; }
    public string DescripcionTratamiento { get; private set; } = null!;
    public Guid? ProductoIdRecomendado { get; private set; }
    public decimal CantidadRecomendada { get; private set; }
    public EstadoCalendario Estado { get; private set; }
    public TipoActividad Tipo { get; private set; }
    public bool EsManual { get; private set; }
    public string? Justificacion { get; private set; }

    public CalendarioSanitario(
        Guid id, 
        Guid loteId, 
        int diaDeAplicacion, 
        string descripcionTratamiento, 
        TipoActividad tipo = TipoActividad.Otros,
        Guid? productoIdRecomendado = null,
        decimal cantidadRecomendada = 0,
        bool esManual = false,
        string? justificacion = null) 
        : base(id)
    {
        if (loteId == Guid.Empty)
            throw new CalendarioDomainException("El LoteId no puede ser vacío.");

        if (diaDeAplicacion <= 0)
            throw new CalendarioDomainException("El día de aplicación debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(descripcionTratamiento))
            throw new CalendarioDomainException("La descripción del tratamiento es requerida.");

        LoteId = loteId;
        DiaDeAplicacion = diaDeAplicacion;
        DescripcionTratamiento = descripcionTratamiento;
        ProductoIdRecomendado = productoIdRecomendado;
        CantidadRecomendada = cantidadRecomendada;
        Estado = EstadoCalendario.Pendiente;
        Tipo = tipo;
        EsManual = esManual;
        Justificacion = justificacion;
    }

    // Constructor para EF Core
    private CalendarioSanitario() : base() { }

    public void MarcarComoAplicado()
    {
        if (Estado == EstadoCalendario.Aplicado)
            throw new CalendarioDomainException("El tratamiento ya ha sido aplicado.");

        if (Estado == EstadoCalendario.Cancelado)
            throw new CalendarioDomainException("No se puede aplicar un tratamiento cancelado.");

        Estado = EstadoCalendario.Aplicado;
    }

    public void Reprogramar(int nuevoDia, string justificacion)
    {
        if (Estado != EstadoCalendario.Pendiente)
            throw new CalendarioDomainException("Solo se pueden reprogramar actividades pendientes.");

        if (nuevoDia <= 0)
            throw new CalendarioDomainException("El nuevo día de aplicación debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(justificacion))
            throw new CalendarioDomainException("Se requiere una justificación para reprogramar una actividad.");

        DiaDeAplicacion = nuevoDia;
        Justificacion = justificacion;
    }

    public void Cancelar(string justificacion)
    {
        if (Estado != EstadoCalendario.Pendiente)
            throw new CalendarioDomainException("Solo se pueden cancelar actividades pendientes.");

        if (string.IsNullOrWhiteSpace(justificacion))
            throw new CalendarioDomainException("Se requiere una justificación para cancelar una actividad.");

        Estado = EstadoCalendario.Cancelado;
        Justificacion = justificacion;
    }
}
