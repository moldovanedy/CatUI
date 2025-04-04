namespace CatUI.Data.Shapes
{
    public class EllipseClipShape : ClipShape
    {
        public override bool IsPointInside(Point2D point, Rect bounds, float contentScale, Size viewportSize)
        {
            float xOffset = point.X - bounds.CenterX;
            float yOffset = point.Y - bounds.CenterY;
            float a = bounds.Width / 2f;
            float b = bounds.Height / 2f;

            return
                (xOffset * xOffset / (a * a)) +
                (yOffset * yOffset / (b * b)) <= 1;
        }

        public override EllipseClipShape Duplicate()
        {
            return new EllipseClipShape();
        }
    }
}
