using System;
using System.Reflection;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Elements;
using CatUI.Elements.Media;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Windowing.Desktop;

namespace CatTest
{
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
                AssetsManager.AddAssetAssembly(Assembly.GetExecutingAssembly());

                var image = AssetsManager.LoadFromAssembly<Image>("/Assets/search_128px.png");
                if (image == null)
                {
                    throw new NullReferenceException("Image is null");
                }

                _window = new DesktopWindow(
                    800,
                    600,
                    //minWidth: 300,
                    //minHeight: 200,
                    "Test");

                _window.Document.BackgroundColor = new Color(0x21_21_21);
                _window.Document.Root = new Element
                {
                    Children =
                    [
                        new RectangleElement(new ColorBrush(new Color(0x00_ff_ff)))
                        {
                            Position = "10 5",
                            Layout = new ElementLayout().SetFixedWidth("80%").SetFixedHeight("20%"),
                            Children =
                            [
                                new ImageView(image)
                                {
                                    //Position = "20 20",
                                    Layout =
                                        new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                    Background = new ColorBrush(new Color(0xff_00_ff)),
                                    HorizontalAlignment = HorizontalAlignmentType.Right,
                                    VerticalAlignment = VerticalAlignmentType.Bottom,
                                    ImageFit = ImageFitType.CanShrinkAndGrow,
                                    ShouldKeepAspectRatio = true
                                }
                            ]
                        },
                        new RectangleElement(new ColorBrush(new Color(0xff_ff_00)))
                        {
                            Position = new Dimension2(10, "60%"),
                            Layout = new ElementLayout().SetFixedWidth("80%").SetFixedHeight("20%"),
                            Children =
                            [
                                new GeometricPathElement(
                                    "M0,0.054V20h21V0.054H0z M15.422,18.129l-5.264-2.768l-5.265,2.768l1.006-5.863L1.64,8.114l5.887-0.855l2.632-5.334l2.633,5.334l5.885,0.855l-4.258,4.152L15.422,18.129z",
                                    new ColorBrush(new Color(0xff_98_00)),
                                    new ColorBrush(new Color(0x21_96_f3)))
                                {
                                    Position = "5 10",
                                    Layout =
                                        new ElementLayout().SetFixedWidth("25%").SetFixedHeight("15%"),
                                    Background = new ColorBrush(new Color(0xff_ff_ff)),
                                    ShouldApplyScaling = true,
                                    OutlineParameters = new OutlineParams(
                                        2,
                                        LineCapType.Round,
                                        miterLimit: 5)
                                },
                                new RectangleElement(
                                    new ColorBrush(new Color(0x1d_ea_85)))
                                {
                                    Position = "55% 10%",
                                    Layout = new ElementLayout().SetFixedWidth("35%")
                                                                .SetFixedHeight("15%")
                                },
                                new TextBlock("He\u00adllo wor\u00adld!\nHe\u00adllo wor\u00adld!")
                                {
                                    Layout = new ElementLayout()
                                             .SetMinMaxAndPreferredWidth("25%", Dimension.Unset, 250)
                                             .SetMinMaxAndPreferredHeight("80%", Dimension.Unset, 250),
                                    FontSize = 32,
                                    Background =
                                        new ColorBrush(new Color(0x00_ff_ff_80,
                                            Color.ColorType.RGBA)),
                                    WordWrap = true,
                                    Position = new Dimension2(0, 0)
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
    }
}
