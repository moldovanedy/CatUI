using System;

namespace CatUI.Data
{
    public struct BoundingRect
    {
        public Point2D Begin = Point2D.Zero;
        public Point2D End = Point2D.Zero;

        public BoundingRect() { }

        public readonly float Width
        {
            get
            {
                return Math.Abs(End.X - Begin.X);
            }
        }
        public readonly float Height
        {
            get
            {
                return Math.Abs(End.Y - Begin.Y);
            }
        }
    }
}
