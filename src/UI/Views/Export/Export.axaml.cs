using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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
        { ExportStatus.Working, new Bitmap(Utils.GetAsset("imgs/status/indeterminate_spinner.png")) },
        { ExportStatus.Error, new Bitmap(Utils.GetAsset("imgs/status/task_error.png")) },
        { ExportStatus.Finished, new Bitmap(Utils.GetAsset("imgs/status/task_complete.png")) },
        { ExportStatus.FinishedWithMessages, new Bitmap(Utils.GetAsset("imgs/status/task_alert.png")) },
    };

    [ObservableProperty]
    private IImage? statusImg = null;
    public required Song Song { get; set; }
    public ExportStatus Status { set => StatusImg = StatusImgs.GetValueOrDefault(value, null); }
}

public partial class Export : Panel
{
    public ObservableCollection<ExportRow> Rows { get; } = new();

    private bool _exporting = false;
    private bool Exporting
    {
        get => _exporting;
        set
        {
            _exporting = value;
            Dispatcher.UIThread.Invoke(() =>
            {
                BtnExport.IsEnabled = !value;
                ExportOptionsPane.IsEnabled = !value;
                MainWindow.Instance!.TabSelection.IsEnabled = !value;
                ExportSelectionPane.IsEnabled = !value;
            });
        }
    }


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
        if (Exporting) return;

        Rows.Clear();

        if ((bool)RadioExportAll.IsChecked!)
        {
            Database.Songs.ToList().ForEach((s) => Rows.Add(new ExportRow { Song = s }));
        }
        else
        {
            Selection.Selections.ToList().ForEach((s) => Rows.Add(new ExportRow { Song = s }));
        }
    }

    /// <summary>
    /// Modify UI as needed; determine if we are in an exportable state to enable the button.
    /// </summary>
    private void UpdateUIConditions()
    {
        if (Exporting) return;

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

        var ffmpegAvail = Utils.FFMpegAvailable;
        if (!ffmpegAvail)
            RadioLeaveAudioWAV.IsChecked = true;
        RadioLeaveAudioWAV.IsEnabled = ffmpegAvail;
        RadioShouldAudioConvert.IsEnabled = ffmpegAvail;
        NoFFMpegMessage.IsVisible = !ffmpegAvail;
    }

    private async void ExportFlow()
    {
        Exporting = true;
        var path = await Utils.BeginDirSelection("Choose your export path...", Settings.I!.ExportPath);
        if (string.IsNullOrEmpty(path))
        {
            Exporting = false;
            return;
        }
        Settings.I!.ExportPath = path;

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

        // Reset statuses
        Dispatcher.UIThread.Invoke(() => Rows.ToList().ForEach(row => row.Status = ExportStatus.NotStarted));

        // process each song in parallel (for audio conversion)
        // TODO: cancellable?
        await Parallel.ForEachAsync(
            Rows,
            new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Settings.I!.ConcurrentExports) },
            async (row, cancelToken) =>
            {
                if (cancelToken.IsCancellationRequested) return;

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    row.Status = ExportStatus.Working;
                    ListingTable.ScrollIntoView(row, null);
                });
                Exporter.Run(path, row.Song, options);
                await Dispatcher.UIThread.InvokeAsync(() => row.Status = ExportStatus.Finished);
            }
        );

        Exporting = false;
    }
}