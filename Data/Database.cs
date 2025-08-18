using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Avalonia.Threading;
using MercuryConverter.UI.Views;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization;
using SaturnData.Notation.Serialization.Mer;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryConverter.Data;

public static class Database
{
    public static ObservableCollection<Song> Songs = new();

    public static void SetupNew(string dataPath)
    {
        Dispatcher.UIThread.Invoke(() => Songs.Clear());

        var metadataTablePath = Path.Combine(dataPath, "MusicParameterTable.uasset");
        var metadataAsset = new UAsset(metadataTablePath, EngineVersion.VER_UE4_19);
        var metadataTable = metadataAsset.Exports[0] as DataTableExport;

        foreach (var data in metadataTable!.Table.Data)
        {
            if (data["AssetDirectory"].ToString()!.Contains("S99"))
            {
                continue;
            }

            var previewBegin = ((FloatPropertyData)data["PreviewBeginTime"]).Value;
            var previewLen = ((FloatPropertyData)data["PreviewSeconds"]).Value;

            string? cTxt = data["CopyrightMessage"].ToString();

            var jacketPath = $"{Path.Combine(dataPath, "jackets", data["JacketAssetName"].ToString()!)}.png";

            try
            {
                var song = new Song
                {
                    Id = data["AssetDirectory"].ToString()!,
                    Rubi = data["Rubi"].ToString()!,
                    Name = data["MusicMessage"].ToString()!,
                    Artist = data["ArtistMessage"].ToString()!,
                    Genre = ((IntPropertyData)data["ScoreGenre"]).Value,
                    Source = ((UInt32PropertyData)data["VersionNo"]).Value,
                    PreviewTime = previewBegin,
                    PreviewLen = previewLen,
                    Jacket = File.Exists(jacketPath) ? jacketPath : null,
                    Copyright = (cTxt == "-" || cTxt == "") ? null : cTxt,
                };

                foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
                {
                    // skip non-canon difficulties
                    if (diff == Difficulty.None || diff == Difficulty.WorldsEnd) continue;

                    if (GetDiffPair(dataPath, data, diff) is var pair && pair != null)
                    {
                        song.charts.Add((diff, pair.Value.Item1, pair.Value.Item2));
                    }
                }

                Dispatcher.UIThread.Invoke(() => Songs.Add(song));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Couldn't construct a song!\n{e}");
            }
        }
        Console.WriteLine("finished music table");
    }

    private static (Entry, Chart)? GetDiffPair(string dataPath, StructPropertyData song, Difficulty diff)
    {

        var level = ((FloatPropertyData)song[Consts.DIFF_LVL_KEY[diff]]).Value;
        if (level == 0)
            return null;

        var id = song["AssetDirectory"].ToString()!;
        var chartFilePath = Path.Combine(dataPath, "MusicData", id, $"{id}_{Consts.DIFF_FILENAME_PREPEND[diff]}.mer");
        var clearThreshold = ((FloatPropertyData)song[Consts.DIFF_CLEAR_KEY[diff]]).Value;

        var e = NotationSerializer.ToEntry(chartFilePath, new NotationReadArgs
        {
            InferClearThresholdFromDifficulty = false
        });
        e.ClearThreshold = clearThreshold;
        var c = NotationSerializer.ToChart(chartFilePath, new NotationReadArgs
        {
            InferClearThresholdFromDifficulty = false
        });

        return (e, c);
    }
}