namespace CatUI.Data.Events.Input.Pointer;

public delegate void PointerDownEventHandler(object sender, PointerDownEventArgs e);

public class PointerDownEventArgs : AbstractPointerEventArgs
{
    public PointerDownEventArgs(PointerDownEventArgs other) :
        this(other.Position, other.AbsolutePosition, other.PointerId)
    {
    }

    public PointerDownEventArgs(Point2D position, Point2D absolutePosition, int pointerId)
    {
        Position = position;
        AbsolutePosition = absolutePosition;
        IsPressed = true;
        PointerId = pointerId;
    }
}
