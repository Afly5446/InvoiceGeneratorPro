using System.Globalization;
using System.Windows.Controls;

namespace InvoiceGeneratorPro.Validation;

public sealed class PositiveDecimalValidationRule : ValidationRule
{
    public bool AllowZero { get; set; }

    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return new ValidationResult(false, "Введите число.");

        if (!decimal.TryParse(
                value.ToString(),
                NumberStyles.Number,
                cultureInfo,
                out var d))
            return new ValidationResult(false, "Некорректное число.");

        if (!AllowZero && d <= 0)
            return new ValidationResult(false, "Значение должно быть больше нуля.");

        if (AllowZero && d < 0)
            return new ValidationResult(false, "Значение не может быть отрицательным.");

        return ValidationResult.ValidResult;
    }
}
