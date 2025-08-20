using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MercuryConverter.Data;

namespace MercuryConverter.UI.Dialogs;

public partial class DataScanning : UserControl
{
    public static DataScanning? Instance { get; private set; }


    public DataScanning()
    {
        Instance = this;
        InitializeComponent();

        if (!Design.IsDesignMode)
            RunFlow();
    }

    public void RunFlow()
    {
        Task.Run(async () =>
        {
            var path = ""; // TODO: set to current/saved data path (move to config?)
            var selectedPath = await BeginDirSelection();

            if (selectedPath == "") // cancelled opening folder
            {
                // TODO:
                // return and go to completed mode if scan already completed
                // continue if no scan has been completed
                // break if we already have a path but somehow not scanned
                UISetError("No data folder provided.");
                return;
            }
            if (!Directory.Exists(selectedPath))
            {
                UISetError("Folder does not exist.");
                return;
            }
            if (!(File.Exists(Path.Combine(selectedPath, "MusicParameterTable.uasset")) && File.Exists(Path.Combine(selectedPath, "MusicParameterTable.uexp"))))
            {
                UISetError("Missing MusicParameterTable asset files.\nPlease ensure you've set up your data folder properly!");
                return;
            }

            path = selectedPath;

            UIScanningMode(path);
            Database.SetupNew(path);
            UIScanCompletedMode();
        });
    }

    private void UISelectMode()
    {
        UISetError();
        Dispatcher.UIThread.Post(() =>
        {
            ScanStatus.Text = "select your data folder...";
            ScanPath.IsVisible = true;
            ScanInfo.IsVisible = false;
            ButtonGroup.IsVisible = false;
            ProgressAnimation.IsVisible = true;
        });
    }

    private void UIScanningMode(string path)
    {
        UISetError();
        Dispatcher.UIThread.Post(() =>
        {
            ScanStatus.Text = "scanning...";
            ScanPath.IsVisible = true;
            ScanPath.Text = path;
            ScanInfo.IsVisible = false;
            ButtonGroup.IsVisible = false;
            ProgressAnimation.IsVisible = true;
        });
    }

    private void UIScanCompletedMode()
    {
        Dispatcher.UIThread.Post(() =>
        {
            ScanStatus.Text = "scan complete";
            ScanPath.IsVisible = true;
            ScanInfo.IsVisible = true;
            ScanInfoCountText.Text = Database.Songs.Count.ToString();
            ButtonGroup.IsVisible = true;
            ProgressAnimation.IsVisible = false;
        });
    }

    /// <summary>
    /// Use only when a scan is no longer running.
    /// </summary>
    /// <param name="error"></param>
    private void UISetError(string? error = null)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (error == null)
            {
                ScanError.IsVisible = false;
                return;
            }

            ScanError.IsVisible = true;
            ErrorText.Text = error;
            ProgressAnimation.IsVisible = false;
            ButtonGroup.IsVisible = true;
            ScanStatus.Text = "an error has occurred";
        });

    }

    private async Task<string> BeginDirSelection(string? startDir = null)
    {
        IReadOnlyList<IStorageFolder>? dirSelection = null;

        UISelectMode();

        await Dispatcher.UIThread.Invoke(async () =>
        {
            await Task.Delay(250);
            var tl = TopLevel.GetTopLevel(MainWindow.Instance)!;
            dirSelection = await tl.StorageProvider.OpenFolderPickerAsync
            (
                new FolderPickerOpenOptions
                {
                    Title = "Locate Data Folder",
                    AllowMultiple = false,
                    SuggestedStartLocation = startDir == null ? null : await tl.StorageProvider.TryGetFolderFromPathAsync(startDir),
                }
            );
        });

        if (dirSelection!.Count <= 0)
        {
            return "";
        }

        return dirSelection!.First().TryGetLocalPath()!;
    }

    private void CloseHandler(object sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() =>
        {
            MainWindow.Instance!.Dialog.IsOpen = false;
        });
    }

    private void OpenDataHandler(object sender, RoutedEventArgs args)
    {
        RunFlow();
    }
}