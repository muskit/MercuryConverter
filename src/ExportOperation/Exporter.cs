using System;
using System.Diagnostics.Contracts;
using System.IO;
using MercuryConverter.Data;
using MercuryConverter.UI.Views;
using SaturnData.Notation.Serialization;

namespace MercuryConverter.ExportOperation;

public enum AudioFormat { WAV, MP3, OGG }

public class ExportOptions
{

    public required FormatVersion ChartFormat;
    public required AudioFormat AudioFormat;
    public required bool ExcludeVideo;
    public required bool SourceSubdir;
}

public class ExportResult
{
    public enum Status
    {
        Successful, Failed, WithWarnings
    }
    public required Status status;
    /// <summary>
    /// Populated when status is Failed or WithWarnings.
    /// </summary>
    public string? message;
}

public class Exporter
{
    public static ExportResult Run(string outputPath, Song song, ExportOptions options)
    {
        var exportPath = Path.Combine(outputPath, options.SourceSubdir ? song.SourceName : "", song.FolderName);

        Console.WriteLine($"Exporting {song.Id} to {exportPath}");
        return new ExportResult { status = ExportResult.Status.Failed, message = "Unimplemented" };
    }
}