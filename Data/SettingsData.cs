using System;
using System.Diagnostics;
using System.Text.Json;

namespace SpiritKing.Data;
public record SettingsData
{
    public float MasterVolume { get; set; } = 1;
    public float EffectsVolume { get; set; } = 1;
    public float MusicVolume { get; set; } = 1;

    public SettingsData()
    { }

    public SettingsData(ReadOnlySpan<byte> data)
    {
        try
        {
            var settingsData = JsonSerializer.Deserialize<SaveData>(data)
                ?? throw new JsonException("Settings data was not able to be written. Deserialization of file returned a null object.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
    public byte[] ToByteArray() => JsonSerializer.SerializeToUtf8Bytes(this);
}
