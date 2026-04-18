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
            20m); // Inicia con 20 unidades * 50kg = 1000kg

        // Act
        producto.ActualizarStock(5, TipoMovimiento.Salida); // 5 unidades * 50kg = 250kg

        // Assert
        Assert.Equal(750m, producto.StockActualKg);
        Assert.Equal(15m, producto.StockActual);
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
    public void Constructor_DebePermitirPesoCero_ParaProductosNoAlimenticios()
    {
        // Arrange & Act
        var producto = new Producto(
            Guid.NewGuid(),
            "Medicina",
            Guid.NewGuid(),
            Guid.NewGuid(),
            0m, // Peso 0
            10m);

        // Assert
        Assert.Equal(0m, producto.PesoUnitarioKg);
        Assert.Equal(0m, producto.StockActualKg);
    }

    [Fact]
    public void ActualizarStock_ConPesoCero_DebeMantenerStockKgEnCero()
    {
        // Arrange
        var producto = new Producto(
            Guid.NewGuid(),
            "Vitamina",
            Guid.NewGuid(),
            Guid.NewGuid(),
            0m,
            5m);

        // Act
        producto.ActualizarStock(10, TipoMovimiento.Entrada);

        // Assert
        Assert.Equal(10m, producto.StockActual);
        Assert.Equal(0m, producto.StockActualKg);
    }
}
