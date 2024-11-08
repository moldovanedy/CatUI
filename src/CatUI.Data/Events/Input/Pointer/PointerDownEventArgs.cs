namespace CatUI.Data.Events.Input.Pointer
{
    public class PointerDownEventArgs : AbstractPointerEventArgs
    {
        public PointerDownEventArgs(object target, PointerDownEventArgs other) :
            this(
                target: target,
                position: other.Position)
        { }

        public PointerDownEventArgs(object target, Point2D position)
        {
            base.Target = target;
            base.Position = position;

            base.IsPressed = true;
        }
    }
}