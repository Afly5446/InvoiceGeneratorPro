using System.IO;
using HandlebarsDotNet;
using InvoiceGeneratorPro.Models;

namespace InvoiceGeneratorPro.Services;

/// <summary>Компиляция и рендер шаблонов Handlebars.Net.</summary>
public sealed class HandlebarsTemplateService
{
    private readonly string _templatesRoot;
    /// <summary>
    /// Для счетов нужен обычный текст, не HTML: иначе кириллица уходит в &#…; сущности.
    /// </summary>
    private readonly IHandlebars _handlebars = Handlebars.Create(new HandlebarsConfiguration
    {
        TextEncoder = null
    });

    public HandlebarsTemplateService()
    {
        _templatesRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
    }

    public string RenderHeaderNote(InvoiceDocumentModel model)
    {
        var path = Path.Combine(_templatesRoot, "invoice_header.hbs");
        if (!File.Exists(path))
            return string.Empty;

        var source = File.ReadAllText(path);
        var compiled = _handlebars.Compile(source);
        return compiled(model);
    }
}
