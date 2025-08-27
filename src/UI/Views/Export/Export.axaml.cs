using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MercuryConverter.Data;
using MercuryConverter.ExportOperation;
using MercuryConverter.Utility;

namespace MercuryConverter.UI.Views;

public enum ExportStatus
{
    NotStarted, Working, Error, Finished, FinishedWithMessages
}

public partial class ExportRow : ObservableObject
{
    public required Song Song { get; set; }
    public ExportStatus Status { get; set; } = ExportStatus.NotStarted;
    public string? Message { get; set; }
}

public partial class Export : Panel
{
    public ObservableCollection<ExportRow> Rows { get; } = new();

    public Export()
    {
        InitializeComponent();
        DataContext = this;
        RadioShouldAudioConvert.IsCheckedChanged += OnUIChange;
        NumThreads.PropertyChanged += OnUIChange;
        RadioExportAll.IsCheckedChanged += OnExportSelectionChg;
        ListingTable.PointerPressed += OnClick;

        NumThreads.Text = (Environment.ProcessorCount / 2).ToString();

        ToolTip.SetTip(TickGroupExports,
            "Group exported songs into subfolders named after the version they released in. For example:\n" +
            $"\"Exports/{Consts.NUM_SOURCE[5]}/Ado - うっせぇわ\"");

        Task.Run(async () =>
        {
            await Task.Delay(100);
            MainWindow.Instance!.TabControl.SelectionChanged += OnExportSelectionChg;

        });
    }

    private void OnClick(object? sender, RoutedEventArgs e)
    {
        ListingTable.SelectedItems.Clear();
    }

    private void OnUIChange(object? sender, AvaloniaPropertyChangedEventArgs e) => UpdateUIConditions();
    private void OnUIChange(object? sender, RoutedEventArgs args) => UpdateUIConditions();


    private void OnExportSelectionChg(object? sender, RoutedEventArgs args)
    {
        UpdateUIConditions();
        UpdateRows();
    }

    public void OnExportClick()
    {
        Console.WriteLine("export clicked!");
        Task.Run(ExportFlow);
    }

    private void UpdateRows()
    {
        Console.WriteLine("Updating rows!");
        Rows.Clear();

        if ((bool)RadioExportAll.IsChecked!)
        {
            Console.WriteLine("Adding DB songs to rows...");
            Database.Songs.ToList().ForEach((s) => Rows.Add(new ExportRow { Song = s }));
        }
        else
        {
            Console.WriteLine("Adding selections to rows...");
            Selection.Selections.ToList().ForEach((s) => Rows.Add(new ExportRow { Song = s }));
        }
    }

    /// <summary>
    /// Modify UI as needed; determine if we are in an exportable state to enable the button.
    /// </summary>
    private void UpdateUIConditions()
    {
        ListSelectAudioConvertFormat.IsEnabled = (bool)RadioShouldAudioConvert.IsChecked!;

        Console.WriteLine($"input threads: {NumThreads.Text}");
        BtnExport.IsEnabled =
        (   // ensure we have selections
            Selection.Selections.Count > 0
            || ((bool)RadioExportAll.IsChecked! && Database.Songs.Count > 0)
        ) &&
        (   // ensure audio format is set
            !(bool)RadioShouldAudioConvert.IsChecked || ListSelectAudioConvertFormat.SelectedIndex != -1
        ) &&
        (
            int.TryParse(NumThreads.Text, out var thr) && thr <= Environment.ProcessorCount
        );
    }

    private void UIExportingMode(bool isExporting)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            BtnExport.IsEnabled = !isExporting;
            ExportOptionsPane.IsEnabled = !isExporting;
            MainWindow.Instance!.TabSelection.IsEnabled = !isExporting;
            ExportSelectionPane.IsEnabled = !isExporting;
        });
    }

    private async void ExportFlow()
    {
        UIExportingMode(true);

        string path = await Utils.BeginDirSelection("Choose your export path...");
        Console.WriteLine($"Exporting to {path}");

        int i = 0;
        var mtx = new Mutex();

        foreach (var r in Rows)
        {
            Exporter.Run(path, r.Song);
        }

        UIExportingMode(false);
    }
}