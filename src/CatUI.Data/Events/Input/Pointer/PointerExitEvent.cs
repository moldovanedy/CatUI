namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerExitEventHandler(object sender, PointerExitEventArgs e);

    public class PointerExitEventArgs : AbstractPointerEventArgs
    {
        public PointerExitEventArgs(PointerExitEventArgs other) :
            this(
                other.Position,
                other.AbsolutePosition,
                other.IsPressed,
                other.PointerId)
        {
        }

        public PointerExitEventArgs(Point2D position, Point2D absolutePosition, bool isPressed, int pointerId)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = isPressed;
            PointerId = pointerId;
        }
    }
}
