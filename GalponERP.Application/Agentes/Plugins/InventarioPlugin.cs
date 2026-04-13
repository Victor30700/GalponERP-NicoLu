using System.ComponentModel;
using GalponERP.Application.Inventario.Queries.ObtenerStockActual;
using GalponERP.Application.Inventario.Commands.RegistrarConsumoAlimento;
using GalponERP.Application.Inventario.Queries.VerificarNivelesAlimento;
using GalponERP.Application.Inventario.Queries.PredecirAgotamientoStock;
using GalponERP.Application.Productos.Queries;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Agentes.Confirmacion.Commands;
using GalponERP.Application.Common;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;
using System.Text.Json;

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
        try
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
        catch (Exception ex)
        {
            return $"Error al consultar stock: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra el consumo de alimento. Requiere confirmación si no se envía confirmar=true.")]
    public async Task<string> RegistrarConsumoAlimento(
        [Description("Cantidad consumida (debe ser mayor a cero)")] decimal cantidad,
        [Description("ID de la conversación actual")] Guid conversacionId,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre del producto (ej. 'Maíz')")] string? nombreProducto = null,
        [Description("Opcional: Justificación del consumo")] string? justificacion = null,
        [Description("Obligatorio para ejecutar la acción: confirmar=true")] bool confirmar = false)
    {
        try
        {
            // 0. Validación de rango (Sprint 83 - Paso 3)
            if (cantidad <= 0)
            {
                return "Error: La cantidad de consumo debe ser mayor a cero.";
            }

            // 1. Resolver Lote Activo (Regla 7 y 8)
            var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
            if (!lotesActivos.Any()) return "Error: No hay lotes activos. No se puede registrar consumo.";

            var (loteSeleccionado, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.GalponNombre, "Galpón");
            if (loteSeleccionado == null) return msgLote!;

            // 2. Resolver Producto (Regla 7 y 8 - Cascaded)
            var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
            var alimentosDisponibles = productos.Where(p => p.CategoriaNombre.Equals("Alimento", StringComparison.OrdinalIgnoreCase)).ToList();

            if (!alimentosDisponibles.Any()) return "Error: No hay productos de tipo 'Alimento' registrados en el sistema.";

            var (productoSeleccionado, msgProducto) = EntityResolver.Resolve(alimentosDisponibles, nombreProducto, p => p.Nombre, "Alimento");
            if (productoSeleccionado == null) return msgProducto!;

            // 3. Verificar Confirmación (Regla 10)
            if (!confirmar)
            {
                var parametros = new
                {
                    cantidad,
                    conversacionId,
                    nombreGalpon = loteSeleccionado.GalponNombre,
                    nombreProducto = productoSeleccionado.Nombre,
                    justificacion
                };
                
                var json = JsonSerializer.Serialize(parametros);
                await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(InventarioPlugin), nameof(RegistrarConsumoAlimento), json));

                return $"Entiendo que quieres registrar un consumo de {cantidad} {productoSeleccionado.UnidadMedidaNombre} de '{productoSeleccionado.Nombre}' en el '{loteSeleccionado.GalponNombre}'. ¿Es correcto? Por favor, confirma para proceder.";
            }

            // 4. Ejecutar Comando
            var command = new RegistrarConsumoAlimentoCommand(loteSeleccionado.Id, productoSeleccionado.Id, cantidad, justificacion);
            var result = await _mediator.Send(command);
            return $"Consumo registrado exitosamente. Se consumieron {cantidad} {productoSeleccionado.UnidadMedidaNombre} de '{productoSeleccionado.Nombre}' en el '{loteSeleccionado.GalponNombre}'. ID de operación: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar consumo: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Predice cuándo se agotará el stock de un producto basado en el consumo promedio de las últimas 2 semanas.")]
    public async Task<string> PredecirAgotamientoStock(
        [Description("Nombre del producto a analizar")] string nombreProducto)
    {
        try
        {
            var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
            var (producto, msg) = EntityResolver.Resolve(productos, nombreProducto, p => p.Nombre, "Producto");
            if (producto == null) return msg!;

            var prediccion = await _mediator.Send(new PredecirAgotamientoStockQuery(producto.Id));

            if (prediccion.ConsumoPromedioDiario <= 0)
            {
                return $"No hay datos de consumo reciente para '{producto.Nombre}'. El stock actual es de {prediccion.StockActual} {producto.UnidadMedidaNombre}.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"--- PREDICCIÓN DE SUMINISTRO: {producto.Nombre} ---");
            sb.AppendLine($"- Stock Actual: {prediccion.StockActual} {producto.UnidadMedidaNombre}");
            sb.AppendLine($"- Consumo Promedio: {prediccion.ConsumoPromedioDiario:N2} {producto.UnidadMedidaNombre}/día");
            sb.AppendLine($"- Tiempo Restante: {prediccion.DiasRestantes:N1} días");
            sb.AppendLine($"- Fecha de Agotamiento: {prediccion.FechaEstimadaAgotamiento:dd/MM/yyyy}");

            if (prediccion.RequierePedidoUrgente)
            {
                sb.AppendLine("\n⚠️ ALERTA: Se recomienda realizar un pedido de reabastecimiento pronto.");
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al realizar la predicción: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Verifica los niveles de alimento y proyecta cuántos días de stock quedan.")]
    public async Task<string> VerificarNivelAlimento()
    {
        try
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
        catch (Exception ex)
        {
            return $"Error al verificar niveles de alimento: {ex.Message}";
        }
    }
}
