namespace CatUI.Data.Events.Input.Pointer
{
    public class PointerMoveEventArgs : AbstractPointerEventArgs
    {
        public PointerMoveEventArgs(object target, PointerMoveEventArgs other) :
            this(
                target: target,
                position: other.Position,
                isPressed: other.IsPressed)
        { }

        public PointerMoveEventArgs(object target, Point2D position, bool isPressed)
        {
            base.Target = target;
            base.Position = position;
            base.IsPressed = isPressed;
        }
    }
}