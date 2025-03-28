namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerMoveEventHandler(object sender, PointerMoveEventArgs e);

    public class PointerMoveEventArgs : AbstractPointerEventArgs
    {
        /// <summary>
        /// The amount of movement from the last movement on the horizontal axis. A negative value means the pointer
        /// moved to the left, a positive one means the pointer moved to the right.
        /// </summary>
        public float DeltaX { get; protected set; }

        /// <summary>
        /// The amount of movement from the last movement on the vertical axis. A negative value means the pointer
        /// moved up, a positive one means the pointer moved down.
        /// </summary>
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
