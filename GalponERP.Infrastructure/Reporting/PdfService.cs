using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GalponERP.Infrastructure.Reporting;

public class PdfService : IPdfService
{
    static PdfService()
    {
        // QuestPDF License configuration
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerarFichaLiquidacionLote(object datos, ConfiguracionSistema? config = null)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(config?.NombreEmpresa ?? "Pollos NicoLu").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                        col.Item().Text($"NIT: {config?.Nit ?? "0000000-0"}");
                        if (!string.IsNullOrEmpty(config?.Direccion)) col.Item().Text(config.Direccion);
                        if (!string.IsNullOrEmpty(config?.Telefono)) col.Item().Text($"Tel: {config.Telefono}");
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("FICHA DE LIQUIDACIÓN").SemiBold().FontSize(16);
                        col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                {
                    x.Spacing(10);
                    x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    x.Item().Text("Resumen del Lote:").SemiBold().FontSize(12);

                    x.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Concepto
                            columns.RelativeColumn(1); // Valor
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Concepto").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).AlignRight().Text("Valor").SemiBold();
                        });

                        // Attempt to parse dynamic object (datos) properties
                        var dict = new Dictionary<string, string>();
                        if (datos != null)
                        {
                            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(datos))
                            {
                                string name = descriptor.Name;
                                object? value = descriptor.GetValue(datos);
                                dict.Add(name, value?.ToString() ?? "N/A");
                            }
                        }

                        // Pollos
                        table.Cell().PaddingVertical(2).Text("Cantidad Inicial (Aves)");
                        table.Cell().PaddingVertical(2).AlignRight().Text(dict.ContainsKey("CantidadInicial") ? dict["CantidadInicial"] : "-");
                        
                        table.Cell().PaddingVertical(2).Text("Cantidad Final Vendida/Viva");
                        table.Cell().PaddingVertical(2).AlignRight().Text(dict.ContainsKey("CantidadActual") ? dict["CantidadActual"] : "-");

                        table.Cell().PaddingVertical(2).Text("Mortalidad Acumulada");
                        table.Cell().PaddingVertical(2).AlignRight().Text(dict.ContainsKey("MortalidadAcumulada") ? dict["MortalidadAcumulada"] : "-");

                        table.Cell().PaddingVertical(2).Text("% Mortalidad Final");
                        table.Cell().PaddingVertical(2).AlignRight().Text(dict.ContainsKey("PorcentajeMortalidadFinal") ? $"{dict["PorcentajeMortalidadFinal"]}%" : "-");

                        // Eficiencia
                        table.Cell().PaddingVertical(2).Text("Conversión Alimenticia (FCR)");
                        table.Cell().PaddingVertical(2).AlignRight().Text(dict.ContainsKey("FCRFinal") ? dict["FCRFinal"] : "-");

                        // Finanzas
                        table.Cell().PaddingVertical(2).Text("Costo Total Invertido");
                        table.Cell().PaddingVertical(2).AlignRight().Text(dict.ContainsKey("CostoTotal") ? $"{(config?.MonedaPorDefecto ?? "$")} {dict["CostoTotal"]}" : "-");

                        table.Cell().BorderTop(1).PaddingVertical(5).Text("Utilidad Neta Final").SemiBold().FontColor(Colors.Green.Medium);
                        table.Cell().BorderTop(1).PaddingVertical(5).AlignRight().Text(dict.ContainsKey("UtilidadNeta") ? $"{(config?.MonedaPorDefecto ?? "$")} {dict["UtilidadNeta"]}" : "-").SemiBold().FontColor(Colors.Green.Medium);
                    });

                    x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
