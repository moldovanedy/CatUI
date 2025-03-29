namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void PointerUpEventHandler(object sender, PointerUpEventArgs e);

    public class PointerUpEventArgs : AbstractPointerEventArgs, ICancellableInputEvent
    {
        public bool WasCancelled { get; }

        public PointerUpEventArgs(PointerUpEventArgs other) :
            this(other.Position, other.AbsolutePosition, other.WasCancelled)
        {
        }

        public PointerUpEventArgs(Point2D position, Point2D absolutePosition, bool wasCancelled = false)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = false;
            WasCancelled = wasCancelled;
        }
    }
}
