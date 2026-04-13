using System.ComponentModel;
using GalponERP.Application.Lotes.Commands.CrearLote;
using GalponERP.Application.Lotes.Commands.CerrarLote;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Galpones.Queries.ListarGalpones;
using GalponERP.Application.PlantillasSanitarias.Queries;
using GalponERP.Application.Agentes.Confirmacion.Commands;
using GalponERP.Application.Common;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;
using System.Text.Json;

namespace GalponERP.Application.Agentes.Plugins;

public class GestionLotesPlugin
{
    private readonly IMediator _mediator;

    public GestionLotesPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Inicia un nuevo lote de producción en un galpón específico. Requiere confirmación.")]
    public async Task<string> AbrirNuevoLote(
        [Description("Cantidad inicial de pollitos")] int cantidadInicial,
        [Description("Costo unitario por cada pollito")] decimal costoUnitario,
        [Description("ID de la conversación actual")] Guid conversacionId,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre de la plantilla sanitaria a usar (ej. 'Cobb 500 Estándar')")] string? nombrePlantilla = null,
        [Description("Obligatorio para ejecutar la acción: confirmar=true")] bool confirmar = false)
    {
        // 1. Resolver Galpón (Regla 7 y 8)
        var galpones = (await _mediator.Send(new ListarGalponesQuery())).Where(g => g.IsActive).ToList();
        if (!galpones.Any()) return "Error: No hay galpones activos registrados.";

        var (galponSeleccionado, msgGalpon) = EntityResolver.Resolve(galpones, nombreGalpon, g => g.Nombre, "Galpón");
        if (galponSeleccionado == null) return msgGalpon!;

        // 2. Resolver Plantilla Sanitaria (Opcional)
        Guid? plantillaId = null;
        var plantillas = (await _mediator.Send(new ObtenerPlantillasQuery())).ToList();
        if (plantillas.Any())
        {
            var (plantillaSeleccionada, msgPlantilla) = EntityResolver.Resolve(plantillas, nombrePlantilla, p => p.Nombre, "Plantilla");
            // Nota: Plantilla es opcional, si no se encuentra y se especificó nombre, informamos.
            if (plantillaSeleccionada == null && !string.IsNullOrWhiteSpace(nombrePlantilla)) return msgPlantilla!;
            plantillaId = plantillaSeleccionada?.Id;
        }

        // 3. Verificar Confirmación
        if (!confirmar)
        {
            var parametros = new
            {
                cantidadInicial,
                costoUnitario,
                conversacionId,
                nombreGalpon = galponSeleccionado.Nombre,
                nombrePlantilla
            };
            
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(GestionLotesPlugin), nameof(AbrirNuevoLote), json));

            return $"¿Confirmas que deseas abrir un nuevo lote en '{galponSeleccionado.Nombre}' con {cantidadInicial} pollitos a S/ {costoUnitario} c/u?";
        }

        // 4. Ejecutar Comando
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
    [Description("Cierra un lote de producción finalizado. Acción CRÍTICA: Requiere confirmación.")]
    public async Task<string> CerrarLote(
        [Description("ID de la conversación actual")] Guid conversacionId,
        [Description("Opcional: Nombre del galpón donde está el lote (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Obligatorio para ejecutar la acción: confirmar=true")] bool confirmar = false)
    {
        // 1. Resolver Lote Activo
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para cerrar.";

        var (loteSeleccionado, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.GalponNombre, "Lote Activo");
        if (loteSeleccionado == null) return msgLote!;

        // 2. Verificar Confirmación
        if (!confirmar)
        {
            var parametros = new
            {
                conversacionId,
                nombreGalpon = loteSeleccionado.GalponNombre
            };
            
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(GestionLotesPlugin), nameof(CerrarLote), json));

            return $"⚠️ ATENCIÓN: Estás a punto de CERRAR el lote en '{loteSeleccionado.GalponNombre}'. Esta acción es irreversible y generará el balance final. ¿Estás seguro de que deseas proceder?";
        }

        // 3. Ejecutar Comando
        try
        {
            var result = await _mediator.Send(new CerrarLoteCommand(loteSeleccionado.Id));
            var sb = new StringBuilder();
            sb.AppendLine($"Lote en '{loteSeleccionado.GalponNombre}' cerrado exitosamente.");
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
