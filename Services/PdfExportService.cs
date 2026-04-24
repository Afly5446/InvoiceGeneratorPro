using System.IO;
using InvoiceGeneratorPro.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace InvoiceGeneratorPro.Services;

/// <summary>Экспорт счёта в PDF через PdfSharp (Arial через <see cref="PdfSharpFontResolver"/>).</summary>
public static class PdfExportService
{
    static PdfExportService() => PdfSharpFontSetup.Ensure();

    public static void Save(InvoiceDocumentModel model, string filePath)
    {
        PdfSharpFontSetup.Ensure();

        var doc = new PdfDocument();
        doc.Info.Title = $"Счёт {model.InvoiceNumber}";

        var page = doc.AddPage();
        page.Size = PdfSharp.PageSize.A4;

        using var gfx = XGraphics.FromPdfPage(page);
        var titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);
        var headerFont = new XFont("Arial", 11, XFontStyleEx.Bold);
        var bodyFont = new XFont("Arial", 10, XFontStyleEx.Regular);
        var smallFont = new XFont("Arial", 9, XFontStyleEx.Regular);

        double y = 40;
        const double left = 40;
        const double width = 515;

        gfx.DrawString("Счёт на оплату", titleFont, XBrushes.Black, left, y);
        y += 28;

        if (!string.IsNullOrWhiteSpace(model.RenderedHeaderNote))
        {
            foreach (var line in model.RenderedHeaderNote.Replace("\r\n", "\n").Split('\n', StringSplitOptions.None))
            {
                gfx.DrawString(line, smallFont, XBrushes.DimGray, new XRect(left, y, width, 20), XStringFormats.TopLeft);
                y += 12;
            }

            y += 8;
        }

        y = DrawMeta(gfx, bodyFont, headerFont, left, y, width, model);
        y += 8;

        y = DrawLinesTable(gfx, headerFont, bodyFont, left, y, width, model);
        y += 12;

        DrawTotals(gfx, bodyFont, headerFont, left, y, width, model);

        var dir = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        doc.Save(filePath);
    }

    private static double DrawMeta(XGraphics gfx, XFont body, XFont label, double x, double y, double w, InvoiceDocumentModel m)
    {
        void Row(string k, string v)
        {
            gfx.DrawString(k, label, XBrushes.DimGray, x, y);
            y += 14;
            gfx.DrawString(v, body, XBrushes.Black, new XRect(x, y, w, 40), XStringFormats.TopLeft);
            y += 18;
        }

        Row("№ счёта", m.InvoiceNumber);
        Row("Дата", m.IssueDate.ToString("dd.MM.yyyy"));
        Row("Поставщик", m.SellerName);
        Row("Реквизиты поставщика", m.SellerDetails);
        Row("Покупатель", m.ClientName);
        Row("Адрес покупателя", m.ClientAddress);
        Row("ИНН / КПП покупателя", m.ClientTaxId);
        return y;
    }

    private static double DrawLinesTable(XGraphics gfx, XFont header, XFont body, double x, double y, double w, InvoiceDocumentModel m)
    {
        gfx.DrawString("Позиции", header, XBrushes.Black, x, y);
        y += 20;

        var cols = new[] { 0.45, 0.15, 0.2, 0.2 };
        var cw = cols.Select(c => w * c).ToArray();
        double cx = x;
        string[] titles = ["Наименование", "Кол-во", "Цена", "Сумма"];
        for (var i = 0; i < titles.Length; i++)
        {
            gfx.DrawString(titles[i], header, XBrushes.Black, cx, y);
            cx += cw[i];
        }

        y += 16;
        gfx.DrawLine(XPens.LightGray, x, y, x + w, y);
        y += 6;

        foreach (var line in m.Lines)
        {
            cx = x;
            var cells = new[] { line.Description, line.Quantity.ToString("0.##"), line.UnitPrice.ToString("N2"), line.LineTotal.ToString("N2") };
            for (var i = 0; i < cells.Length; i++)
            {
                gfx.DrawString(cells[i], body, XBrushes.Black, new XRect(cx, y, cw[i] - 4, 40), XStringFormats.TopLeft);
                cx += cw[i];
            }

            y += 16;
        }

        return y;
    }

    private static void DrawTotals(XGraphics gfx, XFont body, XFont bold, double x, double y, double w, InvoiceDocumentModel m)
    {
        gfx.DrawLine(XPens.Gray, x, y, x + w, y);
        y += 10;
        gfx.DrawString($"Подытог без НДС: {m.Subtotal:N2}", body, XBrushes.Black, x, y);
        y += 16;
        gfx.DrawString($"НДС ({m.VatPercent:0.##}%): {m.VatAmount:N2}", body, XBrushes.Black, x, y);
        y += 18;
        gfx.DrawString($"К оплате: {m.Total:N2}", bold, XBrushes.Black, x, y);
    }
}
