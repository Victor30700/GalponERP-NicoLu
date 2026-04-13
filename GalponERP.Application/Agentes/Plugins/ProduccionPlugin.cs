using System.ComponentModel;
using GalponERP.Application.Mortalidad.Commands.RegistrarMortalidad;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Galpones.Queries.ListarGalpones;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Common;
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
        [Description("Cantidad de aves muertas (debe ser mayor a cero)")] int cantidad,
        [Description("Causa o motivo de la muerte")] string motivo,
        [Description("Opcional: Nombre legible del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        // 0. Validación de rango (Sprint 83 - Paso 3)
        if (cantidad <= 0)
        {
            return "Error: La cantidad de bajas debe ser mayor a cero. No se permiten valores negativos o nulos.";
        }

        // 1. Obtener Lotes Activos para inferencia inteligente
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();

        if (!lotesActivos.Any())
        {
            return "Error: No hay ningún lote activo en el sistema. Dile al usuario que primero debe abrir un lote para poder registrar bajas.";
        }

        // 2. Resolución de Entidad con EntityResolver (Sprint 83 - Paso 1)
        var (loteSeleccionado, message) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Galpón con lote activo");
        
        if (loteSeleccionado == null)
        {
            return message!;
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
