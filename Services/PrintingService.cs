using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace InvoiceGeneratorPro.Services;

/// <summary>Печать FlowDocument через системный диалог принтера.</summary>
public static class PrintingService
{
    public static bool Print(FlowDocument document, Window owner, string description)
    {
        var clone = CloneDocument(document);
        var pd = new PrintDialog
        {
            PrintTicket = new PrintTicket
            {
                CopyCount = 1,
                OutputColor = OutputColor.Color
            }
        };

        if (pd.ShowDialog() != true)
            return false;

        clone.PageHeight = pd.PrintableAreaHeight;
        clone.PageWidth = pd.PrintableAreaWidth;
        clone.PagePadding = new Thickness(48);
        clone.ColumnWidth = Math.Max(1, pd.PrintableAreaWidth - 96);

        var paginator = ((IDocumentPaginatorSource)clone).DocumentPaginator;
        pd.PrintDocument(paginator, description);
        return true;
    }

    private static FlowDocument CloneDocument(FlowDocument source)
    {
        var xaml = XamlWriter.Save(source);
        return (FlowDocument)XamlReader.Parse(xaml);
    }
}
