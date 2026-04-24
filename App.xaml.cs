using System.Windows;
using InvoiceGeneratorPro.Services;

namespace InvoiceGeneratorPro;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        PdfSharpFontSetup.Ensure();
        base.OnStartup(e);
    }
}
