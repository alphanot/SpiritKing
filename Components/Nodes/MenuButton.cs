using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.Interfaces;
using System;
using System.Collections.Generic;

namespace SpiritKing.Components.Nodes;

public class MenuButton : Interfaces.IDrawable
{
    public string Text { get; set; }

    public Point Position
    {
        get
        {
            return _pos;
        }
        set
        {
            _pos = value;
            if (outlineRect != null)
            {
                outlineRect.Position = new Vector2(_pos.X, _pos.Y);
            }
        }
    }

    private Point _pos;

    public Point Size { get; set; }

    public bool Highlighted = false;

    private readonly Texture2D texture;

    private readonly OutlineRectF outlineRect;

    private readonly SpriteFont _font;

    private Vector2 _STRINGSIZE = Vector2.Zero;

    public virtual Action Action { get; set; }
    public int Padding { get; set; }

    public int DrawOrder => 1;

    public bool Visible => true;

    public MenuButton(Game game, Point position, Point size, string text, int padding = 25)
    {
        _font = game.Content.Load<SpriteFont>("Fonts/LabelText");
        texture = new Texture2D(game.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });

        Padding = padding;
        Position = position;
        Text = text;

        _STRINGSIZE = _font.MeasureString(Text);

        var newSize = new Point((int)(size.X < _STRINGSIZE.X ? _STRINGSIZE.X : size.X), (int)(size.Y < _STRINGSIZE.Y ? _STRINGSIZE.Y : size.Y));

        Size = new Point(newSize.X + padding * 2, newSize.Y + padding * 2);
        outlineRect = new OutlineRectF(texture, Position.X, Position.Y, Size.X, Size.Y, 4);
    }

    public void Dispose()
    {
        texture?.Dispose();
        outlineRect.Dispose();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (texture != null)
        {
            spriteBatch.DrawString(_font, Text, new Vector2(Position.X + (Size.X - _STRINGSIZE.X) / 2, Position.Y + (Size.Y - _STRINGSIZE.Y) / 2), Color.LightGray);

            if (Highlighted)
            {
                outlineRect.Draw(gameTime, spriteBatch);
            }
        }
    }
}