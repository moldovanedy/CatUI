namespace CatUI.Data.Events.Input.Pointer;

public delegate void PointerEnterEventHandler(object sender, PointerEnterEventArgs e);

public class PointerEnterEventArgs : AbstractPointerEventArgs
{
    public PointerEnterEventArgs(PointerEnterEventArgs other) :
        this(
            other.Position,
            other.AbsolutePosition,
            other.IsPressed,
            other.PointerId)
    {
    }

    public PointerEnterEventArgs(Point2D position, Point2D absolutePosition, bool isPressed, int pointerId)
    {
        Position = position;
        AbsolutePosition = absolutePosition;
        IsPressed = isPressed;
        PointerId = pointerId;
    }
}
