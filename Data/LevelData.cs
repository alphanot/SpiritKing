using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpiritKing.Data;

public record LevelData
{
    public string Name { get; set; } = "Unnamed Level";
    public Vector2 PlayerSpawnPosition { get; set; } = new(100, 100);
    public List<PlatformData> Platforms { get; set; } = [];
    public List<EnemyData> Enemies { get; set; } = [];
    public List<CollectibleData> Collectibles { get; set; } = [];
    public int WorldWidth { get; set; } = 50000;
    public int WorldHeight { get; set; } = 50000;
}

public record PlatformData
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EnemyType
{
    Goblin,
    Gargoyle,
    Hound
}

public record EnemyData
{
    public EnemyType Type { get; set; }
    public Vector2 Position { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CollectibleType
{
    HealthItem
}

public record CollectibleData
{
    public CollectibleType Type { get; set; }
    public Vector2 Position { get; set; }
    public int Value { get; set; } = 10;
}
