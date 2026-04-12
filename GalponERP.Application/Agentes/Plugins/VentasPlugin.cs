using System.ComponentModel;
using GalponERP.Application.Ventas.Commands.RegistrarVentaParcial;
using GalponERP.Application.Ventas.Queries.ObtenerVentas;
using GalponERP.Application.Clientes.Queries.ListarClientes;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Interfaces;
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
        [Description("Cantidad de pollos vendidos")] int cantidad,
        [Description("Peso total de los pollos en Kilogramos")] decimal pesoTotalKg,
        [Description("Precio por kilogramo pactado")] decimal precioPorKilo,
        [Description("Opcional: Nombre del galpón del cual se venden (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre del cliente (ej. 'Distribuidora Avícola')")] string? nombreCliente = null)
    {
        // 1. Resolver Lote Activo (Regla 7 y 8)
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "Error: No hay lotes activos para realizar ventas.";

        LoteResponse? loteSeleccionado = null;
        if (lotesActivos.Count == 1) loteSeleccionado = lotesActivos.First();
        else if (!string.IsNullOrWhiteSpace(nombreGalpon))
            loteSeleccionado = lotesActivos.FirstOrDefault(l => l.NombreGalpon.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));

        if (loteSeleccionado == null)
        {
            var nombres = string.Join(", ", lotesActivos.Select(l => l.NombreGalpon));
            return $"Hay múltiples lotes activos en: [{nombres}]. ¿De cuál de ellos se está realizando la venta?";
        }

        // 2. Resolver Cliente (Regla 7 y 8)
        var clientes = (await _mediator.Send(new ListarClientesQuery())).Where(c => c.IsActive).ToList();
        ClienteResponse? clienteSeleccionado = null;

        if (!string.IsNullOrWhiteSpace(nombreCliente))
            clienteSeleccionado = clientes.FirstOrDefault(c => c.Nombre.Contains(nombreCliente, StringComparison.OrdinalIgnoreCase));
        else if (clientes.Count == 1)
            clienteSeleccionado = clientes.First();

        if (clienteSeleccionado == null)
        {
            var nombresClientes = string.Join(", ", clientes.Select(c => c.Nombre));
            if (string.IsNullOrWhiteSpace(nombreCliente))
                return $"Necesito saber a qué cliente se le vende. Clientes disponibles: [{nombresClientes}].";
            return $"No encontré el cliente '{nombreCliente}'. Registros disponibles: [{nombresClientes}].";
        }

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
}
