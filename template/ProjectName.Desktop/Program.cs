using System.Diagnostics;
using CatUI.Data;
using CatUI.Windowing.Desktop;
#if (usesLib)
using ProjectName.UI;
#endif

namespace ProjectName.Desktop
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Init();

            //we create a new window
            var window = new DesktopWindow(title: "ProjectName", minWidth: 250, minHeight: 200);
#if (usesLib)
            //RootElement is from ProjectName.UI, check it out!
            window.Document.Root = new RootElement();
#endif

            //you MUST open the window before you run it!
            window.Open();

            //top-level exception handler: any exception unhandled by UI code will be logged and the window will close
            //automatically (you can put this in some sort of loop, as Run can be called multiple times unless you called
            //Close, but it's generally better to just show the user some kind of error and close the app as it is corrupted
            //in unknown ways at that point)
            try
            {
                window.Run();
            }
            //you don't need to call Close: the Run function will automatically close the app when the user closes it
            //or the platform requests the window to be closed
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private static void Init()
        {
            //early initialization of the app
            CatApplication
                .NewBuilder()
                //you should ALWAYS set the initializer to ensure you have access to everything from CatApplication
                .SetInitializer(new DesktopPlatformInfo().AppInitializer)
                .Build();
        }
    }
}
