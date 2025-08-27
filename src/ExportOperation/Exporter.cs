using System;
using System.Diagnostics.Contracts;
using System.IO;
using MercuryConverter.Data;
using MercuryConverter.UI.Views;

namespace MercuryConverter.ExportOperation;

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
    public static ExportResult Run(string outputPath, Song song)
    {
        var exportPath = Path.Combine(outputPath, song.FolderName);
        Console.WriteLine($"Exporting to {exportPath}...");
        return new ExportResult { status = ExportResult.Status.Failed, message = "Unimplemented" };
    }
}