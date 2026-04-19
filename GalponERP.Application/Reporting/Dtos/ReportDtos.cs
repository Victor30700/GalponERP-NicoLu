using GalponERP.Domain.Entities;

namespace GalponERP.Application.Reporting.Dtos;

public class BaseReportDto
{
    public string NombreEmpresa { get; set; } = string.Empty;
    public string Nit { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? LogoUrl { get; set; }
    public string TituloReporte { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    
    // Datos del Lote (si aplica)
    public string? NombreLote { get; set; }
    public string? NombreGalpon { get; set; }
    public DateTime? FechaIngresoLote { get; set; }
}

public class IngresoLoteReportDto : BaseReportDto
{
    public int CantidadInicial { get; set; }
    public string Raza { get; set; } = "Cobb 500";
    public string? Proveedor { get; set; }
    public decimal PesoPromedioIngreso { get; set; }
    public string? Observaciones { get; set; }
}

public class MortalidadReportDto : BaseReportDto
{
    public List<MortalidadDetalleDto> Detalles { get; set; } = new();
    public int TotalBajas { get; set; }
    public decimal PorcentajeMortalidad { get; set; }
}

public class MortalidadDetalleDto
{
    public DateTime Fecha { get; set; }
    public int Cantidad { get; set; }
    public string Causa { get; set; } = string.Empty;
    public int Acumulado { get; set; }
}

public class FichaSemanalReportDto : BaseReportDto
{
    public List<SemanaCrecimientoDto> Semanas { get; set; } = new();
}

public class SemanaCrecimientoDto
{
    public int NumeroSemana { get; set; }
    public decimal PesoRealGramos { get; set; }
    public decimal PesoEstandarGramos { get; set; }
    public decimal Desviacion { get; set; }
    public decimal PorcentajeDesviacion { get; set; }
}

public class ConsumoAlimentoReportDto : BaseReportDto
{
    public List<ConsumoDetalleDto> Consumos { get; set; } = new();
    public decimal TotalKgConsumidos { get; set; }
    public decimal FCRProyectado { get; set; }
}

public class ConsumoDetalleDto
{
    public DateTime Fecha { get; set; }
    public string Producto { get; set; } = string.Empty;
    public decimal CantidadKg { get; set; }
    public decimal CostoDia { get; set; }
}

public class SanidadReportDto : BaseReportDto
{
    public List<EventoSanitarioDto> Eventos { get; set; } = new();
}

public class EventoSanitarioDto
{
    public DateTime Fecha { get; set; }
    public string Actividad { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string ViaAplicacion { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
}

public class BienestarReportDto : BaseReportDto
{
    public DateTime FechaRegistro { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? Humedad { get; set; }
    public decimal? ConsumoAgua { get; set; }
    public string? Observaciones { get; set; }
    public List<ItemBienestarDto> Checklist { get; set; } = new();
}

public class ItemBienestarDto
{
    public string Concepto { get; set; } = string.Empty;
    public bool Cumple { get; set; }
    public string? Nota { get; set; }
}

public class ControlAguaReportDto : BaseReportDto
{
    public string MesAnio { get; set; } = string.Empty;
    public List<RegistroAguaDto> Registros { get; set; } = new();
}

public class RegistroAguaDto
{
    public DateTime Fecha { get; set; }
    public decimal CloroPpm { get; set; }
    public decimal Ph { get; set; }
    public decimal Temperatura { get; set; }
}

public class InventarioReportDto : BaseReportDto
{
    public string Categoria { get; set; } = "Todas";
    public List<ProductoStockDto> Productos { get; set; } = new();
    public decimal ValorTotalInventario { get; set; }
}

public class ProductoStockDto
{
    public string Nombre { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => StockActual * PrecioUnitario;
    public DateTime? FechaVencimiento { get; set; }
}

public class LiquidacionLoteReportDto : BaseReportDto
{
    public int AvesIngresadas { get; set; }
    public int AvesFinales { get; set; }
    public int TotalBajas { get; set; }
    public decimal PorcentajeMortalidad { get; set; }
    public decimal PesoPromedioFinal { get; set; }
    public decimal FCRFinal { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal IngresosVentas { get; set; }
    public decimal UtilidadNeta { get; set; }
}
