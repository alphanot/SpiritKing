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
        Platforms = new();
    }

    public int DrawOrder => 1;

    public bool Visible => true;

    public List<INode> Children { get; set; }

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
