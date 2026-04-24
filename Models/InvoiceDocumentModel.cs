using System.Globalization;

namespace InvoiceGeneratorPro.Models;

/// <summary>Снимок данных счёта для шаблонизатора и экспорта.</summary>
public sealed class InvoiceDocumentModel
{
    public string SellerName { get; set; } = string.Empty;
    public string SellerDetails { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.Today;

    /// <summary>Дата для Handlebars-шаблона.</summary>
    public string IssueDateDisplay => IssueDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU"));

    /// <summary>Итог для подстановки в шаблон.</summary>
    public string TotalDisplay => Total.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));

    public string ClientName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public string ClientTaxId { get; set; } = string.Empty;

    public decimal VatPercent { get; set; } = 20;
    public IReadOnlyList<InvoiceLineModel> Lines { get; set; } = Array.Empty<InvoiceLineModel>();

    public decimal Subtotal { get; set; }
    public decimal VatAmount { get; set; }
    public decimal Total { get; set; }

    /// <summary>Текстовый блок из Handlebars (шапка / примечание).</summary>
    public string RenderedHeaderNote { get; set; } = string.Empty;
}
