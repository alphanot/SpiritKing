using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace SpiritKing.Data;
public record SaveData
{
    public Vector2 PlayerPosition { get; set; } = new(0, 0);
    public byte[] ToByteArray() => JsonSerializer.SerializeToUtf8Bytes(this);

    public SaveData()
    { }

    public SaveData(ReadOnlySpan<byte> data)
    {
        try
        {
            var saveData = JsonSerializer.Deserialize<SaveData>(data)
                ?? throw new JsonException("Save data was not able to be written. Deserialization of file returned a null object.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
