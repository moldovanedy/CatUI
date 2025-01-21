using System;
using System.Collections.Generic;
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
            _window.Document.Root = new Element(
                preferredWidth: new Dimension(100, Unit.Percent),
                preferredHeight: new Dimension(100, Unit.Percent));

            _window.Document.Root.AddChildren(
                new Rectangle(
                    position: new Dimension2(10, 5),
                    preferredWidth: new Dimension(80, Unit.Percent),
                    preferredHeight: new Dimension(20, Unit.Percent),
                    minWidth: 10,
                    maxWidth: 350,
                    minHeight: 20,
                    maxHeight: 250,
                    fillBrush: new ColorBrush(new Color(0x00_ff_ff)),
                    children:
                    [
                        new ImageView(
                            image,
                            position: "20dp 20dp",
                            preferredWidth: "50%",
                            preferredHeight: "50%",
                            themeOverrides:
                            ThemeBuilder<ImageViewThemeData>
                                .New()
                                .AddData(
                                    LabelThemeData.STYLE_NORMAL,
                                    new ImageViewThemeData { Background = new ColorBrush(new Color(0xff_00_ff)) })
                                .Build())
                    ]),
                new Rectangle(
                    position: new Dimension2(
                        10, new Dimension(60, Unit.Percent)),
                    preferredWidth: new Dimension(80, Unit.Percent),
                    preferredHeight: new Dimension(20, Unit.Percent),
                    fillBrush: new ColorBrush(new Color(0xff_ff_00)),
                    children:
                    [
                        new GeometricPath(
                            svgPath:
                            "M0,0.054V20h21V0.054H0z M15.422,18.129l-5.264-2.768l-5.265,2.768l1.006-5.863L1.64,8.114l5.887-0.855l2.632-5.334l2.633,5.334l5.885,0.855l-4.258,4.152L15.422,18.129z",
                            position: "5 10",
                            shouldApplyScaling: true,
                            preferredWidth: new Dimension(25, Unit.Percent),
                            preferredHeight: new Dimension(15, Unit.Percent),
                            fillBrush: new ColorBrush(new Color(0xff_98_00)),
                            outlineBrush: new ColorBrush(new Color(0x21_96_f3)),
                            outlineParameters: new OutlineParams(
                                4,
                                LineCapType.Round,
                                miterLimit: 5),
                            themeOverrides:
                            ThemeBuilder<ElementThemeData>
                                .New()
                                .AddData(
                                    ElementThemeData.STYLE_NORMAL,
                                    new ElementThemeData() { Background = new ColorBrush(new Color(0xff_ff_ff)) })
                                .Build()),
                        new Rectangle(
                            position: new Dimension2(
                                new Dimension(55, Unit.Percent),
                                new Dimension(10, Unit.Percent)),
                            preferredWidth: new Dimension(35, Unit.Percent),
                            preferredHeight: new Dimension(15, Unit.Percent),
                            fillBrush: new ColorBrush(new Color(0x1d_ea_85))
                        ),
                        new Label(
                            "He\u00adllo wor\u00adld!\nHe\u00adllo wor\u00adld!",
                            wordWrap: true,
                            position: new Dimension2(0, 0),
                            preferredWidth: new Dimension(25, Unit.Percent),
                            maxHeight: "150px",
                            //allowsExpansion: false,
                            themeOverrides:
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
                                .Build()
                        )
                    ]
                )
            );

            _window.Run();
        }
    }
}
