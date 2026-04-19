using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Entities;

namespace GalponERP.Application.Interfaces;

public interface IPdfService
{
    // Métodos de Reportes Operativos (SAVCO)
    byte[] GenerarRegistroIngresoLotePdf(IngresoLoteReportDto datos);
    byte[] GenerarReporteMortalidadPdf(MortalidadReportDto datos);
    byte[] GenerarFichaSemanalPdf(FichaSemanalReportDto datos);
    byte[] GenerarConsumoAlimentoPdf(ConsumoAlimentoReportDto datos);
    byte[] GenerarCalendarioSanitarioPdf(SanidadReportDto datos);
    byte[] GenerarReporteBienestarPdf(BienestarReportDto datos);
    byte[] GenerarControlAguaPdf(ControlAguaReportDto datos);
    byte[] GenerarReporteInventarioPdf(InventarioReportDto datos);
    byte[] GenerarLiquidacionLotePdf(LiquidacionLoteReportDto datos);

    // Mantenimiento de compatibilidad (opcional, se puede remover tras migrar el reporte de cierre actual)
    byte[] GenerarFichaLiquidacionLote(object datos, ConfiguracionSistema? config = null);
}
