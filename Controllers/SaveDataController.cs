using SpiritKing.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SpiritKing.Controllers;
public class SaveDataController()
{
    public string SavesRootPath { get; set; } = "saves/";
    public string CurrentSaveFileName { get; set; } = "save.dat";

    public string SettingsRootPath { get; set; } = "settings/";
    public string SettingsFileName { get; set; } = "settings.dat";

    public TimeOnly? LastLoadTime { get; set; }

    public async Task SaveGameAsync(SaveData data)
    {
        if (!Directory.Exists(SavesRootPath))
        {
            Directory.CreateDirectory(SavesRootPath);
        }
        using var fs = File.Create($"{SavesRootPath}{CurrentSaveFileName}");
        Debug.WriteLine($"File Created at {fs.Name}");
        await fs.WriteAsync(data.ToByteArray());
        fs.Close();
    }

    public async Task<SaveData> LoadGameSaveAsync()
    {
        if (File.Exists($"{SavesRootPath}{CurrentSaveFileName}"))
        {
            var fs = await File.ReadAllBytesAsync($"{SavesRootPath}{CurrentSaveFileName}");
            LastLoadTime = TimeOnly.FromDateTime(DateTime.Now);
            return new(fs);
        }

        return new();
    }

    public async Task SaveGameSettingsAsync(SettingsData settingsData)
    {
        if (!Directory.Exists(SavesRootPath))
        {
            Directory.CreateDirectory(SavesRootPath);
        }
        using var fs = File.Create($"{SavesRootPath}{CurrentSaveFileName}");
        Debug.WriteLine($"File Created at {fs.Name}");
        await fs.WriteAsync(settingsData.ToByteArray());
        fs.Close();
    }

    public async Task<SettingsData> LoadGameSettingsAsync()
    {
        if (File.Exists($"{SavesRootPath}{CurrentSaveFileName}"))
        {
            var fs = await File.ReadAllBytesAsync($"{SavesRootPath}{CurrentSaveFileName}");
            LastLoadTime = TimeOnly.FromDateTime(DateTime.Now);
            return new(fs);
        }

        return new();
    }
}