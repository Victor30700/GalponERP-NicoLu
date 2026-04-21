using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Tests;

public class FormulaTests
{
    [Fact]
    public void Formula_DebeInicializarseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var nombre = "Fórmula Crecimiento";
        var etapa = "Crecimiento";
        var cantidadBase = 100m;

        // Act
        var formula = new Formula(id, nombre, etapa, cantidadBase);

        // Assert
        Assert.Equal(id, formula.Id);
        Assert.Equal(nombre, formula.Nombre);
        Assert.Equal(etapa, formula.Etapa);
        Assert.Equal(cantidadBase, formula.CantidadBase);
        Assert.Empty(formula.Detalles);
    }

    [Fact]
    public void Formula_DebeLanzarExcepcion_SiNombreEsVacio()
    {
        // Act & Assert
        Assert.Throws<FormulaDomainException>(() => new Formula(Guid.NewGuid(), "", "Etapa", 100));
    }

    [Fact]
    public void AgregarDetalle_DebeLanzarExcepcion_SiProductoYaExiste()
    {
        // Arrange
        var formula = new Formula(Guid.NewGuid(), "Test", "Engorde", 100);
        var productoId = Guid.NewGuid();
        formula.AgregarDetalle(productoId, 10);

        // Act & Assert
        Assert.Throws<FormulaDomainException>(() => formula.AgregarDetalle(productoId, 5));
    }

    [Fact]
    public void AgregarDetalle_DebeLanzarExcepcion_SiCantidadEsCero()
    {
        // Arrange
        var formula = new Formula(Guid.NewGuid(), "Test", "Engorde", 100);

        // Act & Assert
        Assert.Throws<FormulaDomainException>(() => formula.AgregarDetalle(Guid.NewGuid(), 0));
    }

    [Fact]
    public void FormulaDomainException_DebeHeredarDeDomainException()
    {
        // Arrange & Act
        var exception = new FormulaDomainException("Mensaje de prueba");

        // Assert
        Assert.IsAssignableFrom<DomainException>(exception);
    }
}
