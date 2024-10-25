using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace SpiritKing.Data;
public record SettingsData
{
    public float MasterVolume { get; set; } = 1;
    public float EffectsVolume { get; set; } = 1;
    public float MusicVolume { get; set; } = 1;

    public SaveSlot? CurrentSave { get; set; }

    public List<SaveSlot> SaveSlots { get; set; } = [];

    public SettingsData()
    { }

    public SettingsData(ReadOnlySpan<byte> data)
    {
        try
        {
            var settingsData = JsonSerializer.Deserialize<SettingsData>(data)
                ?? throw new JsonException("Settings data was not able to be written. Deserialization of file returned a null object.");

            CurrentSave = settingsData.CurrentSave;
            SaveSlots = settingsData.SaveSlots;
            MasterVolume = settingsData.MasterVolume;
            EffectsVolume = settingsData.EffectsVolume;
            MusicVolume = settingsData.MusicVolume;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
    public byte[] ToByteArray() => JsonSerializer.SerializeToUtf8Bytes(this);
}
public record SaveSlot(bool IsOccupied, string Name, float Progress, string FileName);
