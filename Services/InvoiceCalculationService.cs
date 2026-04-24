using InvoiceGeneratorPro.Models;

namespace InvoiceGeneratorPro.Services;

public static class InvoiceCalculationService
{
    public static void Recalculate(IEnumerable<InvoiceLineModel> lines, decimal vatPercent, out decimal subtotal, out decimal vatAmount, out decimal total)
    {
        subtotal = Math.Round(lines.Sum(l => l.LineTotal), 2, MidpointRounding.AwayFromZero);
        vatAmount = Math.Round(subtotal * (vatPercent / 100m), 2, MidpointRounding.AwayFromZero);
        total = Math.Round(subtotal + vatAmount, 2, MidpointRounding.AwayFromZero);
    }
}
