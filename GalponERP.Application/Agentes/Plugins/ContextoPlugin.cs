using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace GalponERP.Application.Agentes.Plugins;

public class ContextoPlugin
{
    [KernelFunction]
    [Description("Calcula un rango de fechas (Inicio y Fin) a partir de una descripción en lenguaje natural (ej. 'ayer', 'esta semana', 'el mes pasado', 'hace 3 días').")]
    public string ObtenerRangoFecha(
        [Description("Descripción del periodo de tiempo (ej. 'hoy', 'ayer', 'esta semana', 'la semana pasada', 'este mes', 'el mes pasado', 'últimos 7 días')")] 
        string descripcion)
    {
        try
        {
            var hoy = DateTime.Today;
            DateTime inicio;
            DateTime fin = hoy.AddDays(1).AddTicks(-1); // Fin del día de hoy

            descripcion = descripcion.ToLower().Trim();

            switch (descripcion)
            {
                case "hoy":
                    inicio = hoy;
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "ayer":
                    inicio = hoy.AddDays(-1);
                    fin = hoy.AddTicks(-1);
                    break;
                case "esta semana":
                    int diff = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;
                    inicio = hoy.AddDays(-1 * diff);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "la semana pasada":
                    int diffPast = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;
                    inicio = hoy.AddDays(-1 * diffPast - 7);
                    fin = hoy.AddDays(-1 * diffPast).AddTicks(-1);
                    break;
                case "este mes":
                    inicio = new DateTime(hoy.Year, hoy.Month, 1);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "el mes pasado":
                    inicio = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1);
                    fin = new DateTime(hoy.Year, hoy.Month, 1).AddTicks(-1);
                    break;
                case "últimos 7 días":
                    inicio = hoy.AddDays(-7);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "últimos 30 días":
                    inicio = hoy.AddDays(-30);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                default:
                    // Si no se reconoce, por defecto es hoy
                    inicio = hoy;
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
            }

            return $"Rango calculado para '{descripcion}': Desde {inicio:dd/MM/yyyy HH:mm:ss} hasta {fin:dd/MM/yyyy HH:mm:ss}";
        }
        catch (Exception ex)
        {
            return $"Error al obtener el rango de fechas: {ex.Message}";
        }
    }
}
