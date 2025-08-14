using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MercuryConverter.Data;

public static class Database
{
    public static void Setup(string dataDirPath)
    {
        // Check that path exists
        if (!Directory.Exists(dataDirPath))
        {
            Console.WriteLine($"Folder {dataDirPath} doesn't exist!");
            return;
        }

        // Get metadata.json
        var jPath = Path.Combine(dataDirPath, "metadata.json");
        string jStr;
        JsonElement mdObj;
        try
        {
            jStr = File.ReadAllText(jPath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't read {jPath}: {e}");
            return;
        }
        try
        {
            mdObj = JsonDocument.Parse(jStr).RootElement.GetProperty("Exports")[0].GetProperty("Table").GetProperty("Data");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't parse JSON object: {e}");
            return;
        }

        // TODO: Clear existing structures

        // Parse metadata.json
        foreach (var mdSong in mdObj.EnumerateArray())
        {
            var id = "";
            var title = "";
            var rubi = "";
            var artist = "";
            var genre = -1;
            var copyright = "";
            var bpm = "";
            var version = -1;
            var previewTime = -1;
            var previewLength = -1;
            var jacketPath = "";

            var level = new string?[] { null, null, null, null };
            var levelBGA = new string?[] { null, null, null,  null};
            var levelAudio = new string?[] { null, null, null, null };
            var levelDesigner = new string?[] { null, null, null, null };
            var levelClearRequirements = new string?[] { null, null, null, null };

            foreach (var prop in mdSong.GetProperty("Value").EnumerateArray())
            {
                var value = prop.GetProperty("Value");
                // Console.WriteLine($"{prop.GetProperty("Name")}={prop.GetProperty("Value")}");
                switch (prop.GetProperty("Name").GetString()!)
                {
                    case "AssetDirectory":
                        id = value.GetString()!;
                        break;
                    case "ScoreGenre":
                        genre = value.GetInt16();
                        break;
                    case "MusicMessage":
                        title = value.GetString();
                        break;
                    case "ArtistMessage":
                        artist = value.GetString();
                        break;
                    case "Rubi":
                        rubi = value.GetString();
                        break;
                    case "Bpm":
                        bpm = value.GetString();
                        break;
                    case "CopyrightMessage":
                        var c = value.GetString();
                        if (!new string?[] { "", "-", null }.Contains(c))
                        {
                            copyright = c;
                        }
                        break;
                }
            }

            Console.WriteLine($"[{id}] {artist} - {title}");
        }
    }
}