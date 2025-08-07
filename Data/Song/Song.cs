using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace MercuryConverter.Data;

public class Song
{
    /// <summary>
    /// Format: `Snn-nnn` where `n` is a digit.
    /// </summary>
    public required string id;
    public required string name;
    public required string rubi;
    public required string artist;
    public required string copyright;
    public required string tempo;
    public required string version;
    public required int genreId;
    public required string game;
    public required string jacket;
    public Chart?[] charts = { null, null, null, null };
}