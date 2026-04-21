using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Services;

namespace GalponERP.Domain.Services;

public class SanidadService : ISanidadService
{
    private const decimal UmbralCaidaConsumoAgua = 0.10m; // 10%

    public (bool EsAlerta, string? Mensaje) AnalizarDesviacionConsumoAgua(Guid loteId, decimal consumoActual, IEnumerable<RegistroBienestar> historial)
    {
        // Tomamos los últimos 3 registros con consumo de agua para promediar
        var ultimosRegistros = historial
            .Where(r => r.ConsumoAgua.HasValue && r.ConsumoAgua.Value > 0)
            .OrderByDescending(r => r.Fecha)
            .Take(3)
            .ToList();

        if (ultimosRegistros.Count < 2)
        {
            // No hay suficiente historial para comparar (necesitamos al menos 2 días previos para un promedio simple)
            return (false, null);
        }

        decimal promedioAnterior = ultimosRegistros.Average(r => r.ConsumoAgua!.Value);

        if (promedioAnterior <= 0) return (false, null);

        decimal desviacion = (promedioAnterior - consumoActual) / promedioAnterior;

        if (desviacion >= UmbralCaidaConsumoAgua)
        {
            return (true, $"¡ALERTA SANITARIA! El consumo de agua ha caído un {desviacion:P1}. " +
                          $"Promedio últimos días: {promedioAnterior:F1}L, Hoy: {consumoActual:F1}L. " +
                          $"Posible estrés o inicio de enfermedad.");
        }

        return (false, null);
    }
}
