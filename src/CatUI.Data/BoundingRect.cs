using System;

namespace CatUI.Data
{
    public readonly struct BoundingRect
    {
        public readonly Point2D Begin = Point2D.Zero;
        public readonly Point2D End = Point2D.Zero;

        public BoundingRect() { }

        public float Width
        {
            get
            {
                return Math.Abs(End.X - Begin.X);
            }
        }
        public float Height
        {
            get
            {
                return Math.Abs(End.Y - Begin.Y);
            }
        }
    }
}
