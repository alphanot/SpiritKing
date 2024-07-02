using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using IDrawable = SpiritKing.Components.Interfaces.IDrawable;

namespace SpiritKing.Components.Nodes;
public class DamageText : IDrawable
{
    public int DrawOrder => 1;
    public bool Visible => true;

    public Vector2 Position { get; set; }
    public string DamageAmount { get; set; } = string.Empty;

    private readonly SpriteFont _font;

    public DamageText(Game game, Vector2 position, string damage)
    {
        _font = game.Content.Load<SpriteFont>("Fonts/HUDText");
        Position = position;
        DamageAmount = damage;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, DamageAmount, Position, Color.White);
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
