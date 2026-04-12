using GalponERP.Application.Interfaces;
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

    public byte[] GenerarFichaLiquidacionLote(object datos)
    {
        // For now, a placeholder PDF until we have the specific DTO
        // We will improve this later when we implement the Query
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text("Ficha de Liquidación de Lote").SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                {
                    x.Spacing(20);
                    x.Item().Text("Este es un reporte preliminar de liquidación de lote.");
                    x.Item().Text($"Datos: {datos.ToString()}");
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }
}
