using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SpiritKing.Utils;

public static class CollisionUtils
{
    public static bool IsCollidingRight(this RectangleF shape, RectangleF otherShape)
    {
        return shape.Right > otherShape.Left &&
          shape.Left < otherShape.Left &&
          shape.Bottom > otherShape.Top &&
          shape.Top < otherShape.Bottom;
    }

    public static bool IsCollidingLeft(this RectangleF shape, RectangleF otherShape)
    {
        return shape.Left < otherShape.Right &&
          shape.Right > otherShape.Right &&
          shape.Bottom > otherShape.Top &&
          shape.Top < otherShape.Bottom;
    }

    public static bool IsCollidingBottom(this RectangleF shape, RectangleF otherShape)
    {
        return shape.Bottom > otherShape.Top &&
          shape.Top < otherShape.Top &&
          shape.Right > otherShape.Left &&
          shape.Left < otherShape.Right;
    }

    public static bool IsCollidingTop(this RectangleF shape, RectangleF otherShape)
    {
        return shape.Top < otherShape.Bottom &&
          shape.Bottom > otherShape.Bottom &&
          shape.Right > otherShape.Left &&
          shape.Left < otherShape.Right;
    }
    public static bool IsCollidingRight(this Rectangle shape, Rectangle otherShape)
        => ((RectangleF)shape).IsCollidingRight(otherShape);
    public static bool IsCollidingLeft(this Rectangle shape, Rectangle otherShape)
        => ((RectangleF)shape).IsCollidingLeft(otherShape);
    public static bool IsCollidingBottom(this Rectangle shape, Rectangle otherShape)
        => ((RectangleF)shape).IsCollidingBottom(otherShape);
    public static bool IsCollidingTop(this Rectangle shape, Rectangle otherShape)
        => ((RectangleF)shape).IsCollidingTop(otherShape);

    public static RectangleF Intersection(this Rectangle shape, RectangleF otherShape)
        => ((RectangleF)shape).Intersection(otherShape);
}