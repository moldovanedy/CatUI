using CatUI.Data;
using CatUI.Windowing.Common;
using CatUI.Windowing.Desktop.PlatformImplementations;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Windowing.Desktop
{
    public class DesktopPlatformInfo : PlatformInfo
    {
        public override CatApplicationInitializer AppInitializer => new(
            new DesktopDispatcher(),
            new DesktopPlatformUiOptions(),
            () => { GLFW.Init(); });
    }
}
