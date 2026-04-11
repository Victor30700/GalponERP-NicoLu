using GalponERP.Domain.Entities;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Tests;

public class LoteTests
{
    [Fact]
    public void Lote_DebeInicializarseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fecha = DateTime.UtcNow;
        var cantidad = 1000;
        var costoPollito = new Moneda(1.50m);

        // Act
        var lote = new Lote(id, fecha, cantidad, costoPollito);

        // Assert
        Assert.Equal(id, lote.Id);
        Assert.Equal(fecha, lote.FechaIngreso);
        Assert.Equal(cantidad, lote.CantidadInicial);
        Assert.Equal(cantidad, lote.CantidadActual);
        Assert.Equal(EstadoLote.Activo, lote.Estado);
    }

    [Fact]
    public void Lote_DebeLanzarExcepcion_SiCantidadEsCero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fecha = DateTime.UtcNow;
        var costoPollito = new Moneda(1.50m);

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => new Lote(id, fecha, 0, costoPollito));
    }

    [Fact]
    public void RegistrarBajas_DebeActualizarCantidadActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), DateTime.UtcNow, 1000, new Moneda(1.50m));

        // Act
        lote.RegistrarBajas(100);

        // Assert
        Assert.Equal(900, lote.CantidadActual);
        Assert.Equal(100, lote.MortalidadAcumulada);
    }

    [Fact]
    public void RegistrarBajas_DebeLanzarExcepcion_SiCantidadExcedeActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), DateTime.UtcNow, 100, new Moneda(1.50m));

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => lote.RegistrarBajas(101));
    }

    [Fact]
    public void RegistrarVenta_DebeActualizarCantidadActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), DateTime.UtcNow, 1000, new Moneda(1.50m));

        // Act
        lote.RegistrarVenta(500);

        // Assert
        Assert.Equal(500, lote.CantidadActual);
        Assert.Equal(500, lote.PollosVendidos);
    }

    [Fact]
    public void RegistrarVenta_DebeLanzarExcepcion_SiLoteEstaCerrado()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), DateTime.UtcNow, 1000, new Moneda(1.50m));
        lote.CerrarLote(1.60m, new Moneda(1000), new Moneda(500), 5.0m);

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => lote.RegistrarVenta(100));
    }

    [Fact]
    public void RegistrarVenta_NoDebePermitirVenta_QueExcedaDisponibilidad()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), DateTime.UtcNow, 100, new Moneda(1.50m));
        lote.RegistrarBajas(10); // Quedan 90

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => lote.RegistrarVenta(91));
    }

    [Fact]
    public void AnularVenta_DebeRestaurarCantidadActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), DateTime.UtcNow, 100, new Moneda(1.50m));
        lote.RegistrarVenta(50); // Quedan 50

        // Act
        lote.AnularVenta(20);

        // Assert
        Assert.Equal(70, lote.CantidadActual);
        Assert.Equal(30, lote.PollosVendidos);
    }
}
