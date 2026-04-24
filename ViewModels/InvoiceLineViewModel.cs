using System.ComponentModel;
using System.Runtime.CompilerServices;
using InvoiceGeneratorPro.Models;

namespace InvoiceGeneratorPro.ViewModels;

public sealed class InvoiceLineViewModel : INotifyPropertyChanged
{
    private string _description = "Услуга / товар";
    private decimal _quantity = 1;
    private decimal _unitPrice;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Description
    {
        get => _description;
        set
        {
            if (_description == value) return;
            _description = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (_unitPrice == value) return;
            _unitPrice = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }

    public decimal LineTotal => new InvoiceLineModel
    {
        Description = Description,
        Quantity = Quantity,
        UnitPrice = UnitPrice
    }.LineTotal;

    public InvoiceLineModel ToModel() => new()
    {
        Description = Description.Trim(),
        Quantity = Quantity,
        UnitPrice = UnitPrice
    };

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
