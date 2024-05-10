using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpiritKing.Components;

public class OutlineRectF : Interfaces.IDrawable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public int Thickness { get; set; }

    private readonly Texture2D sprite;

    private RectangleF _left;
    private RectangleF _top;
    private RectangleF _right;
    private RectangleF _bottom;

    public Vector2 Position
    {
        get
        {
            return new Vector2(X, Y);
        }
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Vector2 Size
    {
        get
        {
            return new Vector2(Width, Height);
        }
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    public int DrawOrder => 1;

    public bool Visible => true;

    public OutlineRectF(Texture2D t2d, float x, float y, float width, float height, int thickness)
    {
        sprite = t2d;

        X = x;
        Y = y;
        Width = width;
        Height = height;
        Thickness = thickness;
        _left = new RectangleF(X, Y, Thickness, Height);
        _top = new RectangleF(X, Y, Width, Thickness);
        _right = new RectangleF(X + Width - Thickness, Y, Thickness, Height);
        _bottom = new RectangleF(X, Y + Height - Thickness, Width, Thickness);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(sprite, Position, _left.ToRectangle(), Color.White);
        spriteBatch.Draw(sprite, Position, _top.ToRectangle(), Color.White);
        spriteBatch.Draw(sprite, Position + new Vector2(Width - Thickness, 0), _right.ToRectangle(), Color.White);
        spriteBatch.Draw(sprite, Position + new Vector2(0, Height - Thickness), _bottom.ToRectangle(), Color.White);
    }

    public void Dispose()
    {
        sprite?.Dispose();
    }
}