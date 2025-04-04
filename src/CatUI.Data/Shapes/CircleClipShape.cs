using System;

namespace CatUI.Data.Shapes
{
    /// <summary>
    /// Similar to <see cref="EllipseClipShape"/>, but this will always be a circle centered at the element center point.
    /// This means that the circle diameter is the minimum between the element's width and height.
    /// </summary>
    public class CircleClipShape : ClipShape
    {
        public override bool IsPointInside(Point2D point, Rect bounds, float contentScale, Size viewportSize)
        {
            float xOffset = point.X - bounds.CenterX;
            float yOffset = point.Y - bounds.CenterY;
            float r = Math.Min(bounds.Width, bounds.Height) / 2f;

            return (xOffset * xOffset) + (yOffset * yOffset) <= r * r;
        }

        public override CircleClipShape Duplicate()
        {
            return new CircleClipShape();
        }
    }
}
