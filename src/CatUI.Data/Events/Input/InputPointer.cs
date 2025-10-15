using CatUI.Data.Events.Input.Pointer;

namespace CatUI.Data.Events.Input;

public class InputPointer
{
    /// <inheritdoc cref="AbstractPointerEventArgs.AbsolutePosition"/>
    public Point2D AbsolutePosition { get; }

    /// <summary>
    /// This is true if the pointer is considered to be pressed, this has different meaning depending on the pointer
    /// type:
    /// <list type="bullet">
    /// <item>for mouse cursor: true if the primary button is pressed, false otherwise</item>
    /// <item>
    /// for fingers on a touchscreen: true if the finger is down, false otherwise (it is also invalid if not down)
    /// </item>
    /// </list>
    /// </summary>
    public bool IsConsideredPressed { get; }

    /// <summary>
    /// Represents the unique ID of the pointer. -1 means an invalid pointer (generally on mobile when the finger
    /// is up or the mouse when it's outside the application window). On all platforms, the IDs will start from 0.
    /// This ID will stay the same for the pointer lifetime.
    /// </summary>
    /// <remarks>
    /// This is not going to behave like an index (i.e. will not be 0 for the first pointer, 1 for the second one
    /// etc.), but rather will be randomly picked. DO NOT rely on this as some sort of index, as the pointer can
    /// have any positive value in the int range, and this is sometimes dependent on the runtime platform. However,
    /// as long as you don't use this as some sort of index in the active pointer dictionary, there should be no
    /// problems.
    /// </remarks>
    public int PointerId { get; }

    public InputDeviceType DeviceType { get; }

    public InputPointer(
        Point2D absolutePosition,
        bool isConsideredPressed,
        int pointerId,
        InputDeviceType deviceType)
    {
        AbsolutePosition = absolutePosition;
        IsConsideredPressed = isConsideredPressed;
        PointerId = pointerId;
        DeviceType = deviceType;
    }

    /// <summary>
    /// Returns true if the <see cref="PointerId"/> is other than -1, false if it's -1.
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return PointerId != -1;
    }


    public enum InputDeviceType
    {
        Mouse = 0,
        Touch = 1
    }
}
