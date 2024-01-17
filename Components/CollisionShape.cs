using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpiritKing.Components
{
    public class CollisionShape
    {
        public RectangleF Shape { get; set; }

        public Color ColliderDebugColor { get; set; } = Color.MediumVioletRed;

        public bool DebugOn;

        private bool _allowDraw = false;

        private GraphicsDevice _device;

        public Texture2D _debugTexture;

        public CollisionShape(RectangleF rect, bool allowDraw = false, GraphicsDevice graphicsDevice = null)
        {
            _device = graphicsDevice;
            Shape = rect;
            DebugOn = allowDraw;
            if (graphicsDevice != null)
            {
                _debugTexture = new Texture2D(graphicsDevice, 1, 1);
                _debugTexture.SetData(new[] { Color.OrangeRed });
            }
        }

        public CollisionShape(float x, float y, float width, float height, bool allowDraw = false, GraphicsDevice graphicsDevice = null)
            : this(new RectangleF(x, y, width, height), allowDraw, graphicsDevice) { }

        public bool IsColliding(CollisionShape other)
        {
            if (other == null) return false;
            return Shape.Intersects(other.Shape);
        }

        public bool IsColliding(CollisionShape other, float modifierX, float modifierY)
        {
            if (other == null) return false;
            return Shape.Intersects(new RectangleF(other.Shape.Position.X + modifierX, other.Shape.Position.Y + modifierY, Shape.Width, Shape.Height));
        }

        public void SetPosition(Vector2 position)
        {
            Shape = new RectangleF(position.X, position.Y, Shape.Width, Shape.Height);
        }

        public Vector2 GetPosition()
        {
            return Shape.Position;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (_allowDraw)
            {
                spriteBatch.Draw(_debugTexture, new Vector2(Shape.X, Shape.Y), Color.White);
            }
        }

        public void Dispose()
        {
            if (_debugTexture != null)
            {
                _debugTexture.Dispose();
                _debugTexture = null;
            }
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
        }

        #region Collision

        public bool IsCollidingLeft(CollisionShape _otherCollider)
        {
            return this.Shape.Right > _otherCollider.Shape.Left &&
              this.Shape.Left < _otherCollider.Shape.Left &&
              this.Shape.Bottom > _otherCollider.Shape.Top &&
              this.Shape.Top < _otherCollider.Shape.Bottom;
        }

        public bool IsCollidingRight(CollisionShape _otherCollider)
        {
            return this.Shape.Left < _otherCollider.Shape.Right &&
              this.Shape.Right > _otherCollider.Shape.Right &&
              this.Shape.Bottom > _otherCollider.Shape.Top &&
              this.Shape.Top < _otherCollider.Shape.Bottom;
        }

        public bool IsCollidingTop(CollisionShape _otherCollider)
        {
            return this.Shape.Bottom > _otherCollider.Shape.Top &&
              this.Shape.Top < _otherCollider.Shape.Top &&
              this.Shape.Right > _otherCollider.Shape.Left &&
              this.Shape.Left < _otherCollider.Shape.Right;
        }

        public bool IsCollidingBottom(CollisionShape _otherCollider)
        {
            return this.Shape.Top < _otherCollider.Shape.Bottom &&
              this.Shape.Bottom > _otherCollider.Shape.Bottom &&
              this.Shape.Right > _otherCollider.Shape.Left &&
              this.Shape.Left < _otherCollider.Shape.Right;
        }

        #endregion Collision
    }
}