using System.ComponentModel;
using GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Galpones.Queries.ListarGalpones;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using MediatR;
using Microsoft.SemanticKernel;

namespace GalponERP.Application.Agentes.Plugins;

public class ProduccionPlugin
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _userContext;

    public ProduccionPlugin(IMediator mediator, ICurrentUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    [KernelFunction]
    [Description("Registra las bajas o mortalidad de aves. Si no se especifica el galpón, el sistema intentará inferirlo o preguntará.")]
    public async Task<string> RegistrarBajasGalpon(
        [Description("Cantidad de aves muertas")] int cantidad,
        [Description("Causa o motivo de la muerte")] string motivo,
        [Description("Opcional: Nombre legible del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        // 1. Obtener Lotes Activos para inferencia inteligente
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();

        if (!lotesActivos.Any())
        {
            return "Error: No hay ningún lote activo en el sistema. Dile al usuario que primero debe abrir un lote para poder registrar bajas.";
        }

        LoteResponse? loteSeleccionado = null;

        // 2. Inferencia Inteligente (Regla 7)
        // Si solo hay 1 lote activo, elegirlo automáticamente (incluso si el usuario dio un nombre mal escrito o no dio ninguno)
        if (lotesActivos.Count == 1)
        {
            loteSeleccionado = lotesActivos.First();
        }
        else if (!string.IsNullOrWhiteSpace(nombreGalpon))
        {
            // 3. Si hay más de uno y el usuario dio un nombre, intentar coincidir
            loteSeleccionado = lotesActivos.FirstOrDefault(l => 
                l.NombreGalpon.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));
        }

        // 4. Fallback Conversacional (Regla 8)
        if (loteSeleccionado == null)
        {
            var nombresGalponesActivos = string.Join(", ", lotesActivos.Select(l => l.NombreGalpon));
            if (string.IsNullOrWhiteSpace(nombreGalpon))
            {
                return $"Hay múltiples lotes activos en: [{nombresGalponesActivos}]. Pregúntale al usuario a cuál de estos se refiere.";
            }
            return $"No encontré el galpón '{nombreGalpon}'. Los registros disponibles con lotes activos son: [{nombresGalponesActivos}]. Pregúntale al usuario a cuál de estos se refiere.";
        }

        // 5. Ejecución del Comando
        var fechaRegistro = DateTime.UtcNow;
        var command = new RegistrarMortalidadCommand(loteSeleccionado.Id, cantidad, motivo, fechaRegistro);
        
        if (_userContext.UsuarioId.HasValue)
        {
            command.UsuarioId = _userContext.UsuarioId.Value;
        }

        try 
        {
            var result = await _mediator.Send(command);
            return $"Registro de mortalidad exitoso. Se registraron {cantidad} bajas por '{motivo}' en el '{loteSeleccionado.NombreGalpon}' " +
                   $"(Lote con ingreso {loteSeleccionado.FechaIngreso:d}). ID de operación: {result}";
        }
        catch (Exception ex)
        {
            return $"Error técnico al procesar el registro: {ex.Message}";
        }
    }
}
