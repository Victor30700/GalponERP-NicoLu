namespace GalponERP.Domain.Services;

public record ProyeccionEtapa(string Etapa, int DiasInicio, int DiasFin, decimal PorcentajeConsumo, decimal ConsumoKg, decimal CostoEstimado);

public record ResultadoSimulacion(
    int CantidadPollos,
    decimal PesoEsperadoTotalKg,
    decimal AlimentoTotalKg,
    decimal CostoAlimentoTotal,
    decimal IngresosProyectados,
    decimal UtilidadBrutaProyectada,
    List<ProyeccionEtapa> DetallesEtapas);

/// <summary>
/// Servicio de dominio "puro" para proyectar la producción y rentabilidad de un lote.
/// No tiene dependencias de base de datos.
/// </summary>
public class SimuladorProyeccionLote
{
    private const decimal FCR_BASE_DEFAULT = 1.6m;

    public ResultadoSimulacion Proyectar(
        int cantidadPollos,
        decimal pesoEsperadoPorPolloKg,
        decimal precioAlimentoPorKg,
        decimal precioVentaPorKg,
        decimal? fcrPersonalizado = null)
    {
        decimal fcr = fcrPersonalizado ?? FCR_BASE_DEFAULT;
        decimal pesoTotalProyectado = cantidadPollos * pesoEsperadoPorPolloKg;
        decimal alimentoTotalKg = pesoTotalProyectado * fcr;
        decimal costoAlimentoTotal = alimentoTotalKg * precioAlimentoPorKg;
        decimal ingresosProyectados = pesoTotalProyectado * precioVentaPorKg;
        decimal utilidadBruta = ingresosProyectados - costoAlimentoTotal;

        var etapas = new List<ProyeccionEtapa>
        {
            new("Inicio", 1, 14, 0.20m, alimentoTotalKg * 0.20m, (alimentoTotalKg * 0.20m) * precioAlimentoPorKg),
            new("Crecimiento", 15, 28, 0.35m, alimentoTotalKg * 0.35m, (alimentoTotalKg * 0.35m) * precioAlimentoPorKg),
            new("Engorde", 29, 45, 0.45m, alimentoTotalKg * 0.45m, (alimentoTotalKg * 0.45m) * precioAlimentoPorKg)
        };

        return new ResultadoSimulacion(
            cantidadPollos,
            pesoTotalProyectado,
            alimentoTotalKg,
            costoAlimentoTotal,
            ingresosProyectados,
            utilidadBruta,
            etapas);
    }
}
