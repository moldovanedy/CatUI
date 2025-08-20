using System;
using CatUI.Data;
using CatUI.Platform.Essentials;
using CatUI.Windowing.Common;
using CatUI.Windowing.DesktopApp.PlatformImplementations;
using OpenTK.Windowing.GraphicsLibraryFramework;

#if CAT_LOCAL_LINUX
using CatUI.Platform.Linux;
#endif

namespace CatUI.Windowing.DesktopApp
{
    public class DesktopPlatformInfo : PlatformInfo
    {
        /// <summary>
        /// If you set this, all windows that will be open will have their icon set to this one automatically, removing
        /// the need for manually calling <see cref="DesktopWindow.SetWindowIcon"/> on each window. You can also set
        /// this to null (the default value) at any time, thus letting the platform show a default icon (or none).
        /// </summary>
        /// <remarks>
        /// This is not retroactive, meaning already opened windows will not have this icon set automatically.
        /// </remarks>
        public WindowIcon? DefaultWindowIcon { get; set; }

        private bool _linuxUseWayland = true;

        private readonly PlatformUiOptionsBase _uiOptions =
#if CAT_LOCAL_LINUX
            new LinuxPlatformUiOptions();
#else
            new DesktopPlatformUiOptions();
#endif

        public override CatApplicationInitializer AppInitializer => new(
            new DesktopDispatcher(),
            _uiOptions,
            () =>
            {
#if CAT_LOCAL_WINDOWS
                GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Win32);
#elif CAT_LOCAL_MACOS
                GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Cocoa);
#else
                if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                {
                    GLFW.InitHint(
                        InitHintPlatform.Platform,
                        _linuxUseWayland
                            ? OpenTK.Windowing.GraphicsLibraryFramework.Platform.Wayland
                            : OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11);
                }
                else
                {
                    GLFW.InitHint(InitHintPlatform.Platform, OpenTK.Windowing.GraphicsLibraryFramework.Platform.Any);
                }
#endif

                GLFW.Init();

#if CAT_LOCAL_LINUX
                LinuxNativeCommunicator.Open();
#endif
            });

        /// <summary>
        /// Sets whether the window is created using Wayland or not on Linux (the other option is X11). By default,
        /// this is true, and you should use Wayland whenever possible, except for legacy functionality. 
        /// </summary>
        /// <remarks>This only works on Linux, it is ignored on other platforms.</remarks>
        /// <param name="useWayland">If true, will use Wayland windows on Linux, else will use X11.</param>
        /// <returns>This object.</returns>
        public DesktopPlatformInfo SetLinuxUseWayland(bool useWayland = true)
        {
            _linuxUseWayland = useWayland;
            return this;
        }

        /// <summary>
        /// This is simply a wrapper for setting <see cref="DefaultWindowIcon"/> to be used in a "builder pattern"-like
        /// setting at app startup.
        /// </summary>
        /// <param name="defaultWindowIcon">The icon that will be used on all windows that will be opened later.</param>
        /// <returns>This object.</returns>
        public DesktopPlatformInfo SetDefaultWindowIcon(WindowIcon? defaultWindowIcon = null)
        {
            DefaultWindowIcon = defaultWindowIcon;
            return this;
        }
    }
}
