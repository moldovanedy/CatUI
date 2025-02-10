namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerUpEventHandler(object sender, PointerUpEventArgs e);

    public class PointerUpEventArgs : AbstractPointerEventArgs
    {
        public PointerUpEventArgs(PointerUpEventArgs other) :
            this(other.Position, other.AbsolutePosition)
        {
        }

        public PointerUpEventArgs(Point2D position, Point2D absolutePosition)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = false;
        }
    }
}
