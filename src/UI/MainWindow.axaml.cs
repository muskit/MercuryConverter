using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using MercuryConverter.Data;
using MercuryConverter.UI.Dialogs;
using MercuryConverter.UI.Views;

namespace MercuryConverter.UI;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }
    private bool initialShown = false;

    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        DataContext = this;

        // Force dark mode in designer
        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }

        // Show dialog on startup
        Activated += OnActivated;

        Selection.Selections.CollectionChanged += OnDbSelChanged;
        Database.Songs.CollectionChanged += OnDbSelChanged;
    }

    private void OnActivated(object? sender, EventArgs e)
    {
        if (initialShown) return;
        initialShown = true;

        Dispatcher.UIThread.Post(() =>
        {
            Dialog.IsOpen = true;
            Dialog.DialogContent = Settings.I!.DataPath == "" ?
                new Welcome().Content : new DataScanning(false).Content;
        });
    }

    private void OnDbSelChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Database.Songs.Count > 0)
        {
            TabSelection.Header = $"selection ({Selection.Selections.Count}/{Database.Songs.Count})";
        }
    }

    public void OpenDataHandler()
    {
        Dialog.IsOpen = true;
        Dialog.DialogContent = new DataScanning(true).Content;
    }

    public void OpenDataHOWTO()
    {
        var launcher = GetTopLevel(this)?.Launcher;
        if (launcher != null)
        {
            launcher.LaunchUriAsync(new Uri("https://github.com/muskit/MercuryConverter/blob/main/HOWTO.md"));
        }
    }
}