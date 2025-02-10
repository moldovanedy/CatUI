namespace CatUI.Data.Events.Input.Pointer
{
    public abstract class AbstractPointerEventArgs : InputEventArgs
    {
        /// <summary>
        /// Represents the position of the pointer in element coordinates (relative to the top-left corner
        /// of the element's bounds).
        /// </summary>
        public Point2D Position { get; protected set; } = Point2D.Zero;

        /// <summary>
        /// Represents the position of the pointer in window coordinates (relative to the top-left corner
        /// of the window's client area).
        /// </summary>
        public Point2D AbsolutePosition { get; protected set; } = Point2D.Zero;

        /// <summary>
        /// Returns true if the pointer is pressed (for mouse, it means the main button is pressed), false otherwise.
        /// </summary>
        public bool IsPressed { get; protected set; }
    }
}
