namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerMoveEventHandler(object sender, PointerMoveEventArgs e);

    public class PointerMoveEventArgs : AbstractPointerEventArgs
    {
        public float DeltaX { get; protected set; }
        public float DeltaY { get; protected set; }

        public PointerMoveEventArgs(PointerMoveEventArgs other) :
            this(
                other.Position,
                other.AbsolutePosition,
                other.DeltaX,
                other.DeltaY,
                other.IsPressed)
        {
        }

        public PointerMoveEventArgs(
            Point2D position, Point2D absolutePosition, float deltaX, float deltaY, bool isPressed)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            DeltaX = deltaX;
            DeltaY = deltaY;
            IsPressed = isPressed;
        }
    }
}
