namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerEnterEventHandler(object sender, PointerEnterEventArgs e);

    public class PointerEnterEventArgs : AbstractPointerEventArgs
    {
        public PointerEnterEventArgs(PointerEnterEventArgs other) :
            this(
                position: other.Position,
                isPressed: other.IsPressed)
        { }

        public PointerEnterEventArgs(Point2D position, bool isPressed)
        {
            Position = position;
            IsPressed = isPressed;
        }
    }
}