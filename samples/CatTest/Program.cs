using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Elements;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Text;
using CatUI.Windowing.DesktopApp;

namespace CatTest;

internal static class Program
{
    // private const int GLFW_ANGLE_PLATFORM_TYPE = 0x00050002;
    //
    // private const int GLFW_ANGLE_PLATFORM_TYPE_NONE = 0x00037001;
    // private const int GLFW_ANGLE_PLATFORM_TYPE_OPENGL = 0x00037002;
    // private const int GLFW_ANGLE_PLATFORM_TYPE_OPENGLES = 0x00037003;
    // private const int GLFW_ANGLE_PLATFORM_TYPE_D3D9 = 0x00037004;
    // private const int GLFW_ANGLE_PLATFORM_TYPE_D3D11 = 0x00037005;
    // private const int GLFW_ANGLE_PLATFORM_TYPE_VULKAN = 0x00037007;
    // private const int GLFW_ANGLE_PLATFORM_TYPE_METAL = 0x00037008;
    //
    // private const int EGL_PLATFORM_ANGLE_TYPE_VULKAN_ANGLE = 0x3450;

    private static DesktopWindow? _window;

    private static void Main()
    {
        try
        {
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS() && !OperatingSystem.IsLinux())
            {
                return;
            }

            Init();

            _window = new DesktopWindow(
                800,
                600,
                minWidth: 300,
                minHeight: 200,
                title: "Test");

            _window.Document.BackgroundColor = new Color(0x21_21_21);
            _window.Document.Root = new ColumnContainer
            {
                Children =
                [
                    new RowContainer
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(50),
                        Arrangement = LinearArrangement.SpacedBy(15),
                        Children =
                        [
                            new Element
                            {
                                //Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                Background = new ColorBrush(new Color(0x00_80_ff_80, Color.ColorType.RGBA)),
                                Children =
                                [
                                    new Label("Test 1", TextAlignmentType.Center)
                                    {
                                        Layout = new ElementLayout()
                                            .SetMinMaxAndPreferredWidth("100%", 0, 70),
                                        TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                                    }
                                ]
                            }
                        ]
                    }
                ]
            };

            _window.Open();
            _window.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e);
        }
    }

    private static void Init()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS() && !OperatingSystem.IsLinux())
        {
            return;
        }

        //early initialization of the app
        CatApplication
            .NewBuilder()
            //you should ALWAYS set the initializer to ensure you have access to everything from CatApplication
            .SetPlatformInfo(new DesktopPlatformInfo())
            .Build();
    }
}
