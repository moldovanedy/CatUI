namespace CatUI.Data.Shapes
{
    /// <summary>
    /// This is the same as not having a clip shape at all.
    /// </summary>
    public class RectangleClipShape : ClipShape
    {
        /// <inheritdoc cref="ClipShape.IsPointInside"/>
        /// <remarks>This calculation is very fast.</remarks>
        public override bool IsPointInside(Point2D point, Rect bounds, float contentScale, Size viewportSize)
        {
            float endX = bounds.X + bounds.Width;
            float endY = bounds.Y + bounds.Height;

            if (point.X >= endX || point.X <= bounds.X)
            {
                return false;
            }

            if (point.Y >= endY || point.Y <= bounds.Y)
            {
                return false;
            }

            return true;
        }

        public override RectangleClipShape Duplicate()
        {
            return new RectangleClipShape();
        }
    }
}
