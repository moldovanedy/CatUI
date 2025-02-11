using System;
using CatUI.Data;
using CatUI.Data.Enums;

namespace CatUI.Windowing.Common
{
    public abstract class PlatformInfo
    {
        public abstract CatApplicationInitializer AppInitializer { get; }

        public static RuntimePlatform CurrentPlatform
        {
            get
            {
                if (OperatingSystem.IsWindows())
                {
                    return RuntimePlatform.Windows;
                }

                if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                {
                    return RuntimePlatform.MacOS;
                }

                if (OperatingSystem.IsLinux())
                {
                    return RuntimePlatform.Linux;
                }

                if (OperatingSystem.IsFreeBSD())
                {
                    return RuntimePlatform.FreeBSD;
                }

                if (OperatingSystem.IsAndroid())
                {
                    return RuntimePlatform.Android;
                }

                if (OperatingSystem.IsIOS())
                {
                    return RuntimePlatform.iOS;
                }

                if (OperatingSystem.IsBrowser())
                {
                    return RuntimePlatform.Browser;
                }

                if (OperatingSystem.IsTvOS())
                {
                    return RuntimePlatform.TvOS;
                }

                if (OperatingSystem.IsWatchOS())
                {
                    return RuntimePlatform.WatchOS;
                }

                return RuntimePlatform.Unknown;
            }
        }
    }
}
