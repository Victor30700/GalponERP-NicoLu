using GalponERP.Domain.Entities;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Tests;

public class VentaPagosTests
{
    private Venta CrearVentaBase(decimal totalMonto)
    {
        var loteId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var precioPorKilo = new Moneda(10);
        var pesoTotal = totalMonto / 10; // 100 / 10 = 10kg

        return new Venta(
            Guid.NewGuid(),
            loteId,
            clienteId,
            DateTime.UtcNow,
            50,
            pesoTotal,
            precioPorKilo,
            usuarioId
        );
    }

    [Fact]
    public void RegistrarPago_DebeActualizarSaldoYEstado_CuandoEsPagoParcial()
    {
        // Arrange
        var venta = CrearVentaBase(100); // Total 100

        // Act
        venta.RegistrarPago(Guid.NewGuid(), new Moneda(40), DateTime.UtcNow, MetodoPago.Efectivo, Guid.NewGuid());

        // Assert
        Assert.Equal(60, venta.SaldoPendiente.Monto);
        Assert.Equal(EstadoPago.Parcial, venta.EstadoPago);
    }

    [Fact]
    public void RegistrarPago_DebeActualizarEstadoAPagado_CuandoSeCompletaElTotal()
    {
        // Arrange
        var venta = CrearVentaBase(100);

        // Act
        venta.RegistrarPago(Guid.NewGuid(), new Moneda(100), DateTime.UtcNow, MetodoPago.Transferencia, Guid.NewGuid());

        // Assert
        Assert.Equal(0, venta.SaldoPendiente.Monto);
        Assert.Equal(EstadoPago.Pagado, venta.EstadoPago);
    }

    [Fact]
    public void RegistrarPago_DebeLanzarExcepcion_SiElMontoExcedeElSaldo()
    {
        // Arrange
        var venta = CrearVentaBase(100);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            venta.RegistrarPago(Guid.NewGuid(), new Moneda(101), DateTime.UtcNow, MetodoPago.Efectivo, Guid.NewGuid()));
        
        Assert.Contains("no puede exceder el saldo pendiente", ex.Message);
    }

    [Fact]
    public void AnularPago_DebeRestaurarSaldoYEstado()
    {
        // Arrange
        var venta = CrearVentaBase(100);
        var pagoId = Guid.NewGuid();
        venta.RegistrarPago(pagoId, new Moneda(100), DateTime.UtcNow, MetodoPago.Efectivo, Guid.NewGuid());
        Assert.Equal(EstadoPago.Pagado, venta.EstadoPago);

        // Act
        venta.AnularPago(pagoId, Guid.NewGuid());

        // Assert
        Assert.Equal(100, venta.SaldoPendiente.Monto);
        Assert.Equal(EstadoPago.Pendiente, venta.EstadoPago);
    }

    [Fact]
    public void Venta_DebeCalcularSaldoCorrectamente_ConMultiplesPagos()
    {
        // Arrange
        var venta = CrearVentaBase(100);

        // Act
        venta.RegistrarPago(Guid.NewGuid(), new Moneda(30), DateTime.UtcNow, MetodoPago.Efectivo, Guid.NewGuid());
        venta.RegistrarPago(Guid.NewGuid(), new Moneda(20), DateTime.UtcNow, MetodoPago.Transferencia, Guid.NewGuid());

        // Assert
        Assert.Equal(50, venta.SaldoPendiente.Monto);
        Assert.Equal(EstadoPago.Parcial, venta.EstadoPago);
    }
}
