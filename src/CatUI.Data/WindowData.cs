namespace CatUI.Data;

/// <summary>
/// Holds some useful information about a window.
/// </summary>
public class WindowData
{
    /// <summary>
    /// The instance of IApplicationWindow that this object represents.
    /// </summary>
    public object CatWindow { get; }

    /// <summary>
    /// A platform-specific object that represents the window. See the remarks for more info about each platform.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>On Windows, it's a nint to the window handle (HWND).</item>
    /// <item>On macOS, it's a nint to the window handle (NSWindow).</item>
    /// <item>On Linux (X11), it's a nint to the window handle (Window).</item>
    /// <item>On Linux (Wayland), it's a nint to the window surface (wl_surface).</item>
    /// <item>On Android, it's a reference to the Android Activity (equal to <see cref="CatWindow"/>).</item>
    /// </list>
    /// </remarks>
    public object NativeWindowHandle { get; }

    /// <summary>
    /// Represents a handle in its framework-defined format (if using a specific framework like GLFW). Can be null.
    /// </summary>
    public object? FrameworkWindowHandle { get; }

    /// <summary>
    /// The window ID as a string. Currently only useful on Linux for native UI like file dialogs and alerts.
    /// On all other platforms it's an empty string, even on Linux Wayland.
    /// </summary>
    public string WindowId { get; }

    public WindowData(
        object catWindow,
        object nativeWindowHandle,
        object? frameworkWindowHandle = null,
        string windowId = "")
    {
        CatWindow = catWindow;
        NativeWindowHandle = nativeWindowHandle;
        FrameworkWindowHandle = frameworkWindowHandle;
        WindowId = windowId;
    }
}
