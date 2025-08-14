using CatUI.Data;
using CatUI.Platform.Essentials;
using CatUI.Windowing.Common;
using CatUI.Windowing.Desktop.PlatformImplementations;
using OpenTK.Windowing.GraphicsLibraryFramework;

#if WINDOWS || MACOS || MACCATALYST
#else
using CatUI.Platform.Linux;
#endif

namespace CatUI.Windowing.Desktop
{
    public class DesktopPlatformInfo : PlatformInfo
    {
        private readonly PlatformUiOptionsBase _uiOptions =
#if WINDOWS || MACOS || MACCATALYST
            new DesktopPlatformUiOptions();
#else
            new LinuxPlatformUiOptions();
#endif

        public override CatApplicationInitializer AppInitializer => new(
            new DesktopDispatcher(),
            _uiOptions,
            () =>
            {
                GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Wayland);
                GLFW.Init();
#if WINDOWS || MACOS || MACCATALYST
#else
                LinuxNativeCommunicator.Open();
#endif
            });
    }
}
