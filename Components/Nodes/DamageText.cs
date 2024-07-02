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

    private readonly Color _color;

    public DamageText(Game game, Vector2 position, string damage, Color color)
    {
        _font = game.Content.Load<SpriteFont>("Fonts/HUDText");
        Position = position;
        DamageAmount = damage;
        _color = color;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, DamageAmount, Position, _color);
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
