namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerMoveEventHandler(object sender, PointerMoveEventArgs e);

    public class PointerMoveEventArgs : AbstractPointerEventArgs
    {
        public PointerMoveEventArgs(PointerMoveEventArgs other) :
            this(
                position: other.Position,
                isPressed: other.IsPressed)
        { }

        public PointerMoveEventArgs(Point2D position, bool isPressed)
        {
            Position = position;
            IsPressed = isPressed;
        }
    }
}