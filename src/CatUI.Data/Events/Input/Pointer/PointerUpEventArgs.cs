namespace CatUI.Data.Events.Input.Pointer
{
    public class PointerUpEventArgs : AbstractPointerEventArgs
    {
        public PointerUpEventArgs(object target, PointerUpEventArgs other) :
            this(
                target: target,
                position: other.Position)
        { }

        public PointerUpEventArgs(object target, Point2D position)
        {
            base.Target = target;
            base.Position = position;

            base.IsPressed = false;
        }
    }
}