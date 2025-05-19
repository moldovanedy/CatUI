using SkiaSharp;

namespace CatUI.Data.Shapes
{
    /// <summary>
    /// Represents the base for all clip shapes. Those can be used to set the clip region of an element that will be
    /// used for both clipping the content so it doesn't overflow and hit testing for the user pointers.
    /// </summary>
    public abstract class ClipShape : CatObject
    {
        /// <summary>
        /// Returns true if the point is inside the shape, false otherwise. See the remarks on each shape to see how
        /// fast will this computation be.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <param name="bounds">The element absolute bounds.</param>
        /// <param name="contentScale">The document's content scale.</param>
        /// <param name="viewportSize">The viewport size in pixels.</param>
        /// <returns>True if the given point is inside the shape, false otherwise.</returns>
        public abstract bool IsPointInside(Point2D point, Rect bounds, float contentScale, Size viewportSize);

        public abstract SKPath GetSkiaClipPath(Rect bounds, float contentScale, Size viewportSize);
    }
}
