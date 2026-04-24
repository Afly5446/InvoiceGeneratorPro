using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using InvoiceGeneratorPro.Helpers;
using InvoiceGeneratorPro.Models;
using InvoiceGeneratorPro.Services;
using Microsoft.Win32;

namespace InvoiceGeneratorPro.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
{
    private readonly HandlebarsTemplateService _templates = new();
    private string _sellerName = "ООО «Пример»";
    private string _sellerDetails = "ИНН 7700000000, р/с 40702810…, Банк…";
    private string _invoiceNumber = "INV-2026-001";
    private DateTime? _issueDate = DateTime.Today;
    private string _clientName = string.Empty;
    private string _clientAddress = string.Empty;
    private string _clientTaxId = string.Empty;
    private string _vatPercentText = "20";
    private FlowDocument? _previewDocument;
    private FixedDocument? _fixedPreviewDocument;

    public MainViewModel()
    {
        Lines = new ObservableCollection<InvoiceLineViewModel>();
        Lines.CollectionChanged += Lines_CollectionChanged;
        AddLine();

        AddLineCommand = new RelayCommand(_ => AddLine());
        RemoveLineCommand = new RelayCommand(
            p => RemoveLine(p as InvoiceLineViewModel),
            p => p is InvoiceLineViewModel && Lines.Count > 1);

        RefreshPreviewCommand = new RelayCommand(_ => RefreshPreview());
        PrintCommand = new RelayCommand(_ => Print(), _ => PreviewDocument is not null && !HasBlockingErrors);
        ExportPdfCommand = new RelayCommand(_ => ExportPdf(), _ => PreviewDocument is not null && !HasBlockingErrors);

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(ClientName) or nameof(InvoiceNumber) or nameof(SellerName) or nameof(VatPercentText))
                CommandManager.InvalidateRequerySuggested();
        };

        RefreshPreview();
    }

    public ObservableCollection<InvoiceLineViewModel> Lines { get; }

    public RelayCommand AddLineCommand { get; }
    public RelayCommand RemoveLineCommand { get; }
    public RelayCommand RefreshPreviewCommand { get; }
    public RelayCommand PrintCommand { get; }
    public RelayCommand ExportPdfCommand { get; }

    public string SellerName
    {
        get => _sellerName;
        set { if (_sellerName == value) return; _sellerName = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public string SellerDetails
    {
        get => _sellerDetails;
        set { if (_sellerDetails == value) return; _sellerDetails = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public string InvoiceNumber
    {
        get => _invoiceNumber;
        set { if (_invoiceNumber == value) return; _invoiceNumber = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public DateTime? IssueDate
    {
        get => _issueDate;
        set { if (_issueDate == value) return; _issueDate = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public string ClientName
    {
        get => _clientName;
        set { if (_clientName == value) return; _clientName = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public string ClientAddress
    {
        get => _clientAddress;
        set { if (_clientAddress == value) return; _clientAddress = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public string ClientTaxId
    {
        get => _clientTaxId;
        set { if (_clientTaxId == value) return; _clientTaxId = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public string VatPercentText
    {
        get => _vatPercentText;
        set { if (_vatPercentText == value) return; _vatPercentText = value; OnPropertyChanged(); RefreshPreview(); }
    }

    public FlowDocument? PreviewDocument
    {
        get => _previewDocument;
        private set { if (ReferenceEquals(_previewDocument, value)) return; _previewDocument = value; OnPropertyChanged(); }
    }

    public FixedDocument? FixedPreviewDocument
    {
        get => _fixedPreviewDocument;
        private set { if (ReferenceEquals(_fixedPreviewDocument, value)) return; _fixedPreviewDocument = value; OnPropertyChanged(); }
    }

    public string Error => string.Empty;

    public string this[string columnName] => columnName switch
    {
        nameof(ClientName) when string.IsNullOrWhiteSpace(ClientName) => "Укажите наименование клиента.",
        nameof(InvoiceNumber) when string.IsNullOrWhiteSpace(InvoiceNumber) => "Укажите номер счёта.",
        nameof(SellerName) when string.IsNullOrWhiteSpace(SellerName) => "Укажите наименование поставщика.",
        nameof(VatPercentText) when !TryParseVat(VatPercentText, out _) => "НДС должен быть числом от 0 до 100.",
        _ => string.Empty
    };

    private bool HasBlockingErrors =>
        !string.IsNullOrEmpty(this[nameof(ClientName)]) ||
        !string.IsNullOrEmpty(this[nameof(InvoiceNumber)]) ||
        !string.IsNullOrEmpty(this[nameof(SellerName)]) ||
        !string.IsNullOrEmpty(this[nameof(VatPercentText)]);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Lines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (InvoiceLineViewModel old in e.OldItems)
                old.PropertyChanged -= Line_PropertyChanged;
        }

        if (e.NewItems is not null)
        {
            foreach (InvoiceLineViewModel line in e.NewItems)
                line.PropertyChanged += Line_PropertyChanged;
        }

        CommandManager.InvalidateRequerySuggested();
        RefreshPreview();
    }

    private void Line_PropertyChanged(object? sender, PropertyChangedEventArgs e) => RefreshPreview();

    private void AddLine() => Lines.Add(new InvoiceLineViewModel());

    private void RemoveLine(InvoiceLineViewModel? line)
    {
        if (line is null || Lines.Count <= 1) return;
        Lines.Remove(line);
    }

    public void RefreshPreview()
    {
        var model = BuildDocumentModel();
        model.RenderedHeaderNote = _templates.RenderHeaderNote(model);
        PreviewDocument = FlowDocumentBuilder.Build(model);
        FixedPreviewDocument = FixedDocumentBuilder.BuildSummary(model);
        CommandManager.InvalidateRequerySuggested();
    }

    private InvoiceDocumentModel BuildDocumentModel()
    {
        var vat = ParseVat();
        var lineModels = Lines.Select(l => l.ToModel()).ToList();
        InvoiceCalculationService.Recalculate(lineModels, vat, out var subtotal, out var vatAmount, out var total);

        return new InvoiceDocumentModel
        {
            SellerName = SellerName.Trim(),
            SellerDetails = SellerDetails.Trim(),
            InvoiceNumber = InvoiceNumber.Trim(),
            IssueDate = IssueDate ?? DateTime.Today,
            ClientName = ClientName.Trim(),
            ClientAddress = ClientAddress.Trim(),
            ClientTaxId = ClientTaxId.Trim(),
            VatPercent = vat,
            Lines = lineModels,
            Subtotal = subtotal,
            VatAmount = vatAmount,
            Total = total
        };
    }

    private static bool TryParseVat(string text, out decimal v)
    {
        v = 0;
        if (!decimal.TryParse(text.Replace(',', '.'), NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
            return false;
        if (parsed < 0 || parsed > 100)
            return false;
        v = parsed;
        return true;
    }

    private decimal ParseVat() => TryParseVat(VatPercentText, out var v) ? v : 0;

    private void Print()
    {
        if (PreviewDocument is null) return;
        var owner = Application.Current?.MainWindow;
        PrintingService.Print(PreviewDocument, owner!, $"Счёт {InvoiceNumber}");
    }

    private void ExportPdf()
    {
        if (PreviewDocument is null) return;

        var dlg = new SaveFileDialog
        {
            Filter = "PDF (*.pdf)|*.pdf",
            FileName = $"{InvoiceNumber.Trim()}.pdf"
        };

        if (dlg.ShowDialog() != true) return;

        try
        {
            var model = BuildDocumentModel();
            model.RenderedHeaderNote = _templates.RenderHeaderNote(model);
            PdfExportService.Save(model, dlg.FileName);
            MessageBox.Show($"Файл сохранён:\n{dlg.FileName}", "Экспорт PDF", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка PDF", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
