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
    public Guid GalponId { get; private set; }
    public DateTime FechaIngreso { get; private set; }
    public int CantidadInicial { get; private set; }
    public int CantidadActual { get; private set; }
    public int MortalidadAcumulada { get; private set; }
    public int PollosVendidos { get; private set; }
    public Moneda CostoUnitarioPollito { get; private set; } = null!;
    public EstadoLote Estado { get; private set; }
    public string? JustificacionCancelacion { get; private set; }

    // Snapshots Contables
    public decimal? FCRFinal { get; private set; }
    public Moneda? CostoTotalFinal { get; private set; }
    public Moneda? UtilidadNetaFinal { get; private set; }
    public decimal? PorcentajeMortalidadFinal { get; private set; }

    // Propiedades de navegación
    private readonly List<PesajeLote> _pesajes = new();
    public IReadOnlyCollection<PesajeLote> Pesajes => _pesajes.AsReadOnly();

    public Lote(Guid id, Guid galponId, DateTime fechaIngreso, int cantidadInicial, Moneda costoUnitarioPollito) 
        : base(id)
    {
        if (galponId == Guid.Empty)
            throw new LoteDomainException("El ID del galpón es obligatorio.");

        if (cantidadInicial <= 0)
            throw new LoteDomainException("La cantidad inicial del lote debe ser mayor a cero.");

        GalponId = galponId;
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

    public void Cancelar(string justificacion)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden cancelar lotes activos.");

        if (string.IsNullOrWhiteSpace(justificacion))
            throw new LoteDomainException("La justificación es obligatoria para cancelar un lote.");

        Estado = EstadoLote.Cancelado;
        JustificacionCancelacion = justificacion;
    }

    public void Trasladar(Guid nuevoGalponId)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden trasladar lotes activos.");

        if (nuevoGalponId == Guid.Empty)
            throw new LoteDomainException("El ID del nuevo galpón es obligatorio.");

        if (nuevoGalponId == GalponId)
            throw new LoteDomainException("El lote ya se encuentra en ese galpón.");

        GalponId = nuevoGalponId;
    }

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
    /// Corrige un registro de mortalidad previo.
    /// </summary>
    public void CorregirMortalidad(int cantidadAnterior, int cantidadNueva)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden corregir bajas en lotes activos.");

        if (cantidadNueva <= 0)
            throw new LoteDomainException("La nueva cantidad de bajas debe ser mayor a cero.");

        // Revertir el impacto anterior
        MortalidadAcumulada -= cantidadAnterior;
        CantidadActual += cantidadAnterior;

        // Aplicar el nuevo impacto
        if (cantidadNueva > CantidadActual)
            throw new LoteDomainException($"No se pueden registrar {cantidadNueva} bajas. Solo quedan {CantidadActual} pollos vivos.");

        MortalidadAcumulada += cantidadNueva;
        CantidadActual -= cantidadNueva;
    }

    /// <summary>
    /// Elimina un registro de mortalidad (Soft Delete a nivel de registro, pero revierte contadores).
    /// </summary>
    public void EliminarMortalidad(int cantidad)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden eliminar bajas en lotes activos.");

        MortalidadAcumulada -= cantidad;
        CantidadActual += cantidad;
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

    public void CorregirVenta(int cantidadAnterior, int cantidadNueva)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden corregir ventas en lotes activos.");

        if (cantidadNueva <= 0)
            throw new LoteDomainException("La nueva cantidad vendida debe ser mayor a cero.");

        // Revertir el impacto anterior
        PollosVendidos -= cantidadAnterior;
        CantidadActual += cantidadAnterior;

        // Aplicar el nuevo impacto
        int disponibleParaVenta = CantidadInicial - MortalidadAcumulada - PollosVendidos;
        
        if (cantidadNueva > disponibleParaVenta)
            throw new LoteDomainException($"Intento de corregir venta a {cantidadNueva} pollos, pero solo quedan {disponibleParaVenta} disponibles.");

        PollosVendidos += cantidadNueva;
        CantidadActual -= cantidadNueva;
    }

    public void CerrarLote(decimal fcr, Moneda costoTotal, Moneda utilidadNeta, decimal porcentajeMortalidad)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("El lote ya no está activo.");

        FCRFinal = fcr;
        CostoTotalFinal = costoTotal;
        UtilidadNetaFinal = utilidadNeta;
        PorcentajeMortalidadFinal = porcentajeMortalidad;
        Estado = EstadoLote.Cerrado;
    }

    public void Reabrir()
    {
        if (Estado != EstadoLote.Cerrado)
            throw new LoteDomainException("Solo se pueden reabrir lotes que estén cerrados.");

        FCRFinal = null;
        CostoTotalFinal = null;
        UtilidadNetaFinal = null;
        PorcentajeMortalidadFinal = null;
        Estado = EstadoLote.Activo;
    }

    public void AnularVenta(int cantidad)
    {
        if (Estado == EstadoLote.Cerrado)
            throw new LoteDomainException("No se pueden anular ventas de un lote ya cerrado.");

        if (cantidad <= 0)
            throw new LoteDomainException("La cantidad a anular debe ser positiva.");

        if (cantidad > PollosVendidos)
            throw new LoteDomainException("No se puede anular más de lo vendido.");

        PollosVendidos -= cantidad;
        CantidadActual += cantidad;
    }

    public void ActualizarDatosIniciales(Guid galponId, DateTime fechaIngreso, int cantidadInicial, Moneda costoUnitario)
    {
        if (Estado != EstadoLote.Activo)
            throw new LoteDomainException("Solo se pueden actualizar datos iniciales en lotes activos.");

        if (galponId == Guid.Empty)
            throw new LoteDomainException("El ID del galpón es obligatorio.");

        if (cantidadInicial <= 0)
            throw new LoteDomainException("La cantidad inicial debe ser mayor a cero.");

        // Recalcular CantidadActual basada en la nueva cantidad inicial y el rastro histórico
        int nuevaCantidadActual = cantidadInicial - MortalidadAcumulada - PollosVendidos;
        
        if (nuevaCantidadActual < 0)
            throw new LoteDomainException("La nueva cantidad inicial es insuficiente para cubrir la mortalidad y ventas ya registradas.");

        GalponId = galponId;
        FechaIngreso = fechaIngreso;
        CantidadInicial = cantidadInicial;
        CantidadActual = nuevaCantidadActual;
        CostoUnitarioPollito = costoUnitario;
    }
}
