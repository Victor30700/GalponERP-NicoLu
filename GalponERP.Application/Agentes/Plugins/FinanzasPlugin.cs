using System.ComponentModel;
using GalponERP.Application.Dashboard.Queries;
using GalponERP.Application.Finanzas.Queries.ObtenerFlujoCajaEmpresarial;
using GalponERP.Application.Gastos.Commands.RegistrarGastoOperativo;
using GalponERP.Application.Galpones.Queries.ListarGalpones;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Interfaces;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;
using GalponERP.Domain.Interfaces.Repositories;

namespace GalponERP.Application.Agentes.Plugins;

public class FinanzasPlugin
{
    private readonly IMediator _mediator;
    private readonly IGastoOperativoRepository _gastoRepository;
    private readonly ICurrentUserContext _userContext;

    public FinanzasPlugin(IMediator mediator, IGastoOperativoRepository gastoRepository, ICurrentUserContext userContext)
    {
        _mediator = mediator;
        _gastoRepository = gastoRepository;
        _userContext = userContext;
    }

    [KernelFunction]
    [Description("Obtiene un resumen financiero general del sistema, incluyendo inversión y saldos.")]
    public async Task<string> ObtenerResumenFinanciero()
    {
        var resumen = await _mediator.Send(new ObtenerResumenDashboardQuery());

        var sb = new StringBuilder();
        sb.AppendLine("Resumen Financiero Actual:");
        sb.AppendLine($"- Inversión Total en Curso: S/ {resumen.InversionTotalEnCurso}");
        sb.AppendLine($"- Saldo por Cobrar: S/ {resumen.SaldoPorCobrarTotal}");
        sb.AppendLine($"- Stock de Alimento Valorizado: (Referencial en Dashboard)");
        
        if (resumen.AlertasStockMinimo.Any())
        {
            sb.AppendLine("\n⚠️ ALERTAS DE STOCK:");
            foreach (var alerta in resumen.AlertasStockMinimo)
            {
                sb.AppendLine($"- {alerta.ProductoNombre}: {alerta.StockActual} (Mínimo: {alerta.UmbralMinimo})");
            }
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Consulta el flujo de caja en un rango de fechas. Si no se especifican, usa los últimos 30 días.")]
    public async Task<string> ConsultarFlujoCaja(
        [Description("Fecha de inicio (ISO format)")] string? fechaInicio = null,
        [Description("Fecha de fin (ISO format)")] string? fechaFin = null)
    {
        var fin = string.IsNullOrEmpty(fechaFin) ? DateTime.UtcNow : DateTime.Parse(fechaFin);
        var inicio = string.IsNullOrEmpty(fechaInicio) ? fin.AddDays(-30) : DateTime.Parse(fechaInicio);

        var flujo = await _mediator.Send(new ObtenerFlujoCajaEmpresarialQuery(inicio, fin));

        var sb = new StringBuilder();
        sb.AppendLine($"Flujo de Caja ({inicio:d} al {fin:d}):");
        sb.AppendLine($"- Total Ingresos: S/ {flujo.TotalIngresos}");
        sb.AppendLine($"- Total Egresos: S/ {flujo.TotalEgresos}");
        sb.AppendLine($"- Utilidad Neta: S/ {flujo.UtilidadNeta}");
        sb.AppendLine($"- Cantidad de Ventas: {flujo.Ventas.Count}");
        sb.AppendLine($"- Cantidad de Gastos: {flujo.Gastos.Count}");

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Registra un gasto operativo. Si no se especifican galpón o categoría, el sistema intentará inferirlos.")]
    public async Task<string> RegistrarGastoRapido(
        [Description("Monto del gasto")] decimal monto,
        [Description("Descripción breve del gasto")] string descripcion,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Categoría del gasto (ej. 'Luz', 'Sueldos')")] string? categoriaGasto = null)
    {
        // 1. Resolver Galpón (Regla 7 y 8)
        var galpones = (await _mediator.Send(new ListarGalponesQuery())).Where(g => g.IsActive).ToList();
        if (!galpones.Any()) return "Error: No hay galpones registrados en el sistema.";

        GalponResponse? galponSeleccionado = null;
        if (galpones.Count == 1) 
        {
            galponSeleccionado = galpones.First();
        }
        else if (!string.IsNullOrWhiteSpace(nombreGalpon))
        {
            galponSeleccionado = galpones.FirstOrDefault(g => g.Nombre.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));
        }

        if (galponSeleccionado == null)
        {
            var nombresGalpones = string.Join(", ", galpones.Select(g => g.Nombre));
            if (string.IsNullOrWhiteSpace(nombreGalpon))
            {
                return $"Hay múltiples galpones en el sistema: [{nombresGalpones}]. Pregúntale al usuario a cuál de estos cargar el gasto.";
            }
            return $"No encontré el Galpón '{nombreGalpon}'. Los registros disponibles son: [{nombresGalpones}]. Pregúntale al usuario a cuál de estos se refiere.";
        }

        // 2. Resolver Lote Activo en ese Galpón (Cascaded)
        var lotesActivos = await _mediator.Send(new ListarLotesQuery(SoloActivos: true));
        var loteId = lotesActivos.FirstOrDefault(l => l.GalponId == galponSeleccionado.Id)?.Id;

        // 3. Resolver Categoría (Regla 7 y 8)
        var gastosExistentes = await _gastoRepository.ObtenerTodosAsync();
        var categoriasDisponibles = gastosExistentes.Select(g => g.TipoGasto).Distinct().ToList();
        if (!categoriasDisponibles.Any())
        {
            categoriasDisponibles.AddRange(new[] { "Luz", "Agua", "Sueldos", "Alimento", "Medicamentos", "Mantenimiento", "Otros" });
        }

        string? categoriaSeleccionada = null;
        if (categoriasDisponibles.Count == 1) 
        {
            categoriaSeleccionada = categoriasDisponibles.First();
        }
        else if (!string.IsNullOrWhiteSpace(categoriaGasto))
        {
            categoriaSeleccionada = categoriasDisponibles.FirstOrDefault(c => c.Contains(categoriaGasto, StringComparison.OrdinalIgnoreCase));
        }

        if (categoriaSeleccionada == null)
        {
            var nombresCategorias = string.Join(", ", categoriasDisponibles);
            if (string.IsNullOrWhiteSpace(categoriaGasto))
            {
                return $"Debo registrar el gasto en el {galponSeleccionado.Nombre}, pero hay múltiples categorías: [{nombresCategorias}]. Pregúntale al usuario cuál de estas usar.";
            }
            return $"No encontré la categoría de gasto '{categoriaGasto}'. Los registros disponibles son: [{nombresCategorias}]. Pregúntale al usuario a cuál de estos se refiere.";
        }

        // 4. Ejecutar Comando
        try
        {
            var command = new RegistrarGastoOperativoCommand(
                galponSeleccionado.Id,
                loteId,
                descripcion,
                monto,
                DateTime.UtcNow,
                categoriaSeleccionada);

            if (_userContext.UsuarioId.HasValue)
            {
                command.UsuarioId = _userContext.UsuarioId.Value;
            }

            var result = await _mediator.Send(command);
            return $"Gasto registrado exitosamente. S/ {monto} por '{descripcion}' en '{galponSeleccionado.Nombre}' (Categoría: {categoriaSeleccionada}). ID de operación: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar el gasto: {ex.Message}";
        }
    }
}
