using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MercuryConverter.Data;
using MercuryConverter.Utility;

namespace MercuryConverter.UI.Dialogs;

public partial class DataScanning : UserControl
{
    private bool requiresUser;

    public DataScanning(bool requiresUser = true)
    {
        this.requiresUser = requiresUser;
        InitializeComponent();

        if (!Design.IsDesignMode)
            RunFlow();
    }

    public void RunFlow()
    {
        Task.Run(async () =>
        {
            if (Settings.I!.DataPath == "" || requiresUser) // no data path saved
            {
                UISelectMode();
                var selectedPath = await Utils.BeginDirSelection("Locate Data Folder", Settings.I!.DataPath);
                if (selectedPath == "") // cancelled opening folder
                {
                    // TODO:
                    // return and go to completed mode if scan already completed
                    // continue if no scan has been completed
                    // break if we already have a path but somehow not scanned
                    Dispatcher.UIThread.Post(() => ScanPath.Text = "");
                    UISetError("No data folder provided.");
                    return;
                }
                Settings.I.DataPath = selectedPath;
            }

            Dispatcher.UIThread.Post(() => ScanPath.Text = Settings.I.DataPath);
            if (!Directory.Exists(Settings.I.DataPath))
            {
                UISetError("Folder does not exist.");
                return;
            }
            if (!(File.Exists(Path.Combine(Settings.I.DataPath, "MusicParameterTable.uasset"))
                && File.Exists(Path.Combine(Settings.I.DataPath, "MusicParameterTable.uexp"))))
            {
                UISetError("Missing MusicParameterTable asset files.\nPlease ensure you've set up your data folder properly!");
                return;
            }

            UIScanningMode(Settings.I.DataPath);
            Database.SetupNew(Settings.I.DataPath);

            if (requiresUser) // TODO: or if there are warnings
                UIScanCompletedMode();
            else
                Dispatcher.UIThread.Post(() => MainWindow.Instance!.Dialog.IsOpen = false);
        });
    }

    private void UISelectMode()
    {
        UISetError();
        Dispatcher.UIThread.Post(() =>
        {
            ScanStatus.Text = "select your data folder...";
            ScanPath.IsVisible = false;
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

            requiresUser = true;
            ScanError.IsVisible = true;
            ErrorText.Text = error;
            ScanPath.IsVisible = ScanPath.Text == "" ? false : true;
            ScanInfo.IsVisible = false;
            ProgressAnimation.IsVisible = false;
            ButtonGroup.IsVisible = true;
            ScanStatus.Text = "an error has occurred";
        });

    }

    private void CloseHandler(object sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() =>
        {
            MainWindow.Instance!.Dialog.IsOpen = false;
        });
    }

    private void OpenFolderHandler(object sender, RoutedEventArgs args)
    {
        requiresUser = true;
        RunFlow();
    }
}