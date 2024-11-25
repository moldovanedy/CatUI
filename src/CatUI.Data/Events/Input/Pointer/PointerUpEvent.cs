namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerUpEventHandler(object sender, PointerUpEventArgs e);

    public class PointerUpEventArgs : AbstractPointerEventArgs
    {
        public PointerUpEventArgs(PointerUpEventArgs other) :
            this(position: other.Position)
        { }

        public PointerUpEventArgs(Point2D position)
        {
            base.Position = position;
            base.IsPressed = false;
        }
    }
}