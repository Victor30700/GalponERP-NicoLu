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
        var galponId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;
        var cantidad = 1000;
        var costoPollito = new Moneda(1.50m);

        // Act
        var lote = new Lote(id, "Lote Test", galponId, fecha, cantidad, costoPollito);

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
        Assert.Throws<LoteDomainException>(() => new Lote(id, "Lote Error", Guid.NewGuid(), fecha, 0, costoPollito));
    }

    [Fact]
    public void RegistrarBajas_DebeActualizarCantidadActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Test", Guid.NewGuid(), DateTime.UtcNow, 1000, new Moneda(1.50m));

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
        var lote = new Lote(Guid.NewGuid(), "Lote Test", Guid.NewGuid(), DateTime.UtcNow, 100, new Moneda(1.50m));

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => lote.RegistrarBajas(101));
    }

    [Fact]
    public void RegistrarVenta_DebeActualizarCantidadActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Test", Guid.NewGuid(), DateTime.UtcNow, 1000, new Moneda(1.50m));

        // Act
        lote.RegistrarVenta(500, DateTime.UtcNow);

        // Assert
        Assert.Equal(500, lote.CantidadActual);
        Assert.Equal(500, lote.PollosVendidos);
    }

    [Fact]
    public void RegistrarVenta_DebeLanzarExcepcion_SiLoteEstaCerrado()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Test", Guid.NewGuid(), DateTime.UtcNow, 1000, new Moneda(1.50m));
        lote.CerrarLote(1.60m, new Moneda(1000), new Moneda(500), 5.0m);

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => lote.RegistrarVenta(100, DateTime.UtcNow));
    }

    [Fact]
    public void RegistrarVenta_NoDebePermitirVenta_QueExcedaDisponibilidad()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Test", Guid.NewGuid(), DateTime.UtcNow, 100, new Moneda(1.50m));
        lote.RegistrarBajas(10); // Quedan 90

        // Act & Assert
        Assert.Throws<LoteDomainException>(() => lote.RegistrarVenta(91, DateTime.UtcNow));
    }

    [Fact]
    public void AnularVenta_DebeRestaurarCantidadActual()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Test", Guid.NewGuid(), DateTime.UtcNow, 100, new Moneda(1.50m));
        lote.RegistrarVenta(50, DateTime.UtcNow); // Quedan 50

        // Act
        lote.AnularVenta(20);

        // Assert
        Assert.Equal(70, lote.CantidadActual);
        Assert.Equal(30, lote.PollosVendidos);
    }

    [Fact]
    public void BlindajeSanitario_DebeBloquearVenta_SiEstaEnPeriodoRetiro()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Blindaje", Guid.NewGuid(), DateTime.UtcNow.AddDays(-30), 1000, new Moneda(1.50m));
        
        // Aplicar medicamento con 7 días de retiro hoy
        lote.RegistrarAplicacionMedica(DateTime.UtcNow, 7);

        // Act & Assert
        // Intentar vender hoy mismo
        Assert.Throws<LoteDomainException>(() => lote.RegistrarVenta(100, DateTime.UtcNow));
        
        // Intentar vender en 5 días (todavía bloqueado)
        Assert.Throws<LoteDomainException>(() => lote.RegistrarVenta(100, DateTime.UtcNow.AddDays(5)));
    }

    [Fact]
    public void BlindajeSanitario_DebePermitirVenta_SiTerminoPeriodoRetiro()
    {
        // Arrange
        var lote = new Lote(Guid.NewGuid(), "Lote Blindaje OK", Guid.NewGuid(), DateTime.UtcNow.AddDays(-30), 1000, new Moneda(1.50m));
        
        // Aplicar medicamento con 7 días de retiro hoy
        lote.RegistrarAplicacionMedica(DateTime.UtcNow, 7);

        // Act
        // Intentar vender en 8 días (ya liberado)
        lote.RegistrarVenta(100, DateTime.UtcNow.AddDays(8));

        // Assert
        Assert.Equal(100, lote.PollosVendidos);
        Assert.Equal(900, lote.CantidadActual);
    }
}
