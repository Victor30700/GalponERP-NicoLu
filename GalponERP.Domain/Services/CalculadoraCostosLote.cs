using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Services;

/// <summary>
/// Servicio de dominio para cálculos de costos y eficiencia.
/// </summary>
public class CalculadoraCostosLote
{
    /// <summary>
    /// Calcula el Índice de Conversión Alimenticia (FCR).
    /// FCR = Total Alimento Consumido (Kg) / Peso Total Ganado (Kg).
    /// </summary>
    /// <param name="totalAlimentoConsumidoKg">Suma de todo el alimento que ha salido hacia el lote.</param>
    /// <param name="pesoTotalPollosKg">Peso total estimado o real de los pollos vivos.</param>
    /// <returns>Valor decimal del FCR (ej. 1.55).</returns>
    public decimal CalcularFCR(decimal totalAlimentoConsumidoKg, decimal pesoTotalPollosKg)
    {
        if (pesoTotalPollosKg <= 0) return 0;
        
        // Redondeo a 2 decimales (estándar para FCR)
        return Math.Round(totalAlimentoConsumidoKg / pesoTotalPollosKg, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Calcula el costo total de producción de un lote hasta la fecha.
    /// </summary>
    /// <param name="amortizacionInfraestructura">Costo fijo por tiempo de uso del galpón.</param>
    /// <param name="costoPollitosBebe">Costo inicial de adquisición (CostoUnitario * CantidadInicial).</param>
    /// <param name="costoAlimentoConsumido">Costo monetario acumulado del alimento entregado al lote.</param>
    /// <param name="gastosOperativos">Colección de gastos (luz, agua, sueldos) asociados al lote.</param>
    /// <returns>Moneda con el costo total acumulado.</returns>
    public Moneda CalcularCostoTotal(
        Moneda amortizacionInfraestructura,
        Moneda costoPollitosBebe,
        Moneda costoAlimentoConsumido,
        IEnumerable<GastoOperativo> gastosOperativos)
    {
        var totalGastosOperativos = gastosOperativos
            .Select(g => g.Monto)
            .Aggregate(Moneda.Zero, (acc, next) => acc + next);

        return amortizacionInfraestructura + costoPollitosBebe + costoAlimentoConsumido + totalGastosOperativos;
    }
}
