using System;
using System.ComponentModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using IniParser;
using IniParser.Model;

namespace MercuryConverter;

public enum Theme
{
    Light, Dark, System
}

public partial class Settings : ObservableObject
{
    public readonly static Settings I = new();

    private string iniPath;

    public string AppDataPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "muskit", "MercuryConverter");

    [ObservableProperty] private string dataPath = "";
    [ObservableProperty] private string exportPath = "";

    [ObservableProperty] private string concurrentExports = (Environment.ProcessorCount/2).ToString();

    [ObservableProperty] private Theme theme = Theme.System;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        // Console.Write($"Setting {e.PropertyName} changed to ");
        // switch (e.PropertyName)
        // {
        //     case nameof(DataPath):
        //         Console.WriteLine(DataPath);
        //         break;
        //     case nameof(ExportPath):
        //         Console.WriteLine(ExportPath);
        //         break;
        //     case nameof(ConcurrentExports):
        //         Console.WriteLine(ConcurrentExports);
        //         break;
        //     default:
        //         Console.WriteLine("unknown variable");
        //         break;
        // }
        SaveToIni();

        base.OnPropertyChanged(e);
    }

    public Settings()
    {
        Console.WriteLine($"Settings path: {AppDataPath}");
        iniPath = Path.Combine(AppDataPath, "settings.ini");

        // Attempt to read settings; try to create new if unable to
        try
        {
            LoadFromIni();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't read {iniPath}!\n{e.Message}");
            Console.WriteLine("Creating new settings file.");
            SaveToIni();
        }
    }

    private void SaveToIni()
    {
        var data = new IniData();

        data["paths"]["data"] = DataPath;
        data["paths"]["export"] = ExportPath;

        data["export"]["concurrentOperations"] = ConcurrentExports;

        data["ui"]["theme"] = Theme.ToString();

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(iniPath)!);
            FileIniDataParser parser = new();
            parser.WriteFile(iniPath, data);
            Console.WriteLine($"Settings saved to {iniPath}.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not save settings to {iniPath}!\n{e.Message}");
        }
    }

    private void LoadFromIni()
    {
        FileIniDataParser parser = new();
        var iniData = parser.ReadFile(iniPath);

        DataPath = iniData["paths"]["data"];
        ExportPath = iniData["paths"]["export"];
        ConcurrentExports = iniData["export"]["concurrentOperations"];

        if (Enum.TryParse(iniData["ui"]["theme"], out Theme loadedTheme))
            Theme = loadedTheme;
    }
}