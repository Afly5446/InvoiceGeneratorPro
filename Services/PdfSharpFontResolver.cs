using System.IO;
using PdfSharp.Fonts;

namespace InvoiceGeneratorPro.Services;

/// <summary>
/// PdfSharp 6.x: без назначенного <see cref="GlobalFontSettings.FontResolver"/> Arial не находится.
/// Загружаем реальные файлы из каталога шрифтов Windows.
/// </summary>
public sealed class PdfSharpFontResolver : IFontResolver
{
    private static readonly string FontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (!familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            return null;

        var face = (isBold, isItalic) switch
        {
            (true, true) => "arialbi",
            (true, false) => "arialbd",
            (false, true) => "ariali",
            _ => "arial"
        };

        return new FontResolverInfo(face, false, false);
    }

    public byte[]? GetFont(string faceName)
    {
        var file = faceName.ToLowerInvariant() switch
        {
            "arial" => "arial.ttf",
            "arialbd" => "arialbd.ttf",
            "ariali" => "ariali.ttf",
            "arialbi" => "arialbi.ttf",
            _ => null
        };

        if (file is null)
            return null;

        var path = Path.Combine(FontsDir, file);
        return File.Exists(path) ? File.ReadAllBytes(path) : null;
    }
}
