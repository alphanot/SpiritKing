using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SpiritKing.Components.Interfaces;
using System;

namespace SpiritKing.Components.Nodes.Collectables;
public class HealthItem : SpatialEntity, ICollectable
{
    public int DrawOrder => 1;

    public bool Visible => true;

    public bool Enabled => true;

    public int UpdateOrder => 1;

    private readonly Game Game;

    private readonly Texture2D _healthItemSprite;

    public Tweener Tweener { get; set; } = new();

    public int HealthValue { get; set; } = 10;

    public HealthItem(Game game, Vector2? position = null)
    {
        Game = game;
        Position = position == null ? new Vector2(0, 0) : position.Value;

        _healthItemSprite = new Texture2D(game.GraphicsDevice, 1, 1);
        _healthItemSprite.SetData(new[] { Color.White });

        Size = new(40, 40);

        Tweener.TweenTo(this, x => x.Position, Position with { Y = Position.Y - 20 }, 0.6f)
            .AutoReverse().RepeatForever();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_healthItemSprite, Position, Bounds, Color.IndianRed);
    }

    public void Update(GameTime gameTime)
    {
        Tweener.Update(gameTime.GetElapsedSeconds());
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
