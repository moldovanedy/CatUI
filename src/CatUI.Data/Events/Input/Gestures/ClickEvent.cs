using CatUI.Data.Events.Input.Pointer;

namespace CatUI.Data.Events.Input.Gestures
{
    public delegate void ClickEventHandler(object sender, ClickEventArgs e);

    /// <summary>
    /// Represents a mouse click or touchscreen tap on mobile. The difference between this and
    /// <see cref="PointerDownEventArgs"/> is that this only triggers after a subsequent PointerUp, but only if the
    /// event wasn't cancelled, so it's more suitable for most UI elements.
    /// </summary>
    public class ClickEventArgs : InputEventArgs
    {
        /// <summary>
        /// Represents the position of the pointer in element coordinates (relative to the top-left corner
        /// of the element's bounds).
        /// </summary>
        public Point2D Position { get; }

        /// <summary>
        /// Represents the position of the pointer in window coordinates (relative to the top-left corner
        /// of the window's client area).
        /// </summary>
        public Point2D AbsolutePosition { get; }

        public ClickEventArgs(ClickEventArgs other)
            : this(other.Position, other.AbsolutePosition)
        {
        }

        public ClickEventArgs(Point2D position, Point2D absolutePosition)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
        }
    }
}
