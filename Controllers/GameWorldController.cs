using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Nodes;
using SpiritKing.Components.Nodes.Collectables;
using SpiritKing.Components.Posessables;
using SpiritKing.Data;
using System;
using System.Collections.Generic;

namespace SpiritKing.Controllers;
public class GameWorldController : Components.Interfaces.IUpdateable, Components.Interfaces.IDrawable
{
    public List<Platform> Platforms { get; set; }

    public List<ICollectable> Collectables { get; set; }
    public PosessablesController PosessableHandler { get; }
    public QuadTree QuadTree { get; set; }

    public GameWorldController(Game game, LevelData levelData)
    {
        QuadTree = new(new(0, 0, levelData.WorldWidth, levelData.WorldHeight), 5, 5);

        Platforms = [];
        foreach (var p in levelData.Platforms)
        {
            Platforms.Add(new Platform(game, 1, p.Position, p.Size));
        }

        Collectables = [];
        foreach (var c in levelData.Collectibles)
        {
            Collectables.Add(c.Type switch
            {
                CollectibleType.HealthItem => new HealthItem(game, c.Position) { HealthValue = c.Value },
                _ => throw new ArgumentException($"Unknown collectible type: {c.Type}")
            });
        }

        Platforms.ForEach(QuadTree.Insert);
        Collectables.ForEach(QuadTree.Insert);
        Posessable.GetCurrentCollisions += QuadTree.FindCollisions;
        Posessable.PickedUpCollectible += RemoveCollectibleFromWorld;
    }

    public int DrawOrder => 1;

    public bool Visible => true;

    public bool Enabled => true;

    public int UpdateOrder => 1;
    public void Dispose() => GC.SuppressFinalize(this);
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Platforms.ForEach(x => x.Draw(gameTime, spriteBatch));
        Collectables.ForEach(x => x.Draw(gameTime, spriteBatch));
    }

    public void Update(GameTime gameTime)
    {
        Collectables.ForEach(x => x.Update(gameTime));
    }

    private void RemoveCollectibleFromWorld(ICollectable collectable)
    {
        _ = QuadTree.Remove(collectable);
        _ = Collectables.Remove(collectable);
    }
}
