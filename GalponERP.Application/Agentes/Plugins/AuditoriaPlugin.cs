using System.ComponentModel;
using GalponERP.Application.Auditoria.Queries.ObtenerAuditoriaLogs;
using GalponERP.Application.Gastos.Queries.ObtenerGastos;
using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;
using GalponERP.Application.Finanzas.Queries.ValidarConsistenciaFinanciera;
using GalponERP.Application.Common;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class AuditoriaPlugin
{
    private readonly IMediator _mediator;

    public AuditoriaPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Identifica qué usuario registró un gasto operativo específico en el sistema (ej. 'Luz', 'Mantenimiento').")]
    public async Task<string> ConsultarQuienRegistroGasto(
        [Description("Descripción o nombre del gasto a buscar (ej. 'Recibo de luz abril')")] string descripcionGasto)
    {
        try
        {
            var gastos = (await _mediator.Send(new ObtenerGastosQuery(null, null))).ToList();
            if (!gastos.Any()) return "No hay gastos operativos registrados en el sistema.";

            var (gasto, msgGasto) = EntityResolver.Resolve(gastos, descripcionGasto, g => g.Descripcion, "Gasto Operativo");
            if (gasto == null) return msgGasto!;

            var usuarios = await _mediator.Send(new ObtenerUsuariosQuery());
            var usuario = usuarios.FirstOrDefault(u => u.Id == gasto.UsuarioId);

            var nombreResponsable = usuario != null ? $"{usuario.Nombre} ({usuario.Email})" : "Usuario Desconocido";

            return $"El gasto '{gasto.Descripcion}' por un monto de S/ {gasto.Monto.Monto} registrado el {gasto.Fecha:dd/MM/yyyy}, fue ingresado por: {nombreResponsable}.";
        }
        catch (Exception ex)
        {
            return $"Error al consultar quién registró el gasto: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Consulta los logs de auditoría para saber qué acciones se realizaron en el sistema, por quién y cuándo.")]
    public async Task<string> ConsultarLogsAuditoria(
        [Description("Fecha de inicio del rango de búsqueda (YYYY-MM-DD) (opcional)")] string? desde = null,
        [Description("Fecha de fin del rango de búsqueda (YYYY-MM-DD) (opcional)")] string? hasta = null,
        [Description("Nombre de la entidad consultada (ej. 'Producto', 'Lote', 'Venta') (opcional)")] string? entidad = null)
    {
        try
        {
            DateTime? d = null;
            if (!string.IsNullOrEmpty(desde) && DateTime.TryParse(desde, out var dParsed)) d = dParsed;
            
            DateTime? h = null;
            if (!string.IsNullOrEmpty(hasta) && DateTime.TryParse(hasta, out var hParsed)) h = hParsed;

            var logs = await _mediator.Send(new ObtenerAuditoriaLogsQuery(d, h, null, entidad));

            if (!logs.Any())
            {
                return "No se encontraron registros de auditoría para los criterios especificados.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Se encontraron {logs.Count()} registros de auditoría:");
            
            foreach (var log in logs.OrderByDescending(l => l.Fecha))
            {
                sb.AppendLine($"- [{log.Fecha:dd/MM/yyyy HH:mm}] {log.UsuarioNombre} realizó '{log.Accion}' en {log.EntidadNombre} (ID: {log.EntidadId}). Detalle: {log.Detalles}");
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al consultar logs de auditoría: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Busca en el historial de auditoría información específica sobre cambios o eventos.")]
    public async Task<string> BuscarEnHistorial(
        [Description("Palabra clave para buscar en los detalles o acciones (ej. 'precio', 'maíz', 'baja', 'mortalidad')")] string terminoBusqueda,
        [Description("Fecha de inicio (YYYY-MM-DD) (opcional)")] string? desde = null,
        [Description("Fecha de fin (YYYY-MM-DD) (opcional)")] string? hasta = null)
    {
        try
        {
            DateTime? d = null;
            if (!string.IsNullOrEmpty(desde) && DateTime.TryParse(desde, out var dParsed)) d = dParsed;
            
            DateTime? h = null;
            if (!string.IsNullOrEmpty(hasta) && DateTime.TryParse(hasta, out var hParsed)) h = hParsed;

            var logs = await _mediator.Send(new ObtenerAuditoriaLogsQuery(d, h));

            var filtrados = logs.Where(l => 
                l.Accion.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase) || 
                l.Detalles.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                l.EntidadNombre.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(l => l.Fecha)
                .Take(20);

            if (!filtrados.Any())
            {
                return $"No se encontraron eventos en el historial que coincidan con '{terminoBusqueda}'.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Resultados para '{terminoBusqueda}' en el historial:");
            
            foreach (var log in filtrados)
            {
                sb.AppendLine($"- [{log.Fecha:dd/MM/yyyy HH:mm}] {log.UsuarioNombre}: {log.Accion} en {log.EntidadNombre}. {log.Detalles}");
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al buscar en el historial de auditoría: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Realiza una validación de consistencia total entre Inventario, Compras, Ventas y Finanzas para asegurar que no hay errores de datos.")]
    public async Task<string> AuditarConsistenciaIntegral()
    {
        try
        {
            var reporte = await _mediator.Send(new ValidarConsistenciaFinancieraQuery());

            var sb = new StringBuilder();
            sb.AppendLine("--- REPORTE DE CONSISTENCIA INTEGRAL ---");
            sb.AppendLine("1. ABASTECIMIENTO (CxP):");
            sb.AppendLine($"   - Total Compras OC: S/ {reporte.TotalCompras}");
            sb.AppendLine($"   - Total Pagos Realizados: S/ {reporte.TotalPagos}");
            sb.AppendLine($"   - Deuda Pendiente: S/ {reporte.TotalCompras - reporte.TotalPagos}");
            
            sb.AppendLine("\n2. COMERCIALIZACIÓN (CxC):");
            sb.AppendLine($"   - Total Ventas: S/ {reporte.TotalVentas}");
            sb.AppendLine($"   - Total Cobros Recibidos: S/ {reporte.TotalCobros}");
            sb.AppendLine($"   - Saldo por Cobrar: S/ {reporte.TotalVentas - reporte.TotalCobros}");

            sb.AppendLine($"\nESTADO GENERAL: {(reporte.EsConsistente ? "✅ CONSISTENTE" : "⚠️ INCONSISTENCIAS DETECTADAS")}");

            if (!reporte.EsConsistente)
            {
                sb.AppendLine("\nDetalle de Inconsistencias:");
                foreach (var inc in reporte.Inconsistencias)
                {
                    sb.AppendLine($"- {inc}");
                }
            }
            else
            {
                sb.AppendLine("\nNo se encontraron discrepancias entre los registros de operación y contabilidad.");
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al realizar la auditoría de consistencia: {ex.Message}";
        }
    }
}
