using SpiritKing.Data;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpiritKing.Controllers;

public static class LevelController
{
    public static string LevelsRootPath { get; set; } = "levels/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true,
        WriteIndented = true
    };

    public static async Task<LevelData> LoadLevelAsync(string fileName)
    {
        var path = Path.Combine(LevelsRootPath, fileName);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Level file not found: {path}");
        }

        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<LevelData>(json, JsonOptions)
            ?? throw new JsonException($"Failed to deserialize level: {path}");
    }
}
