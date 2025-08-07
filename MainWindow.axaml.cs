using Avalonia.Controls;
using Avalonia.Styling;

namespace MercuryConverter;

using MercuryConverter.Views;

public partial class MainWindow : Window
{
    public string RunType
    {
        get
        {
            if (Design.IsDesignMode)
            {
                return "In Design!";
            }
            return "In Runtime.";
        }
    }

    public MainWindow()
    {
        InitializeComponent();

        // Force dark mode in designer
        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }
        // LblPlatform.Content = RunType;

        // Setup tab views
        SelectionControl.Content = new Selection();
    }
}