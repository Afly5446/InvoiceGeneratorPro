using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using InvoiceGeneratorPro.Models;

namespace InvoiceGeneratorPro.Services;

/// <summary>Фиксированная вёрстка страницы (FixedDocument) — краткая сводка к полному FlowDocument.</summary>
public static class FixedDocumentBuilder
{
    public static FixedDocument BuildSummary(InvoiceDocumentModel m)
    {
        const double w = 793.7;
        const double h = 1122.5;

        var doc = new FixedDocument();

        var page = new FixedPage { Width = w, Height = h, Background = Brushes.White };

        var root = new StackPanel { Margin = new Thickness(48) };
        root.Children.Add(new TextBlock
        {
            Text = "Сводка",
            FontFamily = new FontFamily("Segoe UI, Segoe UI Variable Display"),
            FontSize = 24,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(79, 70, 229)),
            Margin = new Thickness(0, 0, 0, 18)
        });
        root.Children.Add(Meta("Поставщик", m.SellerName));
        root.Children.Add(Meta("Покупатель", m.ClientName));
        root.Children.Add(Meta("№ счёта", m.InvoiceNumber));
        root.Children.Add(Meta("Дата", m.IssueDate.ToString("dd.MM.yyyy")));
        root.Children.Add(Meta("Подытог", m.Subtotal.ToString("N2")));
        root.Children.Add(Meta($"НДС ({m.VatPercent:0.##}%)", m.VatAmount.ToString("N2")));
        root.Children.Add(Meta("К оплате", m.Total.ToString("N2"), FontWeights.Bold, 16));

        page.Children.Add(root);

        var pc = new PageContent();
        ((IAddChild)pc).AddChild(page);
        doc.Pages.Add(pc);
        return doc;
    }

    private static TextBlock Meta(string label, string value, FontWeight weight = default, double size = 13)
    {
        var tb = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10), FontSize = size, FontWeight = weight };
        tb.Inlines.Add(new Run(label + ": ") { Foreground = Brushes.DimGray, FontWeight = FontWeights.SemiBold });
        tb.Inlines.Add(new Run(value));
        return tb;
    }
}
