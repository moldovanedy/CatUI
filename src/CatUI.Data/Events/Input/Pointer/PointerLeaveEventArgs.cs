namespace CatUI.Data.Events.Input.Pointer
{
    public class PointerLeaveEventArgs : AbstractPointerEventArgs
    {
        public PointerLeaveEventArgs(object target, PointerLeaveEventArgs other) :
            this(
                target: target,
                position: other.Position,
                isPressed: other.IsPressed)
        { }

        public PointerLeaveEventArgs(object target, Point2D position, bool isPressed)
        {
            base.Target = target;
            base.Position = position;
            base.IsPressed = isPressed;
        }
    }
}