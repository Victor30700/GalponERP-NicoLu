using System.ComponentModel;
using GalponERP.Application.Inventario.Commands.RegistrarIngresoMercaderia;
using GalponERP.Application.Inventario.Commands.RegistrarPagoCompra;
using GalponERP.Application.Inventario.Commands.CrearOrdenCompra;
using GalponERP.Application.Inventario.Commands.RecibirOrdenCompra;
using GalponERP.Application.Inventario.Queries.ListarOrdenesCompraPendientes;
using GalponERP.Application.Finanzas.Queries.ObtenerCuentasPorPagar;
using GalponERP.Application.Productos.Queries;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Proveedores.Queries.ListarProveedores;
using GalponERP.Application.Common;
using GalponERP.Domain.Entities;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class AbastecimientoPlugin
{
    private readonly IMediator _mediator;

    public AbastecimientoPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Crea una Orden de Compra (OC) para uno o varios productos de un proveedor. La OC queda pendiente hasta ser recibida.")]
    public async Task<string> CrearOrdenCompra(
        [Description("Nombre del proveedor (ej. 'Avícola Santa Rosa')")] string nombreProveedor,
        [Description("Lista de productos con formato 'Producto:Cantidad:PrecioUnitario' separados por punto y coma (ej. 'Maíz:100:1.5;Soya:50:2.0')")] string itemsTexto,
        [Description("Opcional: Nota o descripción adicional")] string? nota = null)
    {
        // 1. Resolver Proveedor
        var proveedores = (await _mediator.Send(new ListarProveedoresQuery())).Where(p => p.IsActive).ToList();
        var (proveedor, msgProv) = EntityResolver.Resolve(proveedores, nombreProveedor, p => p.RazonSocial, "Proveedor");
        if (proveedor == null) return msgProv!;

        // 2. Resolver Productos y preparar items
        var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
        var items = new List<OrdenCompraItemDto>();
        var partes = itemsTexto.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var parte in partes)
        {
            var subpartes = parte.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (subpartes.Length < 2) continue;

            var nombreProd = subpartes[0].Trim();
            var (prod, _) = EntityResolver.Resolve(productos, nombreProd, p => p.Nombre, "Producto");
            if (prod == null) return $"No se pudo resolver el producto '{nombreProd}'.";

            if (!decimal.TryParse(subpartes[1].Trim(), out decimal cant)) continue;
            decimal precio = 0;
            if (subpartes.Length >= 3) decimal.TryParse(subpartes[2].Trim(), out precio);

            items.Add(new OrdenCompraItemDto(prod.Id, cant, precio));
        }

        if (!items.Any()) return "No se pudieron procesar los ítems de la orden. Use el formato 'Producto:Cantidad:Precio'.";

        try
        {
            var command = new CrearOrdenCompraCommand(proveedor.Id, items, nota);
            var result = await _mediator.Send(command);
            return $"Orden de Compra creada exitosamente para '{proveedor.RazonSocial}'. ID OC: {result}. Queda pendiente de recepción.";
        }
        catch (Exception ex)
        {
            return $"Error al crear la orden de compra: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Lista las Órdenes de Compra (OC) que están pendientes de ser recibidas en el almacén.")]
    public async Task<string> ConsultarOrdenesPendientes()
    {
        var ordenes = await _mediator.Send(new ListarOrdenesCompraPendientesQuery());
        if (!ordenes.Any()) return "No hay órdenes de compra pendientes.";

        var sb = new StringBuilder();
        sb.AppendLine("Órdenes de Compra PENDIENTES:");
        foreach (var o in ordenes)
        {
            sb.AppendLine($"- [{o.Fecha:dd/MM/yyyy}] Proveedor: {o.RazonSocialProveedor}. Total: S/ {o.Total}. (ID: {o.Id})");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Registra la recepción física de una Orden de Compra, ingresando los productos al inventario y generando la deuda.")]
    public async Task<string> RecibirOrdenCompra(
        [Description("ID de la Orden de Compra (Guid)")] string ordenCompraId,
        [Description("Monto pagado al momento de recibir (0 si es crédito total)")] decimal montoPagado,
        [Description("Opcional: Nota sobre la recepción")] string? notaRecibo = null)
    {
        if (!Guid.TryParse(ordenCompraId, out Guid id))
            return "El ID de la orden de compra no es válido.";

        try
        {
            var command = new RecibirOrdenCompraCommand(id, montoPagado, notaRecibo);
            var result = await _mediator.Send(command);
            return $"Orden de Compra recibida exitosamente. Se ha generado la compra formal y actualizado el inventario. ID Compra: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al recibir la orden: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra una compra directa de un insumo (alimento, medicina, etc.) y actualiza el inventario inmediatamente.")]
    public async Task<string> RegistrarCompraDirecta(
        [Description("Nombre del producto comprado (ej. 'Maíz')")] string nombreProducto,
        [Description("Nombre del proveedor (ej. 'Avícola Santa Rosa')")] string nombreProveedor,
        [Description("Cantidad comprada (debe ser mayor a cero)")] decimal cantidad,
        [Description("Costo total de la compra (debe ser mayor a cero)")] decimal costoTotal,
        [Description("Monto pagado inicialmente (puede ser 0 si es a crédito)")] decimal montoPagado,
        [Description("Opcional: Nota o descripción adicional")] string? nota = null)
    {
        // 0. Validación de rango (Sprint 83 - Paso 3)
        if (cantidad <= 0 || costoTotal <= 0 || montoPagado < 0)
        {
            return "Error: La cantidad y el costo total deben ser mayores a cero. El monto pagado no puede ser negativo.";
        }

        if (montoPagado > costoTotal)
        {
            return "Error: El monto pagado no puede ser mayor al costo total de la compra.";
        }

        // 1. Resolver Producto
        var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
        var (producto, msgProd) = EntityResolver.Resolve(productos, nombreProducto, p => p.Nombre, "Producto");
        if (producto == null) return msgProd!;

        // 2. Resolver Proveedor
        var proveedores = (await _mediator.Send(new ListarProveedoresQuery())).Where(p => p.IsActive).ToList();
        var (proveedor, msgProv) = EntityResolver.Resolve(proveedores, nombreProveedor, p => p.RazonSocial, "Proveedor");
        if (proveedor == null) return msgProv!;

        // 3. Ejecutar Comando
        try
        {
            var command = new RegistrarIngresoMercaderiaCommand(producto.Id, cantidad, costoTotal, proveedor.Id, montoPagado, nota);
            var result = await _mediator.Send(command);
            return $"Compra directa registrada exitosamente por {cantidad} {producto.UnidadMedidaNombre} de '{producto.Nombre}' a '{proveedor.RazonSocial}'. ID de Compra: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar la compra: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Consulta las deudas pendientes con proveedores (Cuentas por Pagar), mostrando fechas de vencimiento y alertas.")]
    public async Task<string> ConsultarCuentasPorPagar()
    {
        var cuentas = await _mediator.Send(new ObtenerCuentasPorPagarQuery());
        if (!cuentas.Any()) return "No hay cuentas por pagar pendientes.";

        var sb = new StringBuilder();
        sb.AppendLine("Cuentas por Pagar (Deudas a Proveedores):");
        var hoy = DateTime.Today;

        foreach (var c in cuentas.OrderBy(x => x.FechaVencimiento))
        {
            var alerta = c.FechaVencimiento.Date < hoy ? "⚠️ VENCIDO" : 
                         (c.FechaVencimiento.Date - hoy).TotalDays <= 3 ? "⏳ PRÓXIMO" : "";
            
            sb.AppendLine($"- [{c.FechaVencimiento:dd/MM/yyyy}] {alerta} Proveedor: {c.RazonSocialProveedor}. Saldo: S/ {c.SaldoPendiente}. (ID Compra: {c.CompraId})");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Registra un pago a un proveedor para saldar una deuda de compra.")]
    public async Task<string> RegistrarPagoAProveedor(
        [Description("ID de la compra a la que se aplica el pago (Guid)")] string compraId,
        [Description("Monto a pagar (debe ser mayor a cero)")] decimal monto,
        [Description("Método de pago (0: Efectivo, 1: Transferencia, 2: Tarjeta, 3: Otros)")] int metodoPago)
    {
        if (monto <= 0) return "Error: El monto a pagar debe ser mayor a cero.";

        if (!Guid.TryParse(compraId, out Guid id))
            return "El ID de la compra no tiene un formato válido.";

        try
        {
            var command = new RegistrarPagoCompraCommand(monto, DateTime.UtcNow, (MetodoPago)metodoPago)
            {
                CompraId = id
            };
            await _mediator.Send(command);
            return $"Se registró un pago de S/ {monto} a la compra ID {compraId} exitosamente.";
        }
        catch (Exception ex)
        {
            return $"Error al registrar el pago: {ex.Message}";
        }
    }
}

