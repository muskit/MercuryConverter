using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Threading;
using MercuryConverter.Utility;
using SaturnData.Notation.Core;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

namespace MercuryConverter.Data;

public class FilterOptions
{
    public List<int> categories = [];
    public List<uint> sources = [];

    public override string ToString()
    {
        return $"categories({categories.Count()})=[{string.Join(',', categories.Select(i => i.ToString()))}];sources({sources.Count()})=[{string.Join(',', sources.Select(i => i.ToString()))}]";
    }
}

public static class Database
{
    public static Dictionary<string, string> AudioPaths { get; } = new();
    public static ObservableCollection<Song> Songs { get; } = new();

    public static void SetupNew(string dataPath)
    {
        SetupAudio();
        SetupSongs(dataPath);
    }

    private static void SetupAudio()
    {
        AudioPaths.Clear();

        using (var reader = new StreamReader(Utils.GetAsset("awb.csv")))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                // skip header
                if (line.Contains("songID")) continue;

                var tokens = line.Split(",");
                var id = tokens[0];
                var path = tokens[1];

                if (path.Length <= 0)
                {
                    // TODO: warn of missing audio
                    continue;
                }

                var key = Utils.IIDToMusicFilePath(Convert.ToUInt32(id));

                var audFilePath = path.Split("_");
                var audPath = Path.Combine(Settings.I!.DataPath, "MER_BGM", audFilePath[0], $"{audFilePath[1]}.wav");
                AudioPaths[key] = audPath;
            }
        }
    }

    private static void SetupSongs(string dataPath)
    {
        Dispatcher.UIThread.Invoke(Songs.Clear);

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
                    Uid = ((UInt32PropertyData)data["UniqueID"]).Value,
                    Rubi = data["Rubi"].ToString()!,
                    Name = data["MusicMessage"].ToString()!,
                    Artist = data["ArtistMessage"].ToString()!,
                    Category = ((IntPropertyData)data["ScoreGenre"]).Value,
                    Source = ((UInt32PropertyData)data["VersionNo"]).Value,
                    BpmMessage = data["Bpm"].ToString()!,
                    PreviewTime = previewBegin,
                    PreviewLen = previewLen,
                    Jacket = File.Exists(jacketPath) ? jacketPath : null,
                    Copyright = (cTxt == "-" || cTxt == "") ? null : cTxt,
                    assetData = data,
                };

                foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
                {
                    // skip non-canon difficulties
                    if (diff == Difficulty.None || diff == Difficulty.WorldsEnd) continue;

                    var lvl = ((FloatPropertyData)data[Consts.DIFF_LVL_KEY[diff]]).Value;
                    if (lvl == 0) continue; // skip nonexistent level

                    // check chart existence
                    var chartFilePath = Path.Combine(dataPath, "MusicData", song.Id, $"{song.Id}_{Consts.DIFF_FILENAME_APPEND[diff]}.mer");
                    if (!File.Exists(chartFilePath))
                    {
                        // TODO: add warning message to DataScan
                        Console.WriteLine($"[MISSING CHART] {song.Id} {song.Artist} - {song.Name} / {diff}");
                        continue;
                    }

                    // TODO: check audio existence; add warning but don't skip if missing
                    song.Levels[(int)diff] = (lvl, data[Consts.DIFF_DESIGNER_KEY[diff]].ToString()!);
                }

                Dispatcher.UIThread.Invoke(() => Songs.Add(song));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Couldn't construct a song!\n{e}");
            }
        }
        Console.WriteLine($"Data setup finished with {Songs.Count} songs.");
    }

    public static IEnumerable<Song> SearchAndFilter(string substr, FilterOptions filter)
    {
        /// Filter
        var filtered = new List<Song>();

        // by source
        var sourceSongs = new List<Song>();
        if (filter.sources.Count() == 0)
            sourceSongs.AddRange(Songs);
        else
            sourceSongs.AddRange(Songs.Where(s => filter.sources.Contains(s.Source)));

        // by category
        if (filter.categories.Count() == 0)
            filtered.AddRange(sourceSongs);
        else
            filtered.AddRange(sourceSongs.Where(s => filter.categories.Contains(s.Category)));


        /// Search
        var sanitized = string.IsNullOrEmpty(substr) ? "" : substr.ToLower().Trim();
        if (sanitized == "")
            return filtered;

        return filtered.Where(s =>
        {
            return
                s.Artist.ToLower().Contains(sanitized) ||
                s.Name.ToLower().Contains(sanitized);
        });
    }
}