using GalponERP.Domain.Services;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Entities;

namespace GalponERP.Tests;

public class CalculadoraCostosLoteTests
{
    private readonly CalculadoraCostosLote _calculadora;

    public CalculadoraCostosLoteTests()
    {
        _calculadora = new CalculadoraCostosLote();
    }

    [Fact]
    public void CalcularFCR_DebeRetornarValorExacto()
    {
        // Arrange
        decimal alimentoKg = 1550m;
        decimal pesoPollosKg = 1000m;

        // Act
        decimal fcr = _calculadora.CalcularFCR(alimentoKg, pesoPollosKg);

        // Assert
        Assert.Equal(1.55m, fcr);
    }

    [Fact]
    public void CalcularFCR_DebeRetornarCero_SiPesoEsCero()
    {
        // Act
        decimal fcr = _calculadora.CalcularFCR(100, 0);

        // Assert
        Assert.Equal(0, fcr);
    }

    [Fact]
    public void CalcularCostoTotal_DebeSumarTodosLos_Componentes()
    {
        // Arrange
        var amortizacion = new Moneda(100);
        var costoPollitos = new Moneda(1500);
        var costoAlimento = new Moneda(3000);
        var galponId = Guid.NewGuid();
        var loteId = Guid.NewGuid();
        
        var gastosOperativos = new List<GastoOperativo>
        {
            new GastoOperativo(Guid.NewGuid(), galponId, loteId, "Luz", new Moneda(50), DateTime.UtcNow),
            new GastoOperativo(Guid.NewGuid(), galponId, loteId, "Agua", new Moneda(30), DateTime.UtcNow)
        };

        // Act
        var total = _calculadora.CalcularCostoTotal(
            amortizacion,
            costoPollitos,
            costoAlimento,
            gastosOperativos
        );

        // Assert
        // 100 + 1500 + 3000 + 50 + 30 = 4680
        Assert.Equal(4680m, total.Monto);
    }
}
