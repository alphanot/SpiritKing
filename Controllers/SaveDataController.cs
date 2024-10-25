using SpiritKing.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritKing.Controllers;
public class SaveDataController()
{
    public string SavesRootPath { get; set; } = "saves/";
    public string? CurrentSaveFileName { get; set; }

    public string SettingsRootPath { get; set; } = "settings/";
    public string SettingsFileName { get; set; } = "settings.dat";

    public TimeOnly? LastLoadTime { get; set; }

    public async Task SaveGameAsync(SaveData data, string? saveName = null)
    {
        CurrentSaveFileName = saveName ?? CurrentSaveFileName;
        if (!Directory.Exists(SavesRootPath))
        {
            Directory.CreateDirectory(SavesRootPath);
        }
        using var fs = File.Create($"{SavesRootPath}{CurrentSaveFileName}");
        Debug.WriteLine($"File Created at {fs.Name}");
        await fs.WriteAsync(data.ToByteArray());
        fs.Close();
    }

    public async Task<SaveData> LoadGameSaveAsync(string saveName)
    {
        CurrentSaveFileName = saveName;
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
        if (!Directory.Exists(SettingsRootPath))
        {
            Directory.CreateDirectory(SettingsRootPath);
        }
        settingsData.SaveSlots = GetSaveFileNames().Select(x => new SaveSlot(true, "name", 0, x)).ToList();

        using var fs = File.Create($"{SettingsRootPath}{SettingsFileName}");

        await fs.WriteAsync(settingsData.ToByteArray());
        fs.Close();
    }

    public async Task<SettingsData> LoadGameSettingsAsync()
    {
        if (File.Exists($"{SettingsRootPath}{SettingsFileName}"))
        {
            var fs = await File.ReadAllBytesAsync($"{SettingsRootPath}{SettingsFileName}");
            LastLoadTime = TimeOnly.FromDateTime(DateTime.Now);
            return new(fs);
        }

        return new();
    }

    public List<string> GetSaveFileNames()
    {
        if (!Directory.Exists(SavesRootPath))
        {
            return [];
        }

        var files = Directory.GetFiles(SavesRootPath);
        return files.Select(x => Path.GetFileName(x) ?? string.Empty)
                    .Where(x => x != string.Empty)
                    .ToList();
    }
}