namespace CatUI.Data.Events.Input.Pointer
{
    public class PointerEnterEventArgs : AbstractPointerEventArgs
    {
        public PointerEnterEventArgs(object target, PointerEnterEventArgs other) :
            this(
                target: target,
                position: other.Position,
                isPressed: other.IsPressed)
        { }

        public PointerEnterEventArgs(object target, Point2D position, bool isPressed)
        {
            base.Target = target;
            base.Position = position;
            base.IsPressed = isPressed;
        }
    }
}