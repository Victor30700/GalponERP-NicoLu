using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

public enum EstadoLote
{
    Activo,
    Cerrado,
    Cancelado
}

/// <summary>
/// Entidad que representa un lote de pollos de engorde.
/// El corazón del seguimiento de mortalidad y producción.
/// </summary>
public class Lote : Entity
{
    public DateTime FechaIngreso { get; private set; }
    public int CantidadInicial { get; private set; }
    public int CantidadActual { get; private set; }
    public int MortalidadAcumulada { get; private set; }
    public int PollosVendidos { get; private set; }
    public Moneda CostoUnitarioPollito { get; private set; } = null!;
    public EstadoLote Estado { get; private set; }

    public Lote(Guid id, DateTime fechaIngreso, int cantidadInicial, Moneda costoUnitarioPollito) 
        : base(id)
    {
        if (cantidadInicial <= 0)
            throw new LoteDomainException("La cantidad inicial del lote debe ser mayor a cero.");

        FechaIngreso = fechaIngreso;
        CantidadInicial = cantidadInicial;
        CantidadActual = cantidadInicial;
        MortalidadAcumulada = 0;
        PollosVendidos = 0;
        CostoUnitarioPollito = costoUnitarioPollito;
        Estado = EstadoLote.Activo;
    }

    // Constructor para EF Core
    private Lote() : base() { }

    /// <summary>
    /// Registra la mortalidad en el lote.
    /// Actualiza la cantidad actual de pollos vivos.
    /// </summary>
    /// <param name="cantidad">Cantidad de bajas a registrar.</param>
    public void RegistrarBajas(int cantidad)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden registrar bajas en lotes activos.");

        if (cantidad <= 0)
            throw new LoteDomainException("La cantidad de bajas debe ser un número positivo.");

        if (cantidad > CantidadActual)
            throw new LoteDomainException($"No se pueden registrar {cantidad} bajas. Solo quedan {CantidadActual} pollos vivos.");

        MortalidadAcumulada += cantidad;
        CantidadActual -= cantidad;
    }

    /// <summary>
    /// Registra la venta de pollos del lote.
    /// </summary>
    /// <param name="cantidad">Cantidad de pollos a vender.</param>
    public void RegistrarVenta(int cantidad)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden registrar ventas en lotes activos.");

        if (cantidad <= 0)
            throw new LoteDomainException("La cantidad a vender debe ser un número positivo.");

        // Validación crítica de negocio: (CantidadInicial - MortalidadAcumulada - PollosVendidos)
        int disponibleParaVenta = CantidadInicial - MortalidadAcumulada - PollosVendidos;
        
        if (cantidad > disponibleParaVenta)
            throw new LoteDomainException($"Intento de vender {cantidad} pollos, pero solo quedan {disponibleParaVenta} disponibles.");

        PollosVendidos += cantidad;
        CantidadActual -= cantidad;
    }

    public void CerrarLote()
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("El lote ya no está activo.");

        Estado = EstadoLote.Cerrado;
    }
}
