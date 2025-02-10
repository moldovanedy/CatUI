using System;

namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    public class MouseButtonEventArgs : AbstractPointerEventArgs
    {
        public MouseButtonType ButtonType { get; }

        public MouseButtonEventArgs(MouseButtonEventArgs other) :
            this(
                other.Position,
                other.AbsolutePosition,
                other.ButtonType,
                other.IsPressed)
        {
        }

        public MouseButtonEventArgs(
            Point2D position, Point2D absolutePosition, MouseButtonType buttonType, bool isPressed)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = isPressed;
            ButtonType = buttonType;
        }
    }

    /// <summary>
    /// Represents the type of the button that is acted upon. All buttons (except the extra ones) should work with
    /// other types of input like touchpads. For touchscreens, generally only primary click is supported.
    /// </summary>
    /// <remarks>
    /// Can be used as a bitmap, as every value represents a bit.
    /// </remarks>
    [Flags]
    public enum MouseButtonType
    {
        /// <summary>
        /// Represents the primary mouse button, usually the left button, but might be the right button if the user
        /// set it in the system preferences of the platform.
        /// </summary>
        Primary = 1,

        /// <summary>
        /// Represents the primary mouse button, usually the right button, but might be the left button if the user
        /// set it in the system preferences of the platform.
        /// </summary>
        Secondary = 2,

        /// <summary>
        /// Represents the mouse middle scroll wheel, if the mouse supports it. This does NOT refer to actual scrolling
        /// behavior, but rather at the literal wheel click.
        /// </summary>
        Middle = 4,

        /// <summary>
        /// Represents the scroll up on mice that support scrolling. 
        /// </summary>
        WheelUp = 8,

        /// <summary>
        /// Represents the scroll down on mice that support scrolling. 
        /// </summary>
        WheelDown = 16,

        /// <summary>
        /// Represents the scroll to left on mice that support scrolling sideways. 
        /// </summary>
        WheelLeft = 32,

        /// <summary>
        /// Represents the scroll to right on mice that support scrolling sideways. 
        /// </summary>
        WheelRight = 64,

        /// <summary>
        /// Represents the first extra button on a mouse on those that support extra buttons (up to 5 supported in CatUI). 
        /// </summary>
        Extra1 = 1 << 7,

        /// <summary>
        /// Represents the second extra button on a mouse on those that support extra buttons (up to 5 supported in CatUI). 
        /// </summary>
        Extra2 = 1 << 8,

        /// <summary>
        /// Represents the third extra button on a mouse on those that support extra buttons (up to 5 supported in CatUI). 
        /// </summary>
        Extra3 = 1 << 9,

        /// <summary>
        /// Represents the fourth extra button on a mouse on those that support extra buttons (up to 5 supported in CatUI). 
        /// </summary>
        Extra4 = 1 << 10,

        /// <summary>
        /// Represents the fifth extra button on a mouse on those that support extra buttons (up to 5 supported in CatUI). 
        /// </summary>
        Extra5 = 1 << 11
    }
}
