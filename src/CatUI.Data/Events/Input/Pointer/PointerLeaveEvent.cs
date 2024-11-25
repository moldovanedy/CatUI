namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerLeaveEventHandler(object sender, PointerLeaveEventArgs e);

    public class PointerLeaveEventArgs : AbstractPointerEventArgs
    {
        public PointerLeaveEventArgs(PointerLeaveEventArgs other) :
            this(
                position: other.Position,
                isPressed: other.IsPressed)
        { }

        public PointerLeaveEventArgs(Point2D position, bool isPressed)
        {
            base.Position = position;
            base.IsPressed = isPressed;
        }
    }
}