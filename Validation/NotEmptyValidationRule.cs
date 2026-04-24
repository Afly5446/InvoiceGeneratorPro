using System.Globalization;
using System.Windows.Controls;

namespace InvoiceGeneratorPro.Validation;

/// <summary>Правило валидации: строка не пустая после Trim.</summary>
public sealed class NotEmptyValidationRule : ValidationRule
{
    public string FieldName { get; set; } = "Поле";

    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        var s = value?.ToString()?.Trim() ?? string.Empty;
        return s.Length > 0
            ? ValidationResult.ValidResult
            : new ValidationResult(false, $"{FieldName} обязательно для заполнения.");
    }
}
