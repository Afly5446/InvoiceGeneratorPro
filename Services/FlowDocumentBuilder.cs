using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using InvoiceGeneratorPro.Models;

namespace InvoiceGeneratorPro.Services;

/// <summary>Сборка документа WPF (FlowDocument) для предпросмотра и печати.</summary>
public static class FlowDocumentBuilder
{
    public static FlowDocument Build(InvoiceDocumentModel m)
    {
        var accent = (Brush)new SolidColorBrush(Color.FromRgb(79, 70, 229));
        var doc = new FlowDocument
        {
            PagePadding = new Thickness(44),
            ColumnWidth = double.PositiveInfinity,
            FontFamily = new FontFamily("Segoe UI, Segoe UI Variable Display"),
            FontSize = 12,
            Background = Brushes.White
        };

        doc.Blocks.Add(new Paragraph(new Run("Счёт на оплату"))
        {
            FontSize = 24,
            FontWeight = FontWeights.SemiBold,
            Foreground = accent,
            Margin = new Thickness(0, 0, 0, 14)
        });

        if (!string.IsNullOrWhiteSpace(m.RenderedHeaderNote))
        {
            var notePara = new Paragraph();
            foreach (var line in m.RenderedHeaderNote.Replace("\r\n", "\n").Split('\n'))
                notePara.Inlines.Add(new Run(line + Environment.NewLine));
            notePara.Margin = new Thickness(0, 0, 0, 16);
            notePara.Foreground = new SolidColorBrush(Color.FromRgb(55, 65, 81));
            doc.Blocks.Add(notePara);
        }

        doc.Blocks.Add(MetaTable(m));

        var table = LinesTable(m);
        table.Margin = new Thickness(0, 16, 0, 16);
        doc.Blocks.Add(table);

        doc.Blocks.Add(TotalsBlock(m));

        doc.Blocks.Add(new Paragraph(new Run($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}"))
        {
            FontSize = 10,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 24, 0, 0)
        });

        return doc;
    }

    private static Block MetaTable(InvoiceDocumentModel m)
    {
        var t = CreateTable(2);
        AddRow(t, "№ счёта", m.InvoiceNumber);
        AddRow(t, "Дата", m.IssueDate.ToString("dd.MM.yyyy"));
        AddRow(t, "Поставщик", m.SellerName);
        AddRow(t, "Реквизиты поставщика", m.SellerDetails);
        AddRow(t, "Покупатель", m.ClientName);
        AddRow(t, "Адрес покупателя", m.ClientAddress);
        AddRow(t, "ИНН / КПП покупателя", m.ClientTaxId);
        return t;
    }

    private static Table LinesTable(InvoiceDocumentModel m)
    {
        var t = new Table { CellSpacing = 0 };
        t.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) });
        t.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
        t.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
        t.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

        var header = new TableRowGroup();
        var hr = new TableRow { Background = new SolidColorBrush(Color.FromRgb(244, 246, 251)) };
        hr.Cells.Add(HeaderCell("Наименование"));
        hr.Cells.Add(HeaderCell("Кол-во"));
        hr.Cells.Add(HeaderCell("Цена"));
        hr.Cells.Add(HeaderCell("Сумма"));
        header.Rows.Add(hr);
        t.RowGroups.Add(header);

        var body = new TableRowGroup();
        foreach (var line in m.Lines)
        {
            var row = new TableRow();
            row.Cells.Add(BodyCell(line.Description));
            row.Cells.Add(BodyCell(line.Quantity.ToString("0.##")));
            row.Cells.Add(BodyCell(line.UnitPrice.ToString("N2")));
            row.Cells.Add(BodyCell(line.LineTotal.ToString("N2")));
            body.Rows.Add(row);
        }

        t.RowGroups.Add(body);
        return t;
    }

    private static Block TotalsBlock(InvoiceDocumentModel m)
    {
        var t = CreateTable(2);
        AddRow(t, "Подытог без НДС", m.Subtotal.ToString("N2"));
        AddRow(t, $"НДС ({m.VatPercent:0.##}%)", m.VatAmount.ToString("N2"));
        var totalRow = new TableRow();
        totalRow.Cells.Add(new TableCell(new Paragraph(new Run("К оплате")) { FontWeight = FontWeights.Bold })
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(4)
        });
        totalRow.Cells.Add(new TableCell(new Paragraph(new Run(m.Total.ToString("N2"))) { FontWeight = FontWeights.Bold })
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(4),
            TextAlignment = TextAlignment.Right
        });
        t.RowGroups[0].Rows.Add(totalRow);
        return t;
    }

    private static Table CreateTable(int columns)
    {
        var t = new Table { CellSpacing = 0 };
        for (var i = 0; i < columns; i++)
            t.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
        var rg = new TableRowGroup();
        t.RowGroups.Add(rg);
        return t;
    }

    private static void AddRow(Table t, string label, string value)
    {
        var row = new TableRow();
        row.Cells.Add(new TableCell(new Paragraph(new Run(label)) { Foreground = Brushes.DimGray })
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(4)
        });
        row.Cells.Add(new TableCell(new Paragraph(new Run(value)))
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(4)
        });
        t.RowGroups[0].Rows.Add(row);
    }

    private static TableCell HeaderCell(string text) =>
        new(new Paragraph(new Run(text)) { FontWeight = FontWeights.SemiBold, FontSize = 11 })
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(6, 4, 6, 4)
        };

    private static TableCell BodyCell(string text) =>
        new(new Paragraph(new Run(text)))
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(6, 4, 6, 4)
        };
}
