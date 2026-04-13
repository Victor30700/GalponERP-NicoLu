using GalponERP.Domain.Entities;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Tests;

public class RegistroBienestarTests
{
    [Fact]
    public void RegistroBienestar_DebeInicializarseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var loteId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;
        var usuarioId = Guid.NewGuid();
        var temp = 30.5m;
        var humedad = 60m;
        var agua = 500m;

        // Act
        var registro = new RegistroBienestar(id, loteId, fecha, temp, humedad, agua, "Todo normal", usuarioId);

        // Assert
        Assert.Equal(id, registro.Id);
        Assert.Equal(loteId, registro.LoteId);
        Assert.Equal(fecha.Date, registro.Fecha);
        Assert.Equal(temp, registro.Temperatura);
        Assert.Equal(humedad, registro.Humedad);
        Assert.Equal(agua, registro.ConsumoAgua);
    }

    [Fact]
    public void Actualizar_DebeModificarValores()
    {
        // Arrange
        var registro = new RegistroBienestar(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 30m, 60m, 500m, "Original", Guid.NewGuid());

        // Act
        registro.Actualizar(32m, 65m, 550m, "Aumentó calor");

        // Assert
        Assert.Equal(32m, registro.Temperatura);
        Assert.Equal(65m, registro.Humedad);
        Assert.Equal(550m, registro.ConsumoAgua);
        Assert.Equal("Aumentó calor", registro.Observaciones);
    }

    [Fact]
    public void RegistroBienestar_DebeLanzarExcepcion_SiLoteEsVacio()
    {
        // Act & Assert
        Assert.Throws<LoteDomainException>(() => 
            new RegistroBienestar(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow, 30m, 60m, 500m, null, Guid.NewGuid()));
    }
}
