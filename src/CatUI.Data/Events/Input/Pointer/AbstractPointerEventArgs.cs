namespace CatUI.Data.Events.Input.Pointer
{
    public abstract class AbstractPointerEventArgs : InputEventArgs
    {
        public Point2D Position { get; protected set; } = Point2D.Zero;
        public bool IsPressed { get; protected set; }
    }
}