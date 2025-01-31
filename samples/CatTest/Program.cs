using System;
using System.Reflection;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Elements;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;
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

        private static Window? _window;

        private static void Main()
        {
            AssetsManager.AddAssetAssembly(Assembly.GetExecutingAssembly());

            var image = AssetsManager.LoadFromAssembly<Image>("/Assets/search_128px.png");
            if (image == null)
            {
                throw new NullReferenceException("Image is null");
            }

            _window = new Window(
                800,
                600,
                minWidth: 300,
                minHeight: 200,
                title: "Test");

            _window.Document.BackgroundColor = new Color(0x21_21_21);
            _window.Document.Root = new Element();

            _window.Document.Root.Children =
            [
                new Rectangle(
                    new ColorBrush(new Color(0x00_ff_ff)),
                    preferredWidth: "80%",
                    preferredHeight: "20%")
                {
                    Position = "10 5",
                    MinWidth = 10,
                    MaxWidth = 350,
                    MinHeight = 20,
                    MaxHeight = 250,
                    Children =
                    [
                        new ImageView(
                            image,
                            ThemeBuilder<ImageViewThemeData>
                                .New()
                                .AddData(
                                    LabelThemeData.STYLE_NORMAL,
                                    new ImageViewThemeData
                                    {
                                        Background =
                                            new ColorBrush(new Color(0xff_00_ff))
                                    })
                                .Build(),
                            "50%",
                            "50%") { Position = "20 20" }
                    ]
                },
                new Rectangle(
                    new ColorBrush(new Color(0xff_ff_00)),
                    preferredWidth: "80%",
                    preferredHeight: "20%")
                {
                    Position = new Dimension2(10, "60%"),
                    Children =
                    [
                        new GeometricPath(
                            "M0,0.054V20h21V0.054H0z M15.422,18.129l-5.264-2.768l-5.265,2.768l1.006-5.863L1.64,8.114l5.887-0.855l2.632-5.334l2.633,5.334l5.885,0.855l-4.258,4.152L15.422,18.129z",
                            new ColorBrush(new Color(0xff_98_00)),
                            new ColorBrush(new Color(0x21_96_f3)),
                            ThemeBuilder<ElementThemeData>
                                .New()
                                .AddData(
                                    ElementThemeData.STYLE_NORMAL,
                                    new ElementThemeData() { Background = new ColorBrush(new Color(0xff_ff_ff)) })
                                .Build(),
                            "25%",
                            "15%")
                        {
                            Position = "5 10",
                            ShouldApplyScaling = true,
                            OutlineParameters = new OutlineParams(
                                2,
                                LineCapType.Round,
                                miterLimit: 5)
                        },
                        new Rectangle(
                            new ColorBrush(new Color(0x1d_ea_85)),
                            preferredWidth: "35%",
                            preferredHeight: "15%") { Position = "55% 10%" },
                        new Label(
                            "He\u00adllo wor\u00adld!\nHe\u00adllo wor\u00adld!",
                            themeOverrides: ThemeBuilder<LabelThemeData>
                                            .New()
                                            .AddData(
                                                LabelThemeData.STYLE_NORMAL,
                                                new LabelThemeData
                                                {
                                                    FontSize = 32,
                                                    Background =
                                                        new ColorBrush(new Color(0x00_ff_ff_80, Color.ColorType.RGBA))
                                                })
                                            .Build(),
                            preferredWidth: "25%",
                            preferredHeight: "80%")
                        {
                            WordWrap = true, Position = new Dimension2(0, 0), MaxHeight = "150", AllowsExpansion = false
                        }
                    ]
                }
            ];

            _window.Run();
        }
    }
}
