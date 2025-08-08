using Avalonia.Controls;
using Avalonia.Styling;

namespace MercuryConverter;

using MercuryConverter.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Force dark mode in designer
        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }

        // Setup tab views
        SelectionControl.Content = new Selection();
    }
}