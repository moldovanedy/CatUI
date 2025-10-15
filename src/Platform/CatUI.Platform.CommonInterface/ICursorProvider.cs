using CatUI.Data;

namespace CatUI.Platform.CommonInterface;

public interface ICursorProvider
{
    /// <summary>
    /// Gets called whenever the cursor "faking" mode changes. Always called at initialization (when creating the
    /// cursor manager).
    /// </summary>
    /// <param name="fakeCursorMode">The new mode.</param>
    void SetBuiltInCursorFakeMode(FakeCursorMode fakeCursorMode);

    /// <summary>
    /// Returns the default cursor icon. This is generally only called once at the application startup.
    /// </summary>
    /// <param name="size">The size of the cursor. Pass this inside the <see cref="CursorIcon"/>.</param>
    /// <param name="hotspot">The hotspot of the cursor. Pass this inside the <see cref="CursorIcon"/>.</param>
    /// <returns>The default cursor icon (generally the "arrow" cursor).</returns>
    CursorIcon GetDefaultCursorIcon(Size size, Point2D hotspot);

    /// <summary>
    /// Will be called when a new cursor is needed (the built-in cursors will be created the same: only when needed).
    /// You need to set <see cref="CursorIcon.InternalPointerData"/>, as you'll need to use it in
    /// <see cref="SetCursorAsActive"/>.
    /// </summary>
    /// <param name="id">The ID of the cursor. Pass this inside the <see cref="CursorIcon"/>.</param>
    /// <param name="size">The size of the cursor. Pass this inside the <see cref="CursorIcon"/>.</param>
    /// <param name="hotspot">The hotspot of the cursor. Pass this inside the <see cref="CursorIcon"/>.</param>
    /// <param name="isBuiltIn">
    /// If true, ignore <c>pixelData</c>, pass 0 to <c>size</c> and <c>hotspot</c>, and provide the built-in cursor.
    /// Otherwise, construct a new cursor using the pixel data.
    /// </param>
    /// <param name="pixelData">
    /// A byte array containing the RGBA 32-bit pixel data. The first byte is the R component of the first pixel,
    /// the fifth byte is the R component of the second pixel etc. Pixels are placed left-to-right, top-to-bottom
    /// (row-major order). This RGBA 32-bit format is used to speed up native cursor creation (use as few
    /// allocations as possible).
    /// </param>
    /// <returns>The new cursor or null if it can't be created.</returns>
    CursorIcon? CreateCursor(int id, Size size, Point2D hotspot, bool isBuiltIn, byte[]? pixelData = null);

    /// <summary>
    /// Destroys a custom cursor.
    /// </summary>
    /// <param name="cursorIcon">The cursor to destroy.</param>
    void DestroyCursor(CursorIcon cursorIcon);

    /// <summary>
    /// Sets the given cursor as the current cursor.
    /// </summary>
    /// <param name="windowIdentifier">
    /// The window ID to set the cursor for. On desktop platforms, this will be an <see cref="nint"/> to the
    /// GLFWWindow struct. On the rest of the platforms, this will be null, as you can't have multiple windows,
    /// so the cursor will always be set to your single surface.
    /// </param>
    /// <param name="cursorIcon">The cursor icon to set.</param>
    /// <returns>
    /// True if the cursor could be set, false otherwise (the default cursor will be set in that case).
    /// </returns>
    bool SetCursorAsActive(object? windowIdentifier, CursorIcon cursorIcon);

    /// <param name="windowIdentifier">
    /// The window ID to set the cursor for. On desktop platforms, this will be an <see cref="nint"/> to the
    /// GLFWWindow struct. On the rest of the platforms, this will be null, as you can't have multiple windows,
    /// so the cursor will always be set to your single surface.
    /// </param>
    /// <param name="cursorMode">The cursor mode to set.</param>
    void SetCursorMode(object? windowIdentifier, CursorMode cursorMode);

    enum FakeCursorMode
    {
        /// <summary>
        /// If a built-in cursor is not supported by the runtime platform, the cursor will default to
        /// <see cref="CursorIcon.CURSOR_ARROW"/>.
        /// </summary>
        DefaultToArrow = 0,

        /// <summary>
        /// If a built-in cursor is not supported by the runtime platform, a custom ("fake") cursor will be drawn
        /// instead.
        /// </summary>
        DrawFakeCursor = 1
    }

    enum CursorMode
    {
        /// <summary>
        /// The cursor is visible. This is the default mode.
        /// </summary>
        Visible = 0,

        /// <summary>
        /// The cursor is hidden inside the window.
        /// </summary>
        Hidden = 1,

        /// <summary>
        /// The cursor is locked in place, but sends relative movement events. This is not supported on all
        /// platforms, currently only supported on desktop. 
        /// </summary>
        Locked = 2
    }
}
