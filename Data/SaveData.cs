using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace SpiritKing.Data;
public record SaveData
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true
    };

    public Vector2 PlayerPosition { get; set; } = new(0, 0);
    public string LevelFileName { get; set; } = "level1.json";

    public byte[] ToByteArray() => JsonSerializer.SerializeToUtf8Bytes(this, JsonOptions);

    public SaveData()
    { }

    public SaveData(ReadOnlySpan<byte> data)
    {
        try
        {
            var saveData = JsonSerializer.Deserialize<SaveData>(data, JsonOptions)
                ?? throw new JsonException("Save data was not able to be written. Deserialization of file returned a null object.");

            PlayerPosition = saveData.PlayerPosition;
            LevelFileName = saveData.LevelFileName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
