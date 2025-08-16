using System;
using System.Diagnostics;
using CatUI.Data;
using CatUI.Windowing.DesktopApp;
using CatUISample.UI;

namespace CatUISample.DesktopApp
{
    public static class Program
    {
        private static void Main()
        {
            //early initialization of the app
            CatApplication
                .NewBuilder()
                .SetInitializer(new DesktopPlatformInfo().AppInitializer)
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
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
