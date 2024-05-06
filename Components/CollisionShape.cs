using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SpiritKing.Components;

public class CollisionShape
{
    public RectangleF Shape { get; set; }

    public Texture2D _debugTexture;

    public CollisionShape(RectangleF rect)
    {
        Shape = rect;
    }

    public CollisionShape(float x, float y, float width, float height)
        : this(new RectangleF(x, y, width, height)) { }

    public bool IsColliding(CollisionShape other)
    {
        return other != null && Shape.Intersects(other.Shape);
    }

    public bool IsColliding(CollisionShape other, float modifierX, float modifierY)
    {
        return other != null
            && Shape.Intersects(new RectangleF(other.Shape.Position.X + modifierX, other.Shape.Position.Y + modifierY, Shape.Width, Shape.Height));
    }

    public void SetPosition(Vector2 position)
    {
        Shape = new RectangleF(position.X, position.Y, Shape.Width, Shape.Height);
    }

    public Vector2 GetPosition()
    {
        return Shape.Position;
    }

    public void Dispose()
    {
        if (_debugTexture != null)
        {
            _debugTexture.Dispose();
            _debugTexture = null;
        }
    }

    #region Collision

    public bool IsCollidingRight(CollisionShape _otherCollider)
    {
        return this.Shape.Right > _otherCollider.Shape.Left &&
          this.Shape.Left < _otherCollider.Shape.Left &&
          this.Shape.Bottom > _otherCollider.Shape.Top &&
          this.Shape.Top < _otherCollider.Shape.Bottom;
    }

    public bool IsCollidingLeft(CollisionShape _otherCollider)
    {
        return this.Shape.Left < _otherCollider.Shape.Right &&
          this.Shape.Right > _otherCollider.Shape.Right &&
          this.Shape.Bottom > _otherCollider.Shape.Top &&
          this.Shape.Top < _otherCollider.Shape.Bottom;
    }

    public bool IsCollidingBottom(CollisionShape _otherCollider)
    {
        return this.Shape.Bottom > _otherCollider.Shape.Top &&
          this.Shape.Top < _otherCollider.Shape.Top &&
          this.Shape.Right > _otherCollider.Shape.Left &&
          this.Shape.Left < _otherCollider.Shape.Right;
    }

    public bool IsCollidingTop(CollisionShape _otherCollider)
    {
        return this.Shape.Top < _otherCollider.Shape.Bottom &&
          this.Shape.Bottom > _otherCollider.Shape.Bottom &&
          this.Shape.Right > _otherCollider.Shape.Left &&
          this.Shape.Left < _otherCollider.Shape.Right;
    }

    #endregion Collision
}