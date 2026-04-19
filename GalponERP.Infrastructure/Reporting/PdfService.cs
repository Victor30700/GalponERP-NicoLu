using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GalponERP.Infrastructure.Reporting;

public class PdfService : IPdfService
{
    private const string ColorSavcoPrimary = "#1A5276";
    private const string ColorSavcoSecondary = "#D4E6F1";

    static PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Implementación Interfaz IPdfService

    public byte[] GenerarRegistroIngresoLotePdf(IngresoLoteReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Element(CellStyle).Text("Cantidad Inicial");
                        table.Cell().Element(ValueStyle).Text($"{datos.CantidadInicial} aves");

                        table.Cell().Element(CellStyle).Text("Raza / Línea");
                        table.Cell().Element(ValueStyle).Text(datos.Raza);

                        table.Cell().Element(CellStyle).Text("Proveedor");
                        table.Cell().Element(ValueStyle).Text(datos.Proveedor ?? "N/A");

                        table.Cell().Element(CellStyle).Text("Peso Promedio Ingreso");
                        table.Cell().Element(ValueStyle).Text($"{datos.PesoPromedioIngreso} g");
                    });

                    if (!string.IsNullOrEmpty(datos.Observaciones))
                    {
                        col.Item().PaddingTop(10).Column(innerCol =>
                        {
                            innerCol.Item().Text("Observaciones:").SemiBold();
                            innerCol.Item().Text(datos.Observaciones);
                        });
                    }
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerarReporteMortalidadPdf(MortalidadReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(100);
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Fecha");
                            header.Cell().Element(HeaderStyle).Text("Causa");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Bajas");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Acum.");
                        });

                        foreach (var item in datos.Detalles)
                        {
                            table.Cell().Element(RowStyle).Text(item.Fecha.ToShortDateString());
                            table.Cell().Element(RowStyle).Text(item.Causa);
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Cantidad.ToString());
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Acumulado.ToString());
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Text(text =>
                    {
                        text.Span("Total Bajas: ").SemiBold();
                        text.Span(datos.TotalBajas.ToString());
                        text.Span(" | ").FontColor(Colors.Grey.Medium);
                        text.Span("% Mortalidad: ").SemiBold();
                        text.Span($"{datos.PorcentajeMortalidad:F2}%");
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerarFichaSemanalPdf(FichaSemanalReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Semana");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Peso Real (g)");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Estándar (g)");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Desv. (g)");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("% Desv.");
                        });

                        foreach (var item in datos.Semanas)
                        {
                            table.Cell().Element(RowStyle).Text(item.NumeroSemana.ToString());
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PesoRealGramos.ToString("N0"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PesoEstandarGramos.ToString("N0"));
                            
                            var desvStyle = item.Desviacion >= 0 ? Colors.Green.Medium : Colors.Red.Medium;
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Desviacion.ToString("N0")).FontColor(desvStyle);
                            table.Cell().Element(RowStyle).AlignRight().Text($"{item.PorcentajeDesviacion:F1}%").FontColor(desvStyle);
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerarConsumoAlimentoPdf(ConsumoAlimentoReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(100);
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Fecha");
                            header.Cell().Element(HeaderStyle).Text("Producto / Alimento");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Cantidad (Kg)");
                        });

                        foreach (var item in datos.Consumos)
                        {
                            table.Cell().Element(RowStyle).Text(item.Fecha.ToShortDateString());
                            table.Cell().Element(RowStyle).Text(item.Producto);
                            table.Cell().Element(RowStyle).AlignRight().Text(item.CantidadKg.ToString("N2"));
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Column(c => {
                        c.Item().Text($"Total Consumido: {datos.TotalKgConsumidos:N2} Kg").SemiBold();
                        c.Item().Text($"FCR Proyectado: {datos.FCRProyectado:N2}").FontColor(ColorSavcoPrimary).SemiBold();
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerarCalendarioSanitarioPdf(SanidadReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Fecha");
                            header.Cell().Element(HeaderStyle).Text("Actividad");
                            header.Cell().Element(HeaderStyle).Text("Producto");
                            header.Cell().Element(HeaderStyle).Text("Dosis");
                            header.Cell().Element(HeaderStyle).Text("Vía");
                        });

                        foreach (var item in datos.Eventos)
                        {
                            table.Cell().Element(RowStyle).Text(item.Fecha.ToShortDateString());
                            table.Cell().Element(RowStyle).Text(item.Actividad);
                            table.Cell().Element(RowStyle).Text(item.Producto);
                            table.Cell().Element(RowStyle).Text(item.Dosis);
                            table.Cell().Element(RowStyle).Text(item.ViaAplicacion);
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerarReporteBienestarPdf(BienestarReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Parámetros Ambientales
                    col.Item().PaddingBottom(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Element(CellStyle).Text("Temperatura");
                        table.Cell().Element(CellStyle).Text("Humedad");
                        table.Cell().Element(CellStyle).Text("Consumo Agua");

                        table.Cell().Element(ValueStyle).Text(datos.Temperatura.HasValue ? $"{datos.Temperatura:F1} °C" : "N/A");
                        table.Cell().Element(ValueStyle).Text(datos.Humedad.HasValue ? $"{datos.Humedad:F1} %" : "N/A");
                        table.Cell().Element(ValueStyle).Text(datos.ConsumoAgua.HasValue ? $"{datos.ConsumoAgua:F1} L" : "N/A");
                    });

                    // Checklist de Bienestar
                    col.Item().PaddingBottom(5).Text("Checklist de Bienestar (Protocolo Welfare Quality)").SemiBold().FontSize(12).FontColor(ColorSavcoPrimary);
                    
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Concepto Evaluado");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("Resultado");
                        });

                        foreach (var item in datos.Checklist)
                        {
                            table.Cell().Element(RowStyle).Text(item.Concepto);
                            table.Cell().Element(RowStyle).AlignCenter().Text(item.Cumple ? "✔ CUMPLE" : "✘ NO CUMPLE").FontColor(item.Cumple ? Colors.Green.Medium : Colors.Red.Medium).SemiBold();
                        }
                    });

                    if (!string.IsNullOrEmpty(datos.Observaciones))
                    {
                        col.Item().PaddingTop(10).Column(innerCol =>
                        {
                            innerCol.Item().Text("Observaciones Generales:").SemiBold();
                            innerCol.Item().Text(datos.Observaciones);
                        });
                    }
                });
            });
        }).GeneratePdf();
    }
    public byte[] GenerarControlAguaPdf(ControlAguaReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().PaddingBottom(5).Text($"Mes: {datos.MesAnio}").SemiBold().FontSize(12);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(100);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Fecha");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Cloro (ppm)");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("pH");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Temp. (°C)");
                        });

                        foreach (var item in datos.Registros)
                        {
                            table.Cell().Element(RowStyle).Text(item.Fecha.ToShortDateString());
                            table.Cell().Element(RowStyle).AlignRight().Text(item.CloroPpm.ToString("F2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Ph.ToString("F2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Temperatura.ToString("F1"));
                        }
                    });

                    if (!datos.Registros.Any())
                    {
                        col.Item().PaddingTop(20).AlignCenter().Text("No hay registros de agua para el mes seleccionado.").Italic();
                    }
                });
            });
        }).GeneratePdf();
    }
    public byte[] GenerarReporteInventarioPdf(InventarioReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().PaddingBottom(5).Text($"Categoría: {datos.Categoria}").SemiBold().FontSize(12);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Producto");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Stock");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("Und");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("P. Prom.");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Subtotal");
                        });

                        foreach (var item in datos.Productos)
                        {
                            table.Cell().Element(RowStyle).Text(item.Nombre);
                            table.Cell().Element(RowStyle).AlignRight().Text(item.StockActual.ToString("N2"));
                            table.Cell().Element(RowStyle).AlignCenter().Text(item.Unidad);
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PrecioUnitario.ToString("N2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Subtotal.ToString("N2"));
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Text(text =>
                    {
                        text.Span("Valor Total del Inventario: ").SemiBold();
                        text.Span(datos.ValorTotalInventario.ToString("C2")).FontSize(14).FontColor(ColorSavcoPrimary).SemiBold();
                    });

                    if (!datos.Productos.Any())
                    {
                        col.Item().PaddingTop(20).AlignCenter().Text("No hay productos registrados en esta categoría.").Italic();
                    }
                });
            });
        }).GeneratePdf();
    }
    public byte[] GenerarLiquidacionLotePdf(LiquidacionLoteReportDto datos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Resumen de Producción
                    col.Item().PaddingBottom(5).Text("RESUMEN DE PRODUCCIÓN").SemiBold().FontSize(12).FontColor(ColorSavcoPrimary);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Element(CellStyle).Text("Aves Ingresadas");
                        table.Cell().Element(ValueStyle).Text($"{datos.AvesIngresadas} aves");

                        table.Cell().Element(CellStyle).Text("Aves Finales / Ventas");
                        table.Cell().Element(ValueStyle).Text($"{datos.AvesFinales} aves");

                        table.Cell().Element(CellStyle).Text("Total Bajas");
                        table.Cell().Element(ValueStyle).Text($"{datos.TotalBajas} aves ({(datos.PorcentajeMortalidad):F2}%)");
                    });

                    // KPIs Biológicos
                    col.Item().PaddingTop(15).PaddingBottom(5).Text("MÉTRICAS BIOLÓGICAS").SemiBold().FontSize(12).FontColor(ColorSavcoPrimary);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Element(CellStyle).Text("Peso Promedio Final");
                        table.Cell().Element(ValueStyle).Text($"{datos.PesoPromedioFinal:N0} g");

                        table.Cell().Element(CellStyle).Text("Conversión Alimenticia (FCR)");
                        table.Cell().Element(ValueStyle).Text(datos.FCRFinal.ToString("F2"));
                    });

                    // Resumen Financiero
                    col.Item().PaddingTop(15).PaddingBottom(5).Text("RESUMEN FINANCIERO").SemiBold().FontSize(12).FontColor(ColorSavcoPrimary);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Element(CellStyle).Text("Costo Total de Producción");
                        table.Cell().Element(ValueStyle).Text(datos.CostoTotal.ToString("C2"));

                        table.Cell().Element(CellStyle).Text("Ingresos por Ventas");
                        table.Cell().Element(ValueStyle).Text(datos.IngresosVentas.ToString("C2"));

                        table.Cell().Element(CellStyle).Text("Utilidad Neta");
                        table.Cell().Element(ValueStyle).Text(datos.UtilidadNeta.ToString("C2")).FontColor(datos.UtilidadNeta >= 0 ? Colors.Green.Medium : Colors.Red.Medium).SemiBold();
                    });
                });
            });
        }).GeneratePdf();
    }

    // Compatibilidad
    public byte[] GenerarFichaLiquidacionLote(object datos, ConfiguracionSistema? config = null)
    {
        // Redirigir o mantener temporalmente. Por ahora lo dejamos como está para no romper nada.
        return GenerarPdfPlaceholder(new BaseReportDto { TituloReporte = "Ficha Liquidación" }, "FICHA LIQUIDACIÓN (LEGACY)");
    }

    #endregion

    #region Componentes Visuales Reutilizables

    private void ComposeBasePage(PageDescriptor page, BaseReportDto datos)
    {
        page.Size(PageSizes.A4);
        page.Margin(1, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

        page.Header().Element(container => ComposeHeader(container, datos));
        page.Footer().Element(ComposeFooter);
    }

    private void ComposeHeader(IContainer container, BaseReportDto datos)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text(datos.NombreEmpresa).SemiBold().FontSize(18).FontColor(ColorSavcoPrimary);
                col.Item().Text($"NIT: {datos.Nit}");
                if (!string.IsNullOrEmpty(datos.Direccion)) col.Item().Text(datos.Direccion).FontSize(9);
                if (!string.IsNullOrEmpty(datos.Telefono)) col.Item().Text($"Tel: {datos.Telefono}").FontSize(9);
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text(datos.TituloReporte.ToUpper()).SemiBold().FontSize(14);
                col.Item().Text($"Fecha: {datos.FechaGeneracion:dd/MM/yyyy HH:mm}");
                
                if (!string.IsNullOrEmpty(datos.NombreLote))
                {
                    col.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Lote: ").SemiBold();
                        text.Span(datos.NombreLote);
                        text.Span(" | Galpón: ").SemiBold();
                        text.Span(datos.NombreGalpon ?? "-");
                    });
                }
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
        {
            row.RelativeItem().Text("GalponERP - Gestión Avícola Profesional").FontSize(8).FontColor(Colors.Grey.Medium).Italic();
            row.RelativeItem().AlignRight().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        });
    }

    #endregion

    #region Estilos de Celda y Tablas

    private IContainer HeaderStyle(IContainer container)
    {
        return container
            .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
            .PaddingVertical(5)
            .PaddingHorizontal(5)
            .Background(ColorSavcoPrimary);
    }

    private IContainer RowStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten3)
            .PaddingVertical(5)
            .PaddingHorizontal(5);
    }

    private IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .Background(Colors.Grey.Lighten4)
            .PaddingHorizontal(5)
            .DefaultTextStyle(x => x.SemiBold());
    }

    private IContainer ValueStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .PaddingHorizontal(5);
    }

    #endregion

    private byte[] GenerarPdfPlaceholder(BaseReportDto datos, string titulo)
    {
        datos.TituloReporte = titulo;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ComposeBasePage(page, datos);
                page.Content().PaddingVertical(20).AlignCenter().Text($"El reporte de {titulo} está en proceso de diseño detallado.").Italic();
            });
        }).GeneratePdf();
    }
}
