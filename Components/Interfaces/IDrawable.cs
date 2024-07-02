using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpiritKing.Components.Interfaces;
public interface IDrawable : INode, IDisposable
{
    int DrawOrder { get; }

    bool Visible { get; }

    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
