namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerDownEventHandler(object sender, PointerDownEventArgs e);

    public class PointerDownEventArgs : AbstractPointerEventArgs
    {
        public PointerDownEventArgs(object target, PointerDownEventArgs other) :
            this(other.Position, other.AbsolutePosition)
        {
        }

        public PointerDownEventArgs(Point2D position, Point2D absolutePosition)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = true;
        }
    }
}
