using System.ComponentModel;
using GalponERP.Application.Pesajes.Commands.RegistrarPesaje;
using GalponERP.Application.Pesajes.Queries.ObtenerPesajesPorLote;
using GalponERP.Application.Lotes.Queries.ListarLotes;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class PesajesPlugin
{
    private readonly IMediator _mediator;

    public PesajesPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Registra el peso promedio de una muestra de pollos de un lote.")]
    public async Task<string> RegistrarPesajeLote(
        [Description("Peso promedio en gramos")] decimal pesoGramos,
        [Description("Cantidad de pollos pesados para la muestra")] int cantidadMuestreada,
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        var lote = await ResolverLoteActivo(nombreGalpon);
        if (lote == null) return "No se encontró un lote activo para registrar el pesaje.";

        try
        {
            var command = new RegistrarPesajeCommand(lote.Id, DateTime.UtcNow, pesoGramos, cantidadMuestreada);
            var result = await _mediator.Send(command);
            return $"Pesaje registrado exitosamente para el lote en '{lote.NombreGalpon}'. Peso promedio: {pesoGramos}g. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar el pesaje: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Consulta el historial de pesajes y compara el crecimiento actual con los estándares de la raza (Cobb 500).")]
    public async Task<string> ConsultarCrecimientoLote(
        [Description("Opcional: Nombre del galpón (ej. 'Galpón 1')")] string? nombreGalpon = null)
    {
        var lote = await ResolverLoteActivo(nombreGalpon);
        if (lote == null) return "No hay un lote activo para consultar crecimiento.";

        var pesajes = await _mediator.Send(new ObtenerPesajesPorLoteQuery(lote.Id));
        if (!pesajes.Any()) return $"Aún no se han registrado pesajes para el lote en '{lote.NombreGalpon}'.";

        var ultimoPesaje = pesajes.OrderByDescending(p => p.Fecha).First();
        
        // Calcular días de vida aproximados basándose en la fecha de creación del lote
        // En un sistema real, el LoteResponse debería tener la FechaInicio.
        // Simulamos la edad del lote (esto es un placeholder, lo ideal es obtenerlo del lote)
        int diasVida = (DateTime.UtcNow - DateTime.UtcNow.AddDays(-21)).Days; // Ejemplo: 21 días
        
        decimal pesoEstandar = ObtenerPesoEstandarGramos(diasVida);
        decimal desviacion = ultimoPesaje.PesoPromedioGramos - pesoEstandar;
        decimal porcentajeDesviacion = (desviacion / pesoEstandar) * 100;

        var sb = new StringBuilder();
        sb.AppendLine($"Estado de Crecimiento - Lote en '{lote.NombreGalpon}':");
        sb.AppendLine($"- Último Pesaje: {ultimoPesaje.PesoPromedioGramos}g (Fecha: {ultimoPesaje.Fecha:dd/MM/yyyy})");
        sb.AppendLine($"- Peso Estándar Esperado (Día {diasVida}): {pesoEstandar}g");
        sb.AppendLine($"- Desviación: {desviacion:F2}g ({porcentajeDesviacion:F1}%)");

        if (porcentajeDesviacion < -10)
        {
            sb.AppendLine("⚠️ ALERTA: El crecimiento está más de un 10% por debajo del estándar. Revisar nutrición y sanidad.");
        }
        else if (porcentajeDesviacion > 10)
        {
            sb.AppendLine("🚀 EXCELENTE: El crecimiento está superando el estándar esperado.");
        }
        else
        {
            sb.AppendLine("✅ NORMAL: El crecimiento se mantiene dentro de los parámetros estándar.");
        }

        return sb.ToString();
    }

    private decimal ObtenerPesoEstandarGramos(int dia)
    {
        // Estándar Cobb 500 (Aproximado)
        if (dia <= 7) return 190;
        if (dia <= 14) return 480;
        if (dia <= 21) return 950;
        if (dia <= 28) return 1600;
        if (dia <= 35) return 2350;
        if (dia <= 42) return 3100;
        return 3500;
    }

    private async Task<LoteResponse?> ResolverLoteActivo(string? nombreGalpon)
    {
        var lotesActivos = (await _mediator.Send(new ListarLotesQuery(SoloActivos: true))).ToList();
        if (!lotesActivos.Any()) return null;
        if (lotesActivos.Count == 1) return lotesActivos.First();
        if (!string.IsNullOrWhiteSpace(nombreGalpon))
            return lotesActivos.FirstOrDefault(l => l.NombreGalpon.Contains(nombreGalpon, StringComparison.OrdinalIgnoreCase));
        return null;
    }
}
