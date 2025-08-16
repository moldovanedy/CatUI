using System;
using CatUI.Data;
using CatUI.Platform.Essentials;
using CatUI.Windowing.Common;
using CatUI.Windowing.DesktopApp.PlatformImplementations;
using OpenTK.Windowing.GraphicsLibraryFramework;

#if WINDOWS || MACOS || MACCATALYST
#else
using CatUI.Platform.Linux;
#endif

namespace CatUI.Windowing.DesktopApp
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
#if WINDOWS
                GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Win32);
#elif MACOS || MACCATALYST
                GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Cocoa);
#else
                if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                {
                    GLFW.InitHint(InitHintPlatform.Platform,
                        OpenTK.Windowing.GraphicsLibraryFramework.Platform.Wayland);
                }
                else
                {
                    GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Any);
                }
#endif

                GLFW.Init();

#if WINDOWS || MACOS || MACCATALYST
#else
                if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                {
                    LinuxNativeCommunicator.Open();
                }
#endif
            });
    }
}
