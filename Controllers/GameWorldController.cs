using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Nodes;
using SpiritKing.Components.Nodes.Collectables;
using SpiritKing.Components.Posessables;
using System;
using System.Collections.Generic;

namespace SpiritKing.Controllers;
public class GameWorldController : Components.Interfaces.IUpdateable, Components.Interfaces.IDrawable
{
    public List<Platform> Platforms { get; set; }

    public List<ICollectable> Collectables { get; set; }
    public PosessablesController PosessableHandler { get; }
    public QuadTree QuadTree { get; set; }

    public GameWorldController(Game game)
    {

        QuadTree = new(new(0, 0, 50000, 50000), 5, 5);

        Platforms =
        [
            new Platform(game, 1, new Vector2(350, 500), new Vector2(600, 50)),
            new Platform(game, 1, new Vector2(250, 300), new Vector2(600, 50)),
            new Platform(game, 1, new Vector2(50, 600), new Vector2(600, 50)),
            new Platform(game, 1, new Vector2(200, 0), new Vector2(100, 100)),
            new Platform(game, 1, new Vector2(0, 100), new Vector2(1000, 100)),
            new Platform(game, 1, new Vector2(1000, 800), new Vector2(1000, 50)),
            new Platform(game, 1, new Vector2(3050, 305), new Vector2(500, 50))
        ];

        Collectables =
        [
            new HealthItem(game, new Vector2(100, 0))
        ];

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
