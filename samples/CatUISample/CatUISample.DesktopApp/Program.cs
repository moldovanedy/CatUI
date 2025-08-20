using System;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Managers;
using CatUI.Windowing.Common;
using CatUI.Windowing.DesktopApp;
using CatUISample.UI;

namespace CatUISample.DesktopApp
{
    public static class Program
    {
        private static void Main()
        {
            var icon = AssetsManager.LoadFromAssembly<ImageAsset>(
                "/icon.png",
                typeof(Program));

            //early initialization of the app
            CatApplication
                .NewBuilder()
                .SetPlatformInfo(
                    new DesktopPlatformInfo()
                        //.SetLinuxUseWayland(false)
                        .SetDefaultWindowIcon(icon == null ? null : new WindowIcon(icon, false)))
                .Build();

            InitialSetup.Init();
            var window = new DesktopWindow(title: "CatUI Sample", minWidth: 250, minHeight: 200);
            window.Document.Root = new RootElement();
            window.Open();

            try
            {
                window.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
