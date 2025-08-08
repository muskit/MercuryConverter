using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace MercuryConverter.Data;

public class Song
{
    /// <summary>
    /// Format: `Snn-nnn` where `n` is a digit.
    /// </summary>
    public required string Id { get; set;}
    public required string Name { get; set; }
    public required string Artist { get; set; }
    public required string Source { get; set; }
    public string rubi;
    public string copyright;
    public string tempo;
    public int genreId;
    public string jacket;
    public Chart?[] charts = { null, null, null, null };
}