namespace CatUI.Platform.Linux
{
    public static class LinuxNativeCommunicator
    {
        public static void Open()
        {
            _ = XdgServices.Init();
        }
    }
}
