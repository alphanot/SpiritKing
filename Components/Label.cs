using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.Interfaces;

namespace SpiritKing.Components;

internal class Label : INode
{
    public int DrawOrder => 1;

    public string Text { get; set; }

    public Vector2 Position { get; set; }

    public Color Color { get; set; }

    private readonly SpriteFont _font;

    private readonly Texture2D _t2d;

    public Vector2 STRING_SIZE { get; private set; }

    public Label(Game game, string text, Vector2 position, Color color)
    {
        _font = game.Content.Load<SpriteFont>("Fonts/TitleText");
        _t2d = new Texture2D(game.GraphicsDevice, 1, 1);
        _t2d.SetData(new[] { Color.White });

        Position = position;
        Text = text;
        Color = color;
        STRING_SIZE = _font.MeasureString(Text);
    }

    public Label(Game game, string text, Vector2 position) : this(game, text, position, Color.White)
    { }

    public void Dispose()
    {
        _t2d?.Dispose();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, Text, Position, Color);
    }

    public void Update(GameTime gameTime)
    { }
}