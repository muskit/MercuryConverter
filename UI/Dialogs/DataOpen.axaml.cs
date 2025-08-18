using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MercuryConverter.Data;
using UAssetAPI;
using UAssetAPI.UnrealTypes;

namespace MercuryConverter.UI.Dialogs;

public partial class DataOpen : UserControl
{
    public static DataOpen? Instance { get; private set; }
    public DataOpen()
    {
        Instance = this;
        InitializeComponent();

        if (!Design.IsDesignMode)
            Run();
    }

    public void Run()
    {
        Task.Run(async () =>
        {
            var path = ""; // TODO: set to current data path

            // Content selection
            while (true)
            {
                var selectedPath = await BeginDirSelection();
                Console.WriteLine($"selectedPath={selectedPath}");
                if (selectedPath != "" && Directory.Exists(selectedPath))
                {
                    path = selectedPath;
                    break;
                }
                // Display error message

            }

            BeginDataScan(path);
        });
    }

    private async Task<string> BeginDirSelection()
    {
        IReadOnlyList<IStorageFolder>? dirSelection = null;

        await Dispatcher.UIThread.Invoke(async () =>
        {
            // Update UI
            ScanView.IsVisible = false;
            SelectView.IsVisible = true;

            await Task.Delay(200);

            dirSelection = await TopLevel.GetTopLevel(MainWindow.Instance)!.StorageProvider.OpenFolderPickerAsync
            (
                new FolderPickerOpenOptions
                {
                    Title = "Locate Data Folder",
                    AllowMultiple = false,
                }
            );
        });

        if (dirSelection!.Count <= 0)
        {
            return "";
        }

        return dirSelection!.First().TryGetLocalPath()!;
    }

    private void BeginDataScan(string dataPath)
    {
        Console.WriteLine(dataPath);
        // Update UI
        Dispatcher.UIThread.Invoke(() =>
        {
            SelectView.IsVisible = false;
            ScanStatus.Text = "scanning...";
            ScanPath.Text = dataPath;
            ScanView.IsVisible = true;
        });

        Database.SetupNew(dataPath);
    }
}