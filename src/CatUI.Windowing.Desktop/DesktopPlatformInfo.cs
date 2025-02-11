using CatUI.Data;
using CatUI.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Windowing.Desktop
{
    public class DesktopPlatformInfo : PlatformInfo
    {
        public override CatApplicationInitializer AppInitializer => new(
            new DesktopDispatcher(),
            () => { GLFW.Init(); });
    }
}
