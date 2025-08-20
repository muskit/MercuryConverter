using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using SaturnData.Notation.Core;

namespace MercuryConverter.Data;

/// <summary>
/// Combining SaturnData's Entry & Chart.
/// </summary>
public class Song
{
    public required string Id { get; set; } // Snn-nnn
    public required string Name { get; set; }
    public required string Artist { get; set; }
    public required uint Source { get; set; }
    public required string Rubi { get; set; }
    public string? Copyright { get; set; } // May have never been used?
    public required int Genre { get; set; }
    public required string? Jacket { get; set; }
    public required float PreviewTime { get; set; }
    public required float PreviewLen { get; set; }
    public string SourceName => Consts.NUM_SOURCE[Source];
    public (float, string)?[] Levels { get; set; } = { null, null, null, null };
    

    // TODO: For SaturnData.Entry instances, use this Guid format:
    // MERCURY_[SONGID]_[DIFF] (each var is int)
}