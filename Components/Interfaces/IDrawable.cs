using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace SpiritKing.Components.Interfaces;
public interface IDrawable : INode, IDisposable
{
    int DrawOrder { get; }

    bool Visible { get; }

    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
