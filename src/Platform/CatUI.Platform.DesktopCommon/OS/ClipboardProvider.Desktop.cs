using System.Runtime.Versioning;
using CatUI.Platform.CommonInterface;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Platform.DesktopCommon.OS;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("linux")]
public class ClipboardProviderDesktop : IClipboardProvider
{
    private nint _windowHandle;

    /// <summary>
    /// Sets the GLFW window pointer for clipboard. This is automatically handled by DesktopWindow, you don't need
    /// to call this at all.
    /// </summary>
    /// <param name="windowHandle"></param>
    public void SetGlfwWindowHandle(nint windowHandle)
    {
        _windowHandle = windowHandle;
    }

    /// <inheritdoc/>
    /// <remarks>On desktop, this function only works if you have at least one window open.</remarks>
    public unsafe string GetClipboardContent()
    {
        if (_windowHandle == 0)
        {
            return string.Empty;
        }

        string content = GLFW.GetClipboardString((Window*)_windowHandle);
        return string.IsNullOrEmpty(content) ? string.Empty : content;
    }

    /// <inheritdoc/>
    /// <remarks>On desktop, this function only works if you have at least one window open.</remarks>
    public unsafe bool SetClipboardContent(string content)
    {
        if (_windowHandle == 0)
        {
            return false;
        }

        GLFW.SetClipboardString((Window*)_windowHandle, content);
        return GLFW.GetError(out _) == ErrorCode.NoError;
    }
}
