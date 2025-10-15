namespace CatUI.Data.Events.Input.Pointer;

public abstract class AbstractPointerEventArgs : InputEventArgs
{
    /// <summary>
    /// Represents the position of the pointer in element coordinates (relative to the top-left corner
    /// of the element's bounds). This is always in pixel coordinates, not dp.
    /// </summary>
    public Point2D Position { get; protected set; } = Point2D.Zero;

    /// <summary>
    /// Represents the position of the pointer in window coordinates (relative to the top-left corner
    /// of the window's client area). This is always in pixel coordinates, not dp.
    /// </summary>
    public Point2D AbsolutePosition { get; protected set; } = Point2D.Zero;

    /// <summary>
    /// Returns true if the pointer is pressed (for mouse, it means the main button is pressed), false otherwise.
    /// </summary>
    public bool IsPressed { get; protected set; }

    /// <summary>
    /// Represents the unique ID of the pointer that generated the event. See <see cref="InputPointer.PointerId"/>
    /// for more info. Will be -1 only if an error occurred while getting the pointer data.
    /// </summary>
    public int PointerId { get; protected set; } = -1;
}
