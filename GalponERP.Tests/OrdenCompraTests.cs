using GalponERP.Domain.Entities;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Tests;

public class OrdenCompraTests
{
    [Fact]
    public void OrdenCompra_DebeInicializarseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proveedorId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;
        var usuarioId = Guid.NewGuid();
        var total = new Moneda(0);

        // Act
        var orden = new OrdenCompra(id, proveedorId, fecha, total, usuarioId, "Nota inicial");

        // Assert
        Assert.Equal(id, orden.Id);
        Assert.Equal(proveedorId, orden.ProveedorId);
        Assert.Equal(EstadoOrdenCompra.Pendiente, orden.Estado);
        Assert.Empty(orden.Items);
        Assert.Equal(0, orden.Total.Monto);
    }

    [Fact]
    public void AgregarItem_DebeActualizarTotalYListaItems()
    {
        // Arrange
        var orden = new OrdenCompra(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Moneda(0), Guid.NewGuid());
        var productoId = Guid.NewGuid();
        var cantidad = 10m;
        var precioUnitario = new Moneda(5.50m);

        // Act
        orden.AgregarItem(Guid.NewGuid(), productoId, cantidad, precioUnitario);

        // Assert
        Assert.Single(orden.Items);
        Assert.Equal(55.00m, orden.Total.Monto);
        var item = orden.Items.First();
        Assert.Equal(productoId, item.ProductoId);
        Assert.Equal(cantidad, item.Cantidad);
        Assert.Equal(55.00m, item.Total.Monto);
    }

    [Fact]
    public void MarcarComoRecibida_DebeCambiarEstado()
    {
        // Arrange
        var orden = new OrdenCompra(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Moneda(0), Guid.NewGuid());

        // Act
        orden.MarcarComoRecibida();

        // Assert
        Assert.Equal(EstadoOrdenCompra.Recibida, orden.Estado);
    }

    [Fact]
    public void Cancelar_DebeCambiarEstadoYAgregarNota()
    {
        // Arrange
        var orden = new OrdenCompra(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Moneda(0), Guid.NewGuid(), "Original");

        // Act
        orden.Cancelar("Falta de stock");

        // Assert
        Assert.Equal(EstadoOrdenCompra.Cancelada, orden.Estado);
        Assert.Contains("Falta de stock", orden.Nota);
    }

    [Fact]
    public void AgregarItem_DebeLanzarExcepcion_SiNoEstaPendiente()
    {
        // Arrange
        var orden = new OrdenCompra(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Moneda(0), Guid.NewGuid());
        orden.MarcarComoRecibida();

        // Act & Assert
        Assert.Throws<InventarioDomainException>(() => 
            orden.AgregarItem(Guid.NewGuid(), Guid.NewGuid(), 10, new Moneda(5)));
    }

    [Fact]
    public void OrdenCompra_DebeLanzarExcepcion_SiProveedorEsVacio()
    {
        // Act & Assert
        Assert.Throws<InventarioDomainException>(() => 
            new OrdenCompra(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow, new Moneda(0), Guid.NewGuid()));
    }
}
