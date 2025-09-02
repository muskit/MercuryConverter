using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using FFMpegCore;
using FFMpegCore.Arguments;
using Instances.Exceptions;
using MercuryConverter.UI;

namespace MercuryConverter.Utility;

public static class Utils
{
    public static string CoreCount => Environment.ProcessorCount.ToString();
    public static async Task<string> BeginDirSelection(string title, string? startDir = null)
    {
        IReadOnlyList<IStorageFolder>? dirSelection = null;

        await Dispatcher.UIThread.Invoke(async () =>
        {
            await Task.Delay(250);
            var tl = TopLevel.GetTopLevel(MainWindow.Instance)!;
            dirSelection = await tl.StorageProvider.OpenFolderPickerAsync
            (
                new FolderPickerOpenOptions
                {
                    Title = title,
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

    /// <summary>
    /// Get an AvaloniaResource asset.
    /// </summary>
    /// <param name="path">Forward-slash (/)-separated path to asset.</param>
    /// <returns></returns>
    public static Stream GetAsset(string path) => AssetLoader.Open(new Uri(Path.Combine("avares://MercuryConverter/Assets/", path)));

    public static string IIDToMusicFilePath(uint id)
    {
        return $"MER_BGM_S{id / 1000:D2}_{id % 1000:D3}";
    }

    private static bool? _ffmpegAvailable = null;
    public static bool FFMpegAvailable
    {
        get
        {
            if (_ffmpegAvailable == null)
            {
                var testVidPath = Path.Combine(
                    AppContext.BaseDirectory,
                    "ic.mp4"
                );

                try
                {
                    FFMpegArguments
                        .FromFileInput(testVidPath)
                        .OutputToFile("-", true, options => options.WithArgument(new ForceFormatArgument("null")))
                        .ProcessSynchronously();

                    _ffmpegAvailable = true;
                }
                catch (InstanceFileNotFoundException)
                {
                    Console.WriteLine($"Could not find FFmpeg on PATH!");
                    _ffmpegAvailable = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something's wrong with FFmpeg!\n{ex.Message}");
                    _ffmpegAvailable = false;
                }

                Console.WriteLine($"FFmpeg available: {_ffmpegAvailable}");
            }
            return (bool)_ffmpegAvailable!;
        }
    }

    public static string RemoveInvalidFileNameChars(string filename)
    {
        return string.Concat(filename.Split(['/', '\\', '\"', '\'']));
    }

    private static string? _version = null;
    public static string VERSION
    {
        get
        {
            if (_version == null)
            {
                using (StreamReader reader = new(GetAsset("version")))
                {
                    _version = reader.ReadToEnd();
                }
                _version = _version.Trim();
            }
            return _version!;
        }
    }
}