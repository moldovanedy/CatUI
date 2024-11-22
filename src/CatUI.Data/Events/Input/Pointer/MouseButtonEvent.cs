namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    public class MouseButtonEventArgs : AbstractPointerEventArgs
    {
        public MouseButtonType ButtonType { get; }

        public MouseButtonEventArgs(MouseButtonEventArgs other) :
            this(
                position: other.Position,
                buttonType: other.ButtonType,
                isPressed: other.IsPressed)
        { }

        public MouseButtonEventArgs(
            Point2D position,
            MouseButtonType buttonType,
            bool isPressed)
        {
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