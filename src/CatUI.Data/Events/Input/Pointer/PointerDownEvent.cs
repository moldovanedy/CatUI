namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerDownEventHandler(object sender, PointerDownEventArgs e);

    public class PointerDownEventArgs : AbstractPointerEventArgs
    {
        public PointerDownEventArgs(object target, PointerDownEventArgs other) :
            this(position: other.Position)
        { }

        public PointerDownEventArgs(Point2D position)
        {
            Position = position;

            IsPressed = true;
        }
    }
}