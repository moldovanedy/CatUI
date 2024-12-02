using System.Collections.Generic;

using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
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
        public const int GLFW_ANGLE_PLATFORM_TYPE = 0x00050002;

        public const int GLFW_ANGLE_PLATFORM_TYPE_NONE = 0x00037001;
        public const int GLFW_ANGLE_PLATFORM_TYPE_OPENGL = 0x00037002;
        public const int GLFW_ANGLE_PLATFORM_TYPE_OPENGLES = 0x00037003;
        public const int GLFW_ANGLE_PLATFORM_TYPE_D3D9 = 0x00037004;
        public const int GLFW_ANGLE_PLATFORM_TYPE_D3D11 = 0x00037005;
        public const int GLFW_ANGLE_PLATFORM_TYPE_VULKAN = 0x00037007;
        public const int GLFW_ANGLE_PLATFORM_TYPE_METAL = 0x00037008;

        public const int EGL_PLATFORM_ANGLE_TYPE_VULKAN_ANGLE = 0x3450;

        private static Window? window;

        private static void Main()
        {
            // AssetsManager.AddAssetAssembly(Assembly.GetExecutingAssembly());
            // Image? image = AssetsManager.LoadFromAssembly<Image>("/Assets/search_128px.png");
            // if (image != null)
            // {
            //     Debug.WriteLine(image.ToString());
            // }
            // else
            // {
            //     Debug.WriteLine("NULL");
            // }

            window = new Window(
                width: 800,
                height: 600,
                minWidth: 300,
                minHeight: 200,
                title: "Test");

            window.Document.BackgroundColor = new Color(0x21_21_21);
            window.Document.Root = new Element(
                preferredWidth: new Dimension(100, Unit.Percent),
                preferredHeight: new Dimension(100, Unit.Percent));

            window.Document.Root.AddChildren(
                new Rectangle(
                    position: new Dimension2(10, 5),
                    preferredWidth: new Dimension(80, Unit.Percent),
                    preferredHeight: new Dimension(20, Unit.Percent),
                    minWidth: 10,
                    maxWidth: 350,
                    minHeight: 20,
                    maxHeight: 250,
                    fillBrush: new ColorBrush(new Color(0x00_ff_ff))),
                new Rectangle(
                    position: new Dimension2(
                        10, new Dimension(60, Unit.Percent)),
                    preferredWidth: new Dimension(80, Unit.Percent),
                    preferredHeight: new Dimension(20, Unit.Percent),
                    fillBrush: new ColorBrush(new Color(0xff_ff_00)),

                    children: [
                        new GeometricPath(
                            svgPath:
                                $"M25 35 L45 60 A40 40 29 1 1 25 25",
                            position: "5 10",
                            preferredWidth: new Dimension(25, Unit.Percent),
                            preferredHeight: new Dimension(15, Unit.Percent),
                            fillBrush: new ColorBrush(new Color(0xff_98_00)),
                            outlineBrush: new ColorBrush(new Color(0x21_96_f3)),
                            outlineParameters: new OutlineParams(
                                outlineWidth: 4,
                                lineCap: LineCapType.Round,
                                miterLimit: 5)
                        ),
                        new Rectangle(
                            position: new Dimension2(
                                new Dimension(55, Unit.Percent),
                                new Dimension(10, Unit.Percent)),
                            preferredWidth: new Dimension(35, Unit.Percent),
                            preferredHeight: new Dimension(15, Unit.Percent),
                            fillBrush: new ColorBrush(new Color(0x1d_ea_85))
                        ),
                        new Label(
                            text: "He\u00adllo wor\u00adld!\nHe\u00adllo wor\u00adld!",
                            wordWrap: true,
                            position: new Dimension2(0, 0),
                            preferredWidth: new Dimension(25, Unit.Percent),
                            maxHeight: "150px",
                            //allowsExpansion: false,
                            themeOverrides: new ThemeDefinition<LabelThemeData>(new Dictionary<string, LabelThemeData>()
                            {
                                {
                                    Label.STYLE_NORMAL,
                                    new LabelThemeData(Label.STYLE_NORMAL)
                                    {
                                        FontSize = 32,
                                        Background = new ColorBrush(new Color(0x00_ff_ff_80, Color.ColorType.RGBA))
                                    }
                                }
                            })
                        ),
                    ]
                )
            );

            window.Run();
        }
    }
}
