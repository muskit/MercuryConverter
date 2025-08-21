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
    public static Settings? I;

    private string iniPath;

    public string AppDataPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "muskit", "MercuryConverter");

    [ObservableProperty]
    private string dataPath = "";
    [ObservableProperty]
    private Theme theme = Theme.System;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        Console.Write($"Setting {e.PropertyName} changed to ");
        switch (e.PropertyName)
        {
            case nameof(DataPath):
                Console.WriteLine(DataPath);
                break;
            default:
                Console.WriteLine("unknown variable");
                break;
        }
        SaveToIni();

        base.OnPropertyChanged(e);
    }

    public Settings()
    {
        I = this;

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
            Console.WriteLine("Attempting to create new settings file.");
            SaveToIni();
        }
    }

    private void SaveToIni()
    {
        var data = new IniData();
        data["paths"]["data"] = DataPath;
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
        if (Enum.TryParse(iniData["ui"]["theme"], out Theme loadedTheme))
            Theme = loadedTheme;
    }
}