using SkiaSharp;

namespace CatUI.Data
{
    /// <summary>
    /// Represents the cursor (or pointer) icon.
    /// </summary>
    public class CursorIcon
    {
        public const int CURSOR_ARROW = 0;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_CONTEXT_MENU = 1;
        public const int CURSOR_HELP = 2;
        public const int CURSOR_POINTING_HAND = 3;

        //needs non-GLFW implementation
        public const int CURSOR_PROGRESS = 4;

        //needs non-GLFW implementation
        public const int CURSOR_WAIT = 5;

        public const int CURSOR_CROSSHAIR = 6;

        public const int CURSOR_TEXT = 7;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_VERTICAL_TEXT = 8;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_DROP_ALIAS = 9;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_DROP_COPY = 10;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_NO_DROP = 11;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_GRAB = 12;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_GRABBING = 13;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_COLUMN_RESIZE = 14;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_ROW_RESIZE = 15;
        public const int CURSOR_NS_RESIZE = 16;
        public const int CURSOR_EW_RESIZE = 17;
        public const int CURSOR_NESW_RESIZE = 18;
        public const int CURSOR_NWSE_RESIZE = 19;

        public const int CURSOR_ALL_RESIZE = 20;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_ZOOM_IN = 21;

        //win32 needs custom and everything needs non-GLFW implementation
        public const int CURSOR_ZOOM_OUT = 22;

        public const int CURSOR_NOT_ALLOWED = 23;

        public const int CURSOR_DEFAULT = CURSOR_ARROW;
        public const int BUILT_IN_CURSOR_LENGTH = 24;

        /// <summary>
        /// The cursor's internal ID. Some cursors are built-in (their IDs are known at compile-time, all of them are
        /// below 256), some are custom cursors, created at runtime from data. Starts at 0.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// True if the cursor is built-in (i.e. provided by the platform, not a custom cursor), false if the cursor is
        /// a custom cursor created at runtime.
        /// </summary>
        public bool IsBuiltInPointerShape => Id < 256;

        /// <summary>
        /// True if the cursor is a built-in one, but the platform does not support it (or it hasn't been implemented
        /// in CatUI). In this case, the cursor can be "faked" with a custom one, OR it's defaulting to
        /// <see cref="CURSOR_ARROW"/>, depending on the value last set in ICursorProvider.SetBuiltInCursorFakeMode(). 
        /// </summary>
        /// <remarks>
        /// This is only relevant if <see cref="IsBuiltInPointerShape"/> is true, for custom cursors it's not relevant
        /// and can have any value. It's also prevalent on Windows (possibly Linux), as the other platforms have good
        /// built-in support for the CatUI specified cursors.
        /// </remarks>
        public bool IsBuiltInFaked { get; }

        /// <summary>
        /// The pointer size in pixels. If it's a built-in cursor, this will be zero.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// The pointer's hotspot (i.e. the point that actually is considered for hover and press events) relative
        /// to the pointer's top-left corner. If it's a built-in cursor, this will be zero.
        /// </summary>
        public Point2D Hotspot { get; }

        /// <summary>
        /// Holds the pointer data in a platform-specific format (see remarks) for efficiency. You should generally not
        /// need to use this, as this class is an abstraction over the pointers. If this is null, it's generally an
        /// error, and it means the CatUI cursor API has not been initialized properly or support for the current
        /// platform has not yet been implemented correctly.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>On desktop (Windows, macOS, Linux), it's an <see cref="nint"/> to GLFW's GLFWCursor.</item>
        /// <item>On Android, it's an android.view.PointerIcon.</item>
        /// </list>
        /// </remarks>
        public object? InternalPointerData { get; private set; }

        /// <summary>
        /// Returns the pointer shape pixels in an <see cref="SKImage"/> if the data can be retrieved. Try to cache
        /// the result of this operation, as it implies some overhead. If the pointer shape is built-in, this returns
        /// null.
        /// </summary>
        /// <returns>The pointer pixels in an <see cref="SKImage"/> if possible, null otherwise.</returns>
        public SKImage? GetPointerImage()
        {
            _ = InternalPointerData;
            return null;
        }

        public CursorIcon(int id, Size size, Point2D hotspot, bool isBuiltInFaked, object? internalPointerData)
        {
            Id = id;
            Size = size;
            Hotspot = hotspot;
            IsBuiltInFaked = isBuiltInFaked;
            InternalPointerData = internalPointerData;
        }
    }
}
