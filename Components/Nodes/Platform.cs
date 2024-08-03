using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpiritKing.Utils;

namespace SpiritKing.Components.Nodes;

public class Platform : SpatialEntity, Interfaces.IDrawable
{
    public int DrawOrder { get; } = 1;
    public Game Game { get; set; }

    public bool Visible => true;

    public bool IsActive { get; set; } = true;

    private readonly Texture2D _tempSprite;

    public Platform(Game game) : this(game, 1, Vector2.Zero, Vector2.Zero)
    {
    }

    public Platform(Game game, int drawOrder, Vector2 position, Vector2 size)
    {
        DrawOrder = drawOrder;
        Game = game;
        _tempSprite = new Texture2D(game.GraphicsDevice, 1, 1);
        _tempSprite.SetData(new[] { Color.White });
        Position = position;
        Size = size;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_tempSprite, Position, Bounds, Color.DarkSlateGray);
    }

    public void Dispose()
    {
        _tempSprite.Dispose();
    }
}