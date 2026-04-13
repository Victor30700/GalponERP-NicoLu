using System.ComponentModel;
using GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;
using GalponERP.Application.Ventas.Commands.RegistrarPago;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Application.Clientes.Queries.ListarClientes;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Common;
using GalponERP.Domain.Entities;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class VentasPlugin
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _userContext;

    public VentasPlugin(IMediator mediator, ICurrentUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    [KernelFunction]
    [Description("Registra una venta de aves de un lote específico.")]
    public async Task<string> RegistrarVenta(
        [Description("Cantidad de pollos vendidos (debe ser mayor a cero)")] int cantidad,
        [Description("Peso total de los pollos en Kilogramos (debe ser mayor a cero)")] decimal pesoTotalKg,
        [Description("Precio por kilogramo pactado (debe ser mayor a cero)")] decimal precioPorKilo,
        [Description("Opcional: Nombre del galpón del cual se venden (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre del cliente (ej. 'Distribuidora Avícola')")] string? nombreCliente = null)
    {
        // 0. Validación de rango (Sprint 83 - Paso 3)
        if (cantidad <= 0 || pesoTotalKg <= 0 || precioPorKilo <= 0)
        {
            return "Error: La cantidad, el peso y el precio deben ser mayores a cero.";
        }

        // 1. Resolver Lote Activo (Regla 7 y 8)
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "Error: No hay lotes activos para realizar ventas.";

        var (loteSeleccionado, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Galpón");
        if (loteSeleccionado == null) return msgLote!;

        // 2. Resolver Cliente (Regla 7 y 8)
        var clientes = (await _mediator.Send(new ListarClientesQuery())).Where(c => c.IsActive).ToList();
        var (clienteSeleccionado, msgCliente) = EntityResolver.Resolve(clientes, nombreCliente, c => c.Nombre, "Cliente");
        if (clienteSeleccionado == null) return msgCliente!;

        // 3. Ejecutar Comando
        try
        {
            var command = new RegistrarVentaParcialCommand(
                loteSeleccionado.Id,
                clienteSeleccionado.Id,
                DateTime.UtcNow,
                cantidad,
                pesoTotalKg,
                precioPorKilo);

            if (_userContext.UsuarioId.HasValue) command.UsuarioId = _userContext.UsuarioId.Value;

            var result = await _mediator.Send(command);
            var total = Math.Round(pesoTotalKg * precioPorKilo, 2);
            return $"Venta registrada con éxito. Se vendieron {cantidad} pollos ({pesoTotalKg} Kg) a '{clienteSeleccionado.Nombre}' por un total de S/ {total}. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar la venta: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Consulta las ventas recientes registradas en el sistema.")]
    public async Task<string> ConsultarVentasRecientes()
    {
        var ventas = await _mediator.Send(new ObtenerVentasQuery());
        if (!ventas.Any()) return "No se encontraron ventas registradas.";

        var sb = new StringBuilder();
        sb.AppendLine("Ventas Recientes:");
        foreach (var v in ventas.OrderByDescending(x => x.Fecha).Take(10))
        {
            sb.AppendLine($"- {v.Fecha:d}: {v.CantidadPollos} pollos a '{v.ClienteNombre}' - Total: S/ {v.Total} (Estado: {v.EstadoPago})");
        }
        return sb.ToString();
    }

    [KernelFunction]
    [Description("Consulta las ventas que tienen saldo pendiente por cobrar (Cuentas por Cobrar).")]
    public async Task<string> ConsultarVentasPendientes()
    {
        var ventas = await _mediator.Send(new ObtenerVentasQuery());
        var pendientes = ventas.Where(v => v.SaldoPendiente > 0).ToList();

        if (!pendientes.Any()) return "No hay ventas con saldo pendiente por cobrar.";

        var sb = new StringBuilder();
        sb.AppendLine("Ventas PENDIENTES de Cobro (Cuentas por Cobrar):");
        var hoy = DateTime.Today;

        foreach (var v in pendientes.OrderBy(x => x.Fecha))
        {
            var dias = (hoy - v.Fecha.Date).TotalDays;
            var alerta = dias > 15 ? "⚠️ ATRASADO" : dias > 7 ? "⏳ PENDIENTE" : "";
            sb.AppendLine($"- [{v.Fecha:dd/MM/yyyy}] {alerta} Cliente: {v.ClienteNombre}. Saldo: S/ {v.SaldoPendiente}. (ID Venta: {v.Id})");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Registra un pago recibido de un cliente por una venta específica.")]
    public async Task<string> RegistrarPagoVenta(
        [Description("ID de la venta a la que se aplica el pago (Guid)")] string ventaId,
        [Description("Monto recibido (debe ser mayor a cero)")] decimal monto,
        [Description("Método de pago (0: Efectivo, 1: Transferencia, 2: Tarjeta, 3: Otros)")] int metodoPago)
    {
        if (monto <= 0) return "Error: El monto del pago debe ser mayor a cero.";

        if (!Guid.TryParse(ventaId, out Guid id))
            return "El ID de la venta no tiene un formato válido.";

        try
        {
            var command = new RegistrarPagoVentaCommand(monto, DateTime.UtcNow, (MetodoPago)metodoPago)
            {
                VentaId = id
            };

            if (_userContext.UsuarioId.HasValue) command.UsuarioId = _userContext.UsuarioId.Value;

            await _mediator.Send(command);
            return $"Se registró exitosamente el pago de S/ {monto} para la venta ID {ventaId}.";
        }
        catch (Exception ex)
        {
            return $"Error al registrar el pago: {ex.Message}";
        }
    }
}
