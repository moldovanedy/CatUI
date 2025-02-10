namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerExitEventHandler(object sender, PointerExitEventArgs e);

    public class PointerExitEventArgs : AbstractPointerEventArgs
    {
        public PointerExitEventArgs(PointerExitEventArgs other) :
            this(
                other.Position,
                other.AbsolutePosition,
                other.IsPressed)
        {
        }

        public PointerExitEventArgs(Point2D position, Point2D absolutePosition, bool isPressed)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = isPressed;
        }
    }
}
