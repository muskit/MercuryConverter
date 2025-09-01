using System.Collections.Generic;
using System.IO;
using MercuryConverter.Utility;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryConverter.Data;

/// <summary>
/// Combining SaturnData's Entry & Chart.
/// </summary>
public class Song
{
    public required string Id { get; set; } // Snn-nnn
    public required uint Uid { get; set; } // nnnn
    public required string Name { get; set; }
    public required string Artist { get; set; }
    public required uint Source { get; set; }
    public required string Rubi { get; set; }
    public required string BpmMessage { get; set; }
    public string? Copyright { get; set; } // May have never been used?
    public required int Genre { get; set; }
    public required string? Jacket { get; set; }
    public required float PreviewTime { get; set; }
    public required float PreviewLen { get; set; }
    public string SourceName => Consts.NUM_SOURCE[Source];
    public string FolderName => $"{Utils.RemoveInvalidFileNameChars(Artist)} - {Utils.RemoveInvalidFileNameChars(Name)}";

    /// <summary>
    /// Pairs of level and chart designer.
    /// </summary>
    public (float, string)?[] Levels { get; set; } = { null, null, null, null };
    public required StructPropertyData assetData;
    /**
    ========= Setting Background =========
    By default it's set to BackgroundOption.Auto which reacts to the difficulty:
    Nrm/Hrd/Exp = Standard Reverse (will be replaced by standard saturn bg once i make that)

    For Inferno, always set the background to BackgroundOption.Boss

    For all other diffs:
    Source 1/2 should be BackgroundOption.Version1

    Source 3/4 should be BackgroundOption.Version2

    Source 5/Plus (whatever that is) should be BackgroundOption.Version3


    ========= Entry.Guid format =========
    MERCURY_[SONGID]_[DIFF] (each var is int)
    */
    public IEnumerable<(Entry, Chart)> GetEntryCharts()
    {
        List<(Entry, Chart)> ret = new();

        for (int i = 0; i < 4; ++i)
        {
            var l = Levels[i];
            if (l == null) continue;

            var diff = (Difficulty)i;

            var chartFilePath = Path.Combine(Settings.I!.DataPath, "MusicData", Id, $"{Id}_{Consts.DIFF_FILENAME_APPEND[diff]}.mer");
            var clearThreshold = ((FloatPropertyData)assetData[Consts.DIFF_CLEAR_KEY[diff]]).Value;

            var e = NotationSerializer.ToEntry(chartFilePath, new NotationReadArgs
            {
                InferClearThresholdFromDifficulty = false,
            });
            e.Title = Name;
            e.Reading = Rubi;
            e.Artist = Artist;
            e.BpmMessage = BpmMessage;
            e.PreviewBegin = PreviewTime * 1000f;
            e.PreviewLength = PreviewLen * 1000f;
            e.ClearThreshold = clearThreshold;
            e.Difficulty = diff;
            e.Level = l.Value.Item1;
            e.NotesDesigner = l.Value.Item2;
            e.JacketPath = "jacket.png";

            // TODO: video

            e.Guid = $"MERCURY_{Uid}_0{(int)diff}";

            if (new List<uint> { 1, 2 }.Contains(Source))
            {
                e.Background = BackgroundOption.Version1;
            }
            else if (new List<uint> { 3, 4 }.Contains(Source))
            {
                e.Background = BackgroundOption.Version2;
            }
            else if (Source == 5)
            {
                e.Background = BackgroundOption.Version3;
            }

            var c = NotationSerializer.ToChart(chartFilePath, new NotationReadArgs { });
            ret.Add((e, c));
        }

        return ret;
    }

    public override string ToString()
    {
        return $"[{Id}] {Artist} - {Name}";
    }
}