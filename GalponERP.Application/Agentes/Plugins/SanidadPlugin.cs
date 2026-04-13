using System.ComponentModel;
using GalponERP.Application.Calendario.Queries.ObtenerCalendarioPorLote;
using GalponERP.Application.Calendario.Commands.MarcarVacunaAplicada;
using GalponERP.Application.Calendario.Commands;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using GalponERP.Application.Mortalidad.Queries.ObtenerMortalidadPorLote;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Common;
using GalponERP.Domain.Entities;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class SanidadPlugin
{
    private readonly IMediator _mediator;

    public SanidadPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Lista las actividades sanitarias programadas para un lote específico (pendientes, aplicadas, etc.).")]
    public async Task<string> ListarCalendarioSanitario(
        [Description("Opcional: Nombre del galpón donde está el lote (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para consultar el calendario.";

        var (lote, msg) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Lote Activo");
        if (lote == null) return msg!;

        var actividades = await _mediator.Send(new ObtenerCalendarioPorLoteQuery(lote.Id));

        if (!actividades.Any())
            return $"No hay actividades sanitarias programadas para el lote en '{lote.NombreGalpon}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Calendario Sanitario para el lote en '{lote.NombreGalpon}':");
        
        foreach (var act in actividades.OrderBy(a => a.DiaDeAplicacion))
        {
            var estado = act.Estado == EstadoCalendario.Pendiente ? "⏳ PENDIENTE" : 
                         act.Estado == EstadoCalendario.Aplicado ? "✅ APLICADO" : "❌ CANCELADO";
            
            sb.AppendLine($"- Día {act.DiaDeAplicacion}: {act.DescripcionTratamiento} [{estado}] (ID: {act.Id})");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Registra que una vacuna o tratamiento sanitario ha sido aplicado a un lote.")]
    public async Task<string> RegistrarAplicacionVacuna(
        [Description("Cantidad consumida del producto (ej. dosis o litros)")] decimal cantidadConsumida,
        [Description("Opcional: ID de la actividad del calendario sanitario (Guid)")] string? actividadId = null,
        [Description("Opcional: Nombre de la vacuna o tratamiento para buscar (ej. 'Newcastle')")] string? nombreActividad = null,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        // 1. Resolver Lote
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para registrar aplicaciones.";

        var (lote, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Lote Activo");
        if (lote == null) return msgLote!;

        // 2. Resolver Actividad
        Guid id;
        if (!string.IsNullOrEmpty(actividadId) && Guid.TryParse(actividadId, out id))
        {
            // Ya tenemos el ID
        }
        else
        {
            var actividades = (await _mediator.Send(new ObtenerCalendarioPorLoteQuery(lote.Id)))
                             .Where(a => a.Estado == EstadoCalendario.Pendiente).ToList();
            
            if (!actividades.Any()) return $"No hay actividades pendientes en el calendario del lote en '{lote.NombreGalpon}'.";

            var (actividad, msgAct) = EntityResolver.Resolve(actividades, nombreActividad, a => a.DescripcionTratamiento, "Actividad Pendiente");
            if (actividad == null) return msgAct!;
            id = actividad.Id;
        }

        try
        {
            await _mediator.Send(new MarcarVacunaAplicadaCommand(id, cantidadConsumida));
            return $"Se registró correctamente la aplicación de la actividad en el lote de '{lote.NombreGalpon}'.";
        }
        catch (Exception ex)
        {
            return $"Error al registrar la aplicación: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Programa una nueva actividad sanitaria (vacuna, vitamina, etc.) de forma manual para un lote.")]
    public async Task<string> ProgramarActividadSanitaria(
        [Description("Descripción del tratamiento o vacuna (ej. 'Newcastle Refuerzo')")] string descripcion,
        [Description("Tipo de actividad: Vacuna, Vitaminas, Desinfectante, Antibiotico, Otros")] string tipo,
        [Description("Fecha programada (YYYY-MM-DD)")] string fecha,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null,
        [Description("Opcional: Nombre del producto a utilizar (ej. 'Vacuna Gumboro')")] string? nombreProducto = null)
    {
        // 1. Resolver Lote
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para programar actividades.";

        var (lote, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Lote Activo");
        if (lote == null) return msgLote!;

        // 2. Validar Tipo
        if (!Enum.TryParse<TipoActividad>(tipo, true, out var tipoEnum))
            return $"El tipo '{tipo}' no es válido. Opciones: Vacuna, Vitaminas, Desinfectante, Antibiotico, Otros.";

        // 3. Validar Fecha
        if (!DateTime.TryParse(fecha, out var fechaProg))
            return "La fecha proporcionada no tiene un formato válido (YYYY-MM-DD).";

        // 4. Resolver Producto (Opcional)
        Guid? productoId = null;
        if (!string.IsNullOrWhiteSpace(nombreProducto))
        {
            var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
            var (prod, msgProd) = EntityResolver.Resolve(productos, nombreProducto, p => p.Nombre, "Producto");
            if (prod == null) return msgProd!;
            productoId = prod.Id;
        }

        try
        {
            var command = new AgregarActividadManualCommand(lote.Id, tipoEnum, fechaProg, descripcion, productoId);
            await _mediator.Send(command);
            return $"Actividad '{descripcion}' programada exitosamente para el día {fechaProg:d} en el lote de '{lote.NombreGalpon}'.";
        }
        catch (Exception ex)
        {
            return $"Error al programar la actividad: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra parámetros de bienestar y ambientales (temperatura, humedad, consumo de agua) para un lote.")]
    public async Task<string> RegistrarParametrosBienestar(
        [Description("Opcional: Temperatura en grados Celsius")] double temperatura = 0,
        [Description("Opcional: Humedad relativa en porcentaje")] double humedad = 0,
        [Description("Opcional: Consumo de agua en litros")] double consumoAgua = 0,
        [Description("Opcional: Observaciones adicionales")] string? observaciones = null,
        [Description("Opcional: Fecha del registro (YYYY-MM-DD), por defecto hoy")] string? fecha = null,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        // 1. Resolver Lote
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para registrar parámetros de bienestar.";

        var (lote, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Lote Activo");
        if (lote == null) return msgLote!;

        // 2. Validar Fecha
        var fechaRegistro = DateTime.Today;
        if (!string.IsNullOrWhiteSpace(fecha) && !DateTime.TryParse(fecha, out fechaRegistro))
            return "La fecha proporcionada no tiene un formato válido (YYYY-MM-DD).";

        try
        {
            decimal? t = temperatura != 0 ? (decimal)temperatura : null;
            decimal? h = humedad != 0 ? (decimal)humedad : null;
            decimal? a = consumoAgua != 0 ? (decimal)consumoAgua : null;

            var command = new GalponERP.Application.Sanidad.Commands.RegistrarBienestar.RegistrarBienestarCommand(
                lote.Id, fechaRegistro, t, h, a, observaciones);
            
            await _mediator.Send(command);
            
            var sb = new StringBuilder();
            sb.AppendLine($"Parámetros de bienestar registrados para el lote de '{lote.NombreGalpon}' el {fechaRegistro:d}:");
            if (t.HasValue) sb.AppendLine($"- Temperatura: {t}°C");
            if (h.HasValue) sb.AppendLine($"- Humedad: {h}%");
            if (a.HasValue) sb.AppendLine($"- Consumo de Agua: {a} L");
            if (!string.IsNullOrWhiteSpace(observaciones)) sb.AppendLine($"- Observaciones: {observaciones}");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al registrar parámetros de bienestar: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Analiza el estado de salud del lote basándose en la mortalidad y sugiere revisar el calendario si es necesario.")]
    public async Task<string> AnalizarSaludLote(
        [Description("Opcional: Nombre del galpón donde está el lote (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return "No hay lotes activos para analizar salud.";

        var (lote, msgLote) = EntityResolver.Resolve(lotesActivos, nombreGalpon, l => l.NombreGalpon, "Lote Activo");
        if (lote == null) return msgLote!;

        var mortalidades = await _mediator.Send(new ObtenerMortalidadPorLoteQuery(lote.Id));
        var totalBajas = mortalidades.Sum(m => m.CantidadBajas);
        
        var sb = new StringBuilder();
        sb.AppendLine($"Análisis de Salud para el lote en '{lote.NombreGalpon}':");
        sb.AppendLine($"- Total de bajas registradas: {totalBajas}");

        if (totalBajas > 10) 
        {
            sb.AppendLine("⚠️ ALERTA: Se detecta una mortalidad considerable.");
            sb.AppendLine("Se recomienda revisar el Calendario Sanitario para asegurar que todas las vacunas y tratamientos estén al día.");
            
            var pendientes = (await _mediator.Send(new ObtenerCalendarioPorLoteQuery(lote.Id)))
                             .Where(a => a.Estado == EstadoCalendario.Pendiente);
            
            if (pendientes.Any())
            {
                sb.AppendLine("\nActividades PENDIENTES críticas:");
                foreach (var p in pendientes.Take(3))
                {
                    sb.AppendLine($"- Día {p.DiaDeAplicacion}: {p.DescripcionTratamiento}");
                }
            }
        }
        else
        {
            sb.AppendLine("El estado de salud parece estable. Continúe con el monitoreo regular.");
        }

        return sb.ToString();
    }
}

