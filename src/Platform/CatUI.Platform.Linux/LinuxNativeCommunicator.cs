using System.Runtime.Versioning;

namespace CatUI.Platform.Linux;

[SupportedOSPlatform("linux")]
public static class LinuxNativeCommunicator
{
    public static void Open()
    {
        _ = XdgServices.Init();
    }
}
