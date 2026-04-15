using GalponERP.Domain.Entities;
using Xunit;

namespace GalponERP.Tests;

public class ProductoTests
{
    [Fact]
    public void ActualizarStock_Entrada_DebeAumentarStockActualKg()
    {
        // Arrange
        var producto = new Producto(
            Guid.NewGuid(),
            "Alimento",
            Guid.NewGuid(),
            Guid.NewGuid(),
            50m, // 50kg por unidad
            100m);

        // Act
        producto.ActualizarStock(10, TipoMovimiento.Entrada); // 10 unidades * 50kg = 500kg

        // Assert
        Assert.Equal(500m, producto.StockActualKg);
    }

    [Fact]
    public void ActualizarStock_Salida_DebeDisminuirStockActualKg()
    {
        // Arrange
        var producto = new Producto(
            Guid.NewGuid(),
            "Alimento",
            Guid.NewGuid(),
            Guid.NewGuid(),
            50m,
            100m,
            0,
            1000m); // Inicia con 1000kg

        // Act
        producto.ActualizarStock(5, TipoMovimiento.Salida); // 5 unidades * 50kg = 250kg

        // Assert
        Assert.Equal(750m, producto.StockActualKg);
    }

    [Fact]
    public void SincronizarStockKg_DebeCalcularBasadoEnUnidades()
    {
        // Arrange
        var producto = new Producto(
            Guid.NewGuid(),
            "Alimento",
            Guid.NewGuid(),
            Guid.NewGuid(),
            50m,
            100m);

        // Act
        producto.SincronizarStockKg(20); // 20 unidades * 50kg = 1000kg

        // Assert
        Assert.Equal(1000m, producto.StockActualKg);
    }

    [Fact]
    public void ActualizarStock_NoDebeSerNegativo()
    {
        // Arrange
        var producto = new Producto(
            Guid.NewGuid(),
            "Alimento",
            Guid.NewGuid(),
            Guid.NewGuid(),
            50m,
            100m,
            0,
            100m); // Inicia con 100kg

        // Act
        producto.ActualizarStock(10, TipoMovimiento.Salida); // 10 unidades * 50kg = 500kg

        // Assert
        Assert.Equal(0m, producto.StockActualKg);
    }
}
