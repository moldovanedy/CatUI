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
    internal sealed class Program
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
            _window.Document.Root = new Element
            {
                PreferredWidth = new Dimension(100, Unit.Percent),
                PreferredHeight = new Dimension(100, Unit.Percent)
            };

            _window.Document.Root.Children =
            [
                new Rectangle
                {
                    Position = new Dimension2(10, 5),
                    PreferredWidth = new Dimension(80, Unit.Percent),
                    PreferredHeight = new Dimension(20, Unit.Percent),
                    MinWidth = 10,
                    MaxWidth = 350,
                    MinHeight = 20,
                    MaxHeight = 250,
                    FillBrush = new ColorBrush(new Color(0x00_ff_ff)),
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
                                .Build()) { Position = "20dp 20dp", PreferredWidth = "50%", PreferredHeight = "50%" }
                    ]
                },
                new Rectangle
                {
                    Position = new Dimension2(10, new Dimension(60, Unit.Percent)),
                    PreferredWidth = new Dimension(80, Unit.Percent),
                    PreferredHeight = new Dimension(20, Unit.Percent),
                    FillBrush = new ColorBrush(new Color(0xff_ff_00)),
                    Children =
                    [
                        new GeometricPath(
                            ThemeBuilder<ElementThemeData>
                                .New()
                                .AddData(
                                    ElementThemeData.STYLE_NORMAL,
                                    new ElementThemeData() { Background = new ColorBrush(new Color(0xff_ff_ff)) })
                                .Build())
                        {
                            SvgPath =
                                "M0,0.054V20h21V0.054H0z M15.422,18.129l-5.264-2.768l-5.265,2.768l1.006-5.863L1.64,8.114l5.887-0.855l2.632-5.334l2.633,5.334l5.885,0.855l-4.258,4.152L15.422,18.129z",
                            Position = "5 10",
                            ShouldApplyScaling = true,
                            PreferredWidth = new Dimension(25, Unit.Percent),
                            PreferredHeight = new Dimension(15, Unit.Percent),
                            FillBrush = new ColorBrush(new Color(0xff_98_00)),
                            OutlineBrush = new ColorBrush(new Color(0x21_96_f3)),
                            OutlineParameters = new OutlineParams(
                                4,
                                LineCapType.Round,
                                miterLimit: 5)
                        },
                        new Rectangle
                        {
                            Position = new Dimension2(
                                new Dimension(55, Unit.Percent),
                                new Dimension(10, Unit.Percent)),
                            PreferredWidth = new Dimension(35, Unit.Percent),
                            PreferredHeight = new Dimension(15, Unit.Percent),
                            FillBrush = new ColorBrush(new Color(0x1d_ea_85))
                        },
                        new Label(
                            "He\u00adllo wor\u00adld!\nHe\u00adllo wor\u00adld!",
                            ThemeBuilder<LabelThemeData>
                                .New()
                                .AddData(
                                    LabelThemeData.STYLE_NORMAL,
                                    new LabelThemeData
                                    {
                                        FontSize = 32,
                                        Background =
                                            new ColorBrush(new Color(0x00_ff_ff_80, Color.ColorType.RGBA))
                                    })
                                .Build())
                        {
                            WordWrap = true,
                            Position = new Dimension2(0, 0),
                            PreferredWidth = new Dimension(25, Unit.Percent),
                            MaxHeight = "150"
                            //AllowsExpansion: false,
                        }
                    ]
                }
            ];

            _window.Run();
        }
    }
}
