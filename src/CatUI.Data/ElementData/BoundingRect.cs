using System;

namespace CatUI.Data.ElementData
{
    public readonly struct BoundingRect : ICloneable
    {
        public readonly Point2D Begin = Point2D.Zero;
        public readonly Point2D End = Point2D.Zero;

        public BoundingRect() { }

        public BoundingRect(Point2D begin, Point2D end)
        {
            Begin = begin;
            End = end;
        }

        public float Width => Math.Abs(End.X - Begin.X);
        public float Height => Math.Abs(End.Y - Begin.Y);

        public object Clone()
        {
            return new BoundingRect(Begin, End);
        }
    }
}
