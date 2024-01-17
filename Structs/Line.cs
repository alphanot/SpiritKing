using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace SpiritKing.Structs
{
    public class Line : IEquatable<Line>
    {
        public Point2 Position;
        public Point2 Target;

        public Line(Point2 Position, Point2 Target)
        {
            this.Position = Position;
            this.Target = Target;
        }

        public void Rotate(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            Target = new Point2
            {
                X = (int)(cosTheta * (Target.X - Position.X) - sinTheta * (Target.Y - Position.Y) + Position.X),
                Y = (int)(sinTheta * (Target.X - Position.X) + cosTheta * (Target.Y - Position.Y) + Position.Y)
            };
        }

        public RectangleF ToRectangle()
        {
            return new RectangleF(Position, new Vector2(Target.X - Position.X, Target.Y - Position.Y));
        }

        public bool Intersects(RectangleF rectangle)
        {
            return LineIntersectsLine(Position, Target, new Point2(rectangle.X, rectangle.Y), new Point2(rectangle.X + rectangle.Width, rectangle.Y)) ||
                   LineIntersectsLine(Position, Target, new Point2(rectangle.X + rectangle.Width, rectangle.Y), new Point2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height)) ||
                   LineIntersectsLine(Position, Target, new Point2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), new Point2(rectangle.X, rectangle.Y + rectangle.Height)) ||
                   LineIntersectsLine(Position, Target, new Point2(rectangle.X, rectangle.Y + rectangle.Height), new Point2(rectangle.X, rectangle.Y)) ||
                   (rectangle.Contains(Position) && rectangle.Contains(Target));
        }

        private bool LineIntersectsLine(Point2 l1Position, Point2 l1Target, Point2 l2Position, Point2 l2Target)
        {
            float q = (l1Position.Y - l2Position.Y) * (l2Target.X - l2Position.X) - (l1Position.X - l2Position.X) * (l2Target.Y - l2Position.Y);
            float d = (l1Target.X - l1Position.X) * (l2Target.Y - l2Position.Y) - (l1Target.Y - l1Position.Y) * (l2Target.X - l2Position.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1Position.Y - l2Position.Y) * (l1Target.X - l1Position.X) - (l1Position.X - l2Position.X) * (l1Target.Y - l1Position.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        public bool Equals(Line other)
        {
            return Position == other.Position && Target == other.Target;
        }
    }
}