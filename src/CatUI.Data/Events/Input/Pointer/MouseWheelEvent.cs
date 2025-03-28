namespace CatUI.Data.Events.Input.Pointer
{
    public delegate void MouseWheelEventHandler(object sender, MouseWheelEventArgs e);

    public class MouseWheelEventArgs : AbstractPointerEventArgs
    {
        /// <summary>
        /// The horizontal scroll difference from the last scroll event. A positive value means scrolling to the right,
        /// negative means scrolling to the left. This is expressed in screen units which are already scaled, so you
        /// normally don't have to scale those values using some other function.
        /// </summary>
        /// <remarks>
        /// This is rare for mouse wheels, as they generally spin vertically, but most of the touchpads will have
        /// horizontal scrolling.
        /// </remarks>
        public float DeltaX { get; }

        /// <summary>
        /// The vertical scroll difference from the last scroll event. A positive value means scrolling down,
        /// negative means scrolling up. This is expressed in screen units which are already scaled, so you
        /// normally don't have to scale those values using some other function.
        /// </summary>
        /// <remarks>This is the normal scrolling behavior of most mice, as well as touchpads.</remarks>
        public float DeltaY { get; }

        public MouseWheelEventArgs(MouseWheelEventArgs other) :
            this(
                other.Position,
                other.AbsolutePosition,
                other.DeltaX,
                other.DeltaY,
                other.IsPressed)
        {
        }

        public MouseWheelEventArgs(
            Point2D position, Point2D absolutePosition, float deltaX, float deltaY, bool isPressed)
        {
            Position = position;
            AbsolutePosition = absolutePosition;
            IsPressed = isPressed;
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }
}
