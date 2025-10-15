using CatUI.Data;
using CatUI.Windowing.DesktopApp;

namespace CatUI.Tests.Windowing;

public class BaseWindowingManager
{
    public DesktopWindow? Window { get; private set; }

    public BaseWindowingManager()
    {
        CatApplication
            .NewBuilder()
            .SetPlatformInfo(
                new DesktopPlatformInfo()
                // .SetLinuxUseWayland(false)
            )
            .Build();
    }

    public void OpenAndRunWindow(
        int width = 800,
        int height = 600,
        string title = "",
        int minWidth = 50,
        int maxWidth = ushort.MaxValue,
        int minHeight = 50,
        int maxHeight = ushort.MaxValue,
        DesktopWindow.WindowFlags windowFlags = DesktopWindow.WindowFlags.Default,
        DesktopWindow.WindowMode startupMode = DesktopWindow.WindowMode.Windowed)
    {
        Window = new DesktopWindow(
            width,
            height,
            title,
            minWidth,
            maxWidth,
            minHeight,
            maxHeight,
            windowFlags,
            startupMode);

        Window.Open();
        Window.Run();
    }
}
