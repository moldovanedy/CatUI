namespace CatUI.Data.Events.Input.Pointer
{
    public class MouseButtonEventArgs : AbstractPointerEventArgs
    {
        public MouseButtonType ButtonType { get; }

        public MouseButtonEventArgs(object target, MouseButtonEventArgs other) :
            this(
                target: target,
                position: other.Position,
                buttonType: other.ButtonType,
                isPressed: other.IsPressed)
        { }

        public MouseButtonEventArgs(
            object target,
            Point2D position,
            MouseButtonType buttonType,
            bool isPressed)
        {
            base.Target = target;
            base.Position = position;
            base.IsPressed = isPressed;
            ButtonType = buttonType;
        }
    }

    public enum MouseButtonType
    {
        Primary = 0,
        Secondary = 1,
        Middle = 2,
        WheelUp = 3,
        WheelDown = 4,
        WheelLeft = 5,
        WheelRight = 6,
        Extra1 = 7,
        Extra2 = 8
    }
}