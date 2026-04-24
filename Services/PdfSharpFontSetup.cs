using PdfSharp.Fonts;

namespace InvoiceGeneratorPro.Services;

/// <summary>Однократная регистрация резолвера шрифтов до любых вызовов <c>XFont</c>.</summary>
public static class PdfSharpFontSetup
{
    private static readonly object Gate = new();
    private static bool _done;

    public static void Ensure()
    {
        if (_done)
            return;

        lock (Gate)
        {
            if (_done)
                return;

            if (GlobalFontSettings.FontResolver is null)
                GlobalFontSettings.FontResolver = new PdfSharpFontResolver();

            _done = true;
        }
    }
}
