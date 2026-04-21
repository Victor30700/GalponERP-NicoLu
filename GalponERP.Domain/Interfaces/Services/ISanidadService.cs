using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Services;

public interface ISanidadService
{
    /// <summary>
    /// Analiza el consumo de agua actual contra el historial para detectar desviaciones peligrosas.
    /// </summary>
    /// <param name="loteId">ID del lote a analizar.</param>
    /// <param name="consumoActual">Consumo registrado hoy.</param>
    /// <param name="historial">Últimos registros de bienestar del lote.</param>
    /// <returns>Tupla con (EsAlerta, Mensaje)</returns>
    (bool EsAlerta, string? Mensaje) AnalizarDesviacionConsumoAgua(Guid loteId, decimal consumoActual, IEnumerable<RegistroBienestar> historial);
}
