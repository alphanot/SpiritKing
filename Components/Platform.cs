using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpiritKing.Components.Interfaces;
using System;

namespace SpiritKing.Components
{
    public class Platform : INode
    {
        public int DrawOrder { get; } = 1;
        public Game Game { get; set; }
        public Vector2 Position { get; set; }
        public CollisionShape Collider { get; set; }

        public static event Action<CollisionShape> GetCollidable;

        private Texture2D _tempSprite { get; set; }

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
            Collider = new CollisionShape((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_tempSprite, Position, Collider.Shape.ToRectangle(), Color.DarkSlateGray);
        }

        public void Update(GameTime gameTime)
        {
            GetCollidable?.Invoke(Collider);
        }

        public void Dispose()
        {
            _tempSprite.Dispose();
        }
    }
}