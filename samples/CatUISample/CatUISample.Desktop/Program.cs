using System.Diagnostics;
using CatUI.Windowing.Desktop;
using CatUISample.UI;

namespace CatUISample.Desktop
{
    public static class Program
    {
        private static void Main(string[] args)
        {
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
