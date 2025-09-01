using System;
using System.Collections.Generic;
using System.IO;
using FFMpegCore;
using MercuryConverter.Data;
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
        var exportDir = Path.Combine(outputPath, options.SourceSubdir ? song.SourceName : "");
        var exportSongPath = Path.Combine(exportDir, song.FolderName);
        Directory.CreateDirectory(exportSongPath);

        Console.WriteLine($"Exporting {song.Id} to {exportSongPath}");

        var entryCharts = song.GetEntryCharts();
        HashSet<string> finishedAudio = new();
        HashSet<string> finishedMovies = new();
        string prevMovie = "-";

        foreach (var ec in entryCharts)
        {
            /// AUDIO ///
            var audioKey = ec.Item1.AudioPath;
            var audioExportFileName = $"{audioKey}.{options.AudioFormat.ToString().ToLower()}";
            var audioExportPath = Path.Combine(exportSongPath, audioExportFileName);
            if (!finishedAudio.Contains(audioKey) && Database.AudioPaths.ContainsKey(audioKey))
            {
                var audioSourcePath = Database.AudioPaths[audioKey];

                // Copy/convert audio -- TODO
                switch (options.AudioFormat)
                {
                    case AudioFormat.WAV:
                        File.Copy(audioSourcePath, audioExportPath, true);
                        break;
                    default:
                        FFMpegArguments
                            .FromFileInput(audioSourcePath)
                            .OutputToFile(audioExportPath)
                            .ProcessSynchronously();
                        break;
                }
                finishedAudio.Add(audioKey);
            }
            ec.Item1.AudioPath = audioExportFileName;

            /// VIDEO ///
            // mv_... = set video
            // null = use previously set video
            // - = disable video
            if (!options.ExcludeVideo)
            {
                var movieProp = song.assetData[Consts.DIFF_MOVIE_KEY[ec.Item1.Difficulty]];

                string movie;
                if (movieProp.RawValue == null)
                    movie = prevMovie;
                else
                    movie = movieProp.ToString()!;

                if (movie != "-")
                {
                    var vidFileName = $"{movie}.mp4";
                    if (!finishedMovies.Contains(movie))
                    {
                        var vidPath = Path.Combine(Settings.I!.DataPath, "movies", vidFileName);
                        // TODO: check file's existence to avoid program crash
                        File.Copy(vidPath, Path.Combine(exportSongPath, vidFileName), true);

                        finishedMovies.Add(movie);
                    }
                    ec.Item1.VideoPath = vidFileName;
                }
                prevMovie = movie;
            }
            
            /// CHART ///
            var chartExt = 
                options.ChartFormat == FormatVersion.SatV1 ||
                options.ChartFormat == FormatVersion.SatV2 ||
                options.ChartFormat == FormatVersion.SatV3 ? 
                "sat" : "mer";

            NotationSerializer.ToFile(
                Path.Combine(exportSongPath, $"{(int)ec.Item1.Difficulty}.{chartExt}"),
                ec.Item1, ec.Item2,
                new NotationWriteArgs { FormatVersion = options.ChartFormat }
            );
            // restore audio key in db AFTER exporting metadata
            ec.Item1.AudioPath = audioKey;
        }

        /// JACKET ///
        if (song.Jacket != null)
            File.Copy(song.Jacket, Path.Combine(exportSongPath, "jacket.png"), true);

        return new ExportResult { status = ExportResult.Status.Failed, message = "Unimplemented" };
    }
}