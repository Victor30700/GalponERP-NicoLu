using System.ComponentModel;
using GalponERP.Application.Lotes.Commands.CrearLote;
using GalponERP.Application.Lotes.Commands.CerrarLote;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Galpones.Queries.ListarGalpones;
using GalponERP.Application.PlantillasSanitarias.Queries;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class GestionLotesPlugin
{
    private readonly IMediator _mediator;

    public GestionLotesPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Inicia un nuevo lote de producción en un galpón específico.")]
    public async Task<string> AbrirNuevoLote(
        [Description("Cantidad inicial de pollitos")] int cantidadInicial,
        [Description("Costo unitario por cada pollito")] decimal costoUnitario,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre de la plantilla sanitaria a usar (ej. 'Cobb 500 Estándar')")] string? nombrePlantilla = null)
    {
        // 1. Resolver Galpón (Regla 7 y 8)
        var galpones = (await _mediator.Send(new ListarGalponesQuery())).Where(g => g.IsActive).ToList();
        if (!galpones.Any()) return "Error: No hay galpones activos registrados.";

        GalponResponse? galponSeleccionado = null;
        if (galpones.Count == 1) galponSeleccionado = galpones.First();
        else if (!string.IsNullOrWhiteSpace(nombreGalpon))
            galponSeleccionado = galpones.FirstOrDefault(g => g.Nombre.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));

        if (galponSeleccionado == null)
        {
            var lista = string.Join(", ", galpones.Select(g => g.Nombre));
            if (string.IsNullOrWhiteSpace(nombreGalpon))
                return $"Para abrir un lote, necesito saber en qué galpón. Disponibles: [{lista}].";
            return $"No encontré el Galpón '{nombreGalpon}'. Registros: [{lista}].";
        }

        // 2. Resolver Plantilla Sanitaria (Opcional)
        Guid? plantillaId = null;
        var plantillas = (await _mediator.Send(new ObtenerPlantillasQuery())).ToList();
        if (plantillas.Any())
        {
            PlantillaSanitariaDto? plantillaSeleccionada = null;
            if (plantillas.Count == 1) plantillaSeleccionada = plantillas.First();
            else if (!string.IsNullOrWhiteSpace(nombrePlantilla))
                plantillaSeleccionada = plantillas.FirstOrDefault(p => p.Nombre.Contains(nombrePlantilla, StringComparison.OrdinalIgnoreCase));

            if (plantillaSeleccionada == null && !string.IsNullOrWhiteSpace(nombrePlantilla))
            {
                var listaP = string.Join(", ", plantillas.Select(p => p.Nombre));
                return $"No encontré la plantilla '{nombrePlantilla}'. Disponibles: [{listaP}]. ¿Deseas usar alguna de estas?";
            }
            plantillaId = plantillaSeleccionada?.Id;
        }

        // 3. Ejecutar Comando
        try
        {
            var command = new CrearLoteCommand(galponSeleccionado.Id, DateTime.UtcNow, cantidadInicial, costoUnitario, plantillaId);
            var result = await _mediator.Send(command);
            return $"Lote abierto exitosamente en el '{galponSeleccionado.Nombre}' con {cantidadInicial} pollitos. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al abrir el lote: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Cierra un lote de producción finalizado y muestra el resumen de rentabilidad.")]
    public async Task<string> CerrarLote(
        [Description("Opcional: Nombre del galpón donde está el lote (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        // 1. Resolver Lote Activo
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para cerrar.";

        LoteResponse? loteSeleccionado = null;
        if (lotesActivos.Count == 1) loteSeleccionado = lotesActivos.First();
        else if (!string.IsNullOrWhiteSpace(nombreGalpon))
            loteSeleccionado = lotesActivos.FirstOrDefault(l => l.NombreGalpon.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));

        if (loteSeleccionado == null)
        {
            var lista = string.Join(", ", lotesActivos.Select(l => l.NombreGalpon));
            return $"Hay múltiples lotes activos en: [{lista}]. ¿Cuál de ellos deseas cerrar?";
        }

        // 2. Ejecutar Comando
        try
        {
            var result = await _mediator.Send(new CerrarLoteCommand(loteSeleccionado.Id));
            var sb = new StringBuilder();
            sb.AppendLine($"Lote en '{loteSeleccionado.NombreGalpon}' cerrado exitosamente.");
            sb.AppendLine("RESUMEN DE CIERRE:");
            sb.AppendLine($"- Ingresos Totales: S/ {result.TotalIngresos}");
            sb.AppendLine($"- Costos Totales: S/ {result.TotalCostos}");
            sb.AppendLine($"- Utilidad Neta: S/ {result.UtilidadNeta}");
            sb.AppendLine($"- Mortalidad: {result.PorcentajeMortalidad}%");
            sb.AppendLine($"- Conversión Alimenticia (FCR): {result.FCR}");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al cerrar el lote: {ex.Message}";
        }
    }
}
