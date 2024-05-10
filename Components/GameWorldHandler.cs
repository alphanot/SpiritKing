using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Nodes;
using System;
using System.Collections.Generic;

namespace SpiritKing.Components;
public class GameWorldHandler : Interfaces.IUpdateable, Interfaces.IDrawable
{
    readonly Game Game;

    public List<Platform> Platforms { get; set; }

    public GameWorldHandler(Game game)
    {
        Game = game;
        Platforms = new()
        {
            new Platform(game, 1, new Vector2(350, 500), new Vector2(600, 50)),
            new Platform(game, 1, new Vector2(250, 300), new Vector2(600, 50)),
            new Platform(game, 1, new Vector2(50, 600), new Vector2(600, 50)),
            //new Platform(game, 1, new Vector2(200, 0), new Vector2(100, 100)),
            new Platform(game, 1, new Vector2(0, 100), new Vector2(1000, 100)),
            new Platform(game, 1, new Vector2(1000, 800), new Vector2(1000, 50)),
            new Platform(game, 1, new Vector2(3050, 305), new Vector2(500, 50))
        };
    }

    public int DrawOrder => 1;

    public bool Visible => true;

    public bool Enabled => true;

    public int UpdateOrder => 1;
    public void Dispose() => throw new NotImplementedException();
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var platform in Platforms)
        {
            platform.Draw(gameTime, spriteBatch);
        }
    }

    public void Update(GameTime gameTime)
    {

    }
}
