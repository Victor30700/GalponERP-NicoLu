using System.ComponentModel;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class InventarioPlugin
{
    private readonly IMediator _mediator;

    public InventarioPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Consulta el stock actual de productos en el inventario.")]
    public async Task<string> ConsultarStockDisponible()
    {
        var stock = await _mediator.Send(new ObtenerStockActualQuery());
        
        if (!stock.Any()) return "No hay stock registrado en el sistema.";

        var sb = new StringBuilder();
        sb.AppendLine("Stock Actual de Productos:");
        foreach (var item in stock)
        {
            sb.AppendLine($"- {item.NombreProducto} ({item.TipoProducto}): {item.StockActual} {item.UnidadMedida} ({item.StockActualKg} Kg)");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Registra el consumo de alimento. Si no se especifican nombres, el sistema intentará inferirlos.")]
    public async Task<string> RegistrarConsumoAlimento(
        [Description("Cantidad consumida")] decimal cantidad,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre del producto (ej. 'Maíz')")] string? nombreProducto = null,
        [Description("Opcional: Justificación del consumo")] string? justificacion = null)
    {
        // 1. Resolver Lote Activo (Regla 7 y 8)
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "Error: No hay lotes activos. No se puede registrar consumo.";

        LoteResponse? loteSeleccionado = null;
        if (lotesActivos.Count == 1) 
        {
            loteSeleccionado = lotesActivos.First();
        }
        else if (!string.IsNullOrWhiteSpace(nombreGalpon))
        {
            loteSeleccionado = lotesActivos.FirstOrDefault(l => l.NombreGalpon.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));
        }

        if (loteSeleccionado == null)
        {
            var nombresGalpones = string.Join(", ", lotesActivos.Select(l => l.NombreGalpon));
            if (string.IsNullOrWhiteSpace(nombreGalpon))
            {
                return $"Hay múltiples lotes activos en: [{nombresGalpones}]. Pregúntale al usuario a cuál de estos se refiere.";
            }
            return $"No encontré el Galpón '{nombreGalpon}'. Los registros disponibles con lotes activos son: [{nombresGalpones}]. Pregúntale al usuario a cuál de estos se refiere.";
        }

        // 2. Resolver Producto (Regla 7 y 8 - Cascaded)
        var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
        var alimentosDisponibles = productos.Where(p => p.CategoriaNombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase)).ToList();

        if (!alimentosDisponibles.Any()) return "Error: No hay productos de tipo 'Alimento' registrados en el sistema.";

        ProductoResponse? productoSeleccionado = null;
        if (alimentosDisponibles.Count == 1) 
        {
            productoSeleccionado = alimentosDisponibles.First();
        }
        else if (!string.IsNullOrWhiteSpace(nombreProducto))
        {
            productoSeleccionado = alimentosDisponibles.FirstOrDefault(p => p.Nombre.Contains(nombreProducto, StringComparison.OrdinalIgnoreCase));
        }

        if (productoSeleccionado == null)
        {
            var nombresAlimentos = string.Join(", ", alimentosDisponibles.Select(p => p.Nombre));
            if (string.IsNullOrWhiteSpace(nombreProducto))
            {
                return $"Debo registrar el alimento en el {loteSeleccionado.NombreGalpon}, pero hay múltiples opciones de alimento: [{nombresAlimentos}]. Pregúntale al usuario cuál de estos consumieron.";
            }
            return $"No encontré el alimento '{nombreProducto}'. Los registros disponibles de tipo 'Alimento' son: [{nombresAlimentos}]. Pregúntale al usuario a cuál de estos se refiere.";
        }

        // 3. Ejecutar Comando
        try
        {
            var command = new RegistrarConsumoAlimentoCommand(loteSeleccionado.Id, productoSeleccionado.Id, cantidad, justificacion);
            var result = await _mediator.Send(command);
            return $"Consumo registrado exitosamente. Se consumieron {cantidad} {productoSeleccionado.UnidadMedidaNombre} de '{productoSeleccionado.Nombre}' en el '{loteSeleccionado.NombreGalpon}'. ID de operación: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar consumo: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Verifica los niveles de alimento y proyecta cuántos días de stock quedan.")]
    public async Task<string> VerificarNivelAlimento()
    {
        var alerta = await _mediator.Send(new VerificarNivelesAlimentoQuery());

        var sb = new StringBuilder();
        sb.AppendLine("Verificación de Niveles de Alimento:");
        sb.AppendLine($"- Stock Total: {Math.Round(alerta.StockActualAlimento, 2)} Kg");
        sb.AppendLine($"- Consumo Diario Global: {Math.Round(alerta.ConsumoDiarioGlobal, 2)} Kg/día");
        sb.AppendLine($"- Días de stock restantes: {(alerta.DiasRestantes >= 999 ? "Indefinido (sin consumo)" : Math.Round(alerta.DiasRestantes, 1).ToString())}");
        
        if (alerta.RequiereAlerta)
        {
            sb.AppendLine("⚠️ ALERTA: El stock es crítico (menos de 3 días). Se recomienda realizar un pedido pronto.");
        }

        return sb.ToString();
    }
}
