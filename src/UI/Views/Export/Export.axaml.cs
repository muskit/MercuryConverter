using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MercuryConverter.Data;
using MercuryConverter.ExportOperation;
using MercuryConverter.Utility;
using SaturnData.Notation.Serialization;

namespace MercuryConverter.UI.Views;

public enum ExportStatus
{
    NotStarted, Working, Error, Finished, FinishedWithMessages
}

public partial class ExportRow : ObservableObject
{
    private static Dictionary<ExportStatus, IImage?> StatusImgs = new()
    {
        { ExportStatus.NotStarted, null },
        { ExportStatus.Working, new Bitmap(Utils.AssetPath("imgs/status/indeterminate_spinner.png")) },
        { ExportStatus.Error, new Bitmap(Utils.AssetPath("imgs/status/task_error.png")) },
        { ExportStatus.Finished, new Bitmap(Utils.AssetPath("imgs/status/task_complete.png")) },
        { ExportStatus.FinishedWithMessages, new Bitmap(Utils.AssetPath("imgs/status/task_alert.png")) },
    };

    [ObservableProperty]
    private IImage? statusImg = null;
    public required Song Song { get; set; }
    public void SetStatus(ExportStatus status)
    {
        StatusImg = StatusImgs[status];
    }
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

        NumThreads.Bind(TextBox.TextProperty, new Binding(nameof(Settings.ConcurrentExports))
        {
            Source = Settings.I!
        });

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

        BtnExport.IsEnabled =
        (   // ensure we have selections
            Selection.Selections.Count > 0
            || ((bool)RadioExportAll.IsChecked! && Database.Songs.Count > 0)
        ) &&
        (   // ensure audio format is set
            !(bool)RadioShouldAudioConvert.IsChecked || ListSelectAudioConvertFormat.SelectedIndex != -1
        ) &&
        (
            // enabled export worker count is in good range
            int.TryParse(NumThreads.Text, out var thr) && 1 <= thr && thr <= Environment.ProcessorCount
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

        var options = await Dispatcher.UIThread.InvokeAsync(() =>
        {
            AudioFormat audFor;
            if (!(bool)RadioShouldAudioConvert.IsChecked!)
                audFor = AudioFormat.WAV;
            else
                audFor = (AudioFormat)ListSelectAudioConvertFormat.SelectedIndex + 1;

            return new ExportOptions
            {
                ChartFormat = (FormatVersion)ListSelectChartFormat.SelectedIndex,
                AudioFormat = audFor,
                ExcludeVideo = (bool)TickExcludeVideos.IsChecked!,
                SourceSubdir = (bool)TickGroupExports.IsChecked!
            };
        });

        // process each song in parallel (for audio conversion)
        Parallel.ForEach(
            Rows,
            new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Settings.I!.ConcurrentExports) },
            async row =>
            {
                await Dispatcher.UIThread.InvokeAsync(() => row.SetStatus(ExportStatus.Working));
                Exporter.Run(path, row.Song, options);
            }
        );

        UIExportingMode(false);
    }
}