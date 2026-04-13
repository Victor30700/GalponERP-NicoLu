using System.ComponentModel;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using GalponERP.Application.Inventario.Queries.ObtenerMovimientosLote;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class AnalisisPlugin
{
    private readonly IMediator _mediator;

    public AnalisisPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Analiza el impacto de las marcas de alimento en la mortalidad de los lotes, comparando históricos.")]
    public async Task<string> AnalizarImpactoAlimentoEnMortalidad()
    {
        try
        {
            var lotes = (await _mediator.Send(new ListarLotesQuery(SoloActivos: false))).ToList();
            if (!lotes.Any()) return "No hay datos de lotes para realizar el análisis.";

            var correlacion = new Dictionary<string, (int TotalBajas, int TotalAvesIniciales, int CantidadLotes)>();

            foreach (var lote in lotes.OrderByDescending(l => l.FechaInicio).Take(10)) // Analizar últimos 10 lotes
            {
                var movimientos = await _mediator.Send(new ObtenerMovimientosLoteQuery(lote.Id));
                var alimentoPrincipal = movimientos
                    .Where(m => m.CategoriaProducto.Equals("Alimento", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(m => m.NombreProducto)
                    .OrderByDescending(g => g.Sum(x => x.Cantidad))
                    .FirstOrDefault()?.Key ?? "Desconocido";

                if (alimentoPrincipal == "Desconocido") continue;

                if (!correlacion.ContainsKey(alimentoPrincipal))
                    correlacion[alimentoPrincipal] = (0, 0, 0);

                var mortalidad = (await _mediator.Send(new ObtenerMortalidadPorLoteQuery(lote.Id))).Sum(m => m.CantidadBajas);
                
                var actual = correlacion[alimentoPrincipal];
                correlacion[alimentoPrincipal] = (actual.TotalBajas + mortalidad, actual.TotalAvesIniciales + lote.CantidadInicial, actual.CantidadLotes + 1);
            }

            if (!correlacion.Any()) return "No se pudo establecer una correlación clara por falta de datos de consumo de alimento.";

            var sb = new StringBuilder();
            sb.AppendLine("ANÁLISIS DE CORRELACIÓN: ALIMENTO VS MORTALIDAD");
            sb.AppendLine("--------------------------------------------------");
            
            foreach (var item in correlacion.OrderBy(c => (double)c.Value.TotalBajas / c.Value.TotalAvesIniciales))
            {
                double tasaMortalidad = (double)item.Value.TotalBajas / item.Value.TotalAvesIniciales * 100;
                sb.AppendLine($"- Alimento: {item.Key}");
                sb.AppendLine($"  Tasa Mortalidad Promedio: {tasaMortalidad:F2}% (en {item.Value.CantidadLotes} lotes)");
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al analizar impacto de alimento en mortalidad: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Modo Consultor: La IA analiza los datos históricos y sugiere mejoras proactivas para la granja.")]
    public async Task<string> ModoConsultorSugiereMejoras()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- MODO CONSULTOR EXPERTO (ANÁLISIS PROACTIVO) ---");

            // 1. Análisis de Alimento
            var impactoAlimento = await AnalizarImpactoAlimentoEnMortalidad();
            sb.AppendLine("\n1. Hallazgos en Alimentación:");
            sb.AppendLine(impactoAlimento);

            // 2. Análisis de Lotes Recientes
            var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
            sb.AppendLine("\n2. Estado de Lotes Actuales:");
            foreach (var lote in lotesActivos)
            {
                var mortalidad = (double)lote.MortalidadPorcentaje;
                if (mortalidad > 5)
                {
                    sb.AppendLine($"- ⚠️ Lote en '{lote.GalponNombre}' tiene mortalidad alta ({mortalidad:F2}%). Sugerencia: Revisar bioseguridad y ventilación.");
                }
                else
                {
                    sb.AppendLine($"- Lote en '{lote.GalponNombre}' se mantiene estable ({mortalidad:F2}%).");
                }
            }

            sb.AppendLine("\n3. Recomendaciones Estratégicas:");
            sb.AppendLine("- Diversificar proveedores de alimento si se detecta variabilidad en mortalidad.");
            sb.AppendLine("- Implementar pesajes semanales estrictos para monitorear Ganancia de Peso Diaria (GPD).");
            sb.AppendLine("- Asegurar el cumplimiento del 100% del Calendario Sanitario.");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al sugerir mejoras de consultoría: {ex.Message}";
        }
    }
}
