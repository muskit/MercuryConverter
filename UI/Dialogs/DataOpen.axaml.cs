using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace MercuryConverter.UI.Dialogs;

public partial class DataOpen : Window
{
    public DataOpen()
    {
        InitializeComponent();
        BeginDirSelection();
    }

    private void BeginDirSelection()
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            await Task.Delay(200);
            var dirSelection = await GetTopLevel(this)!.StorageProvider.OpenFolderPickerAsync
            (
                new FolderPickerOpenOptions
                {
                    Title = "Open Data Folder",
                    AllowMultiple = false,
                }
            );

            if (dirSelection.Count <= 0)
            {
                return;
            }

            var d = dirSelection!.First().TryGetLocalPath()!;
        });
    }
}