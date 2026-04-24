using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using InvoiceGeneratorPro.ViewModels;

namespace InvoiceGeneratorPro;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        ApplyWindowIcon();
    }

    /// <summary>Иконка заголовка окна из <c>AppIcon.ico</c> (копируется в выходную папку при сборке).</summary>
    private void ApplyWindowIcon()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "AppIcon.ico");
            if (!File.Exists(path))
                return;

            Icon = BitmapFrame.Create(new Uri(path, UriKind.Absolute));
        }
        catch
        {
            // дизайн-тайм / запуск без копии иконки
        }
    }
}
