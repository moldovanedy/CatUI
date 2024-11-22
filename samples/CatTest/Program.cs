using System.Collections.Generic;
using System.Diagnostics;
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

            window.Document.BackgroundColor = new Color(0x21_21_21_ff);
            window.Document.Root = new Element(
                doc: window.Document,
                width: new Dimension(100, Unit.Percent),
                height: new Dimension(100, Unit.Percent));

            window.Document.Root.AddChildren(
                new Rectangle(
                    position: new Dimension2(10, 5),
                    width: new Dimension(80, Unit.Percent),
                    height: new Dimension(20, Unit.Percent),
                    minWidth: 10,
                    maxWidth: 350,
                    minHeight: 20,
                    maxHeight: 250,
                    fillBrush: new ColorBrush(new Color(0x00_ff_ff_ff))),
                new Rectangle(
                    doc: window.Document,
                    position: new Dimension2(
                        10, new Dimension(60, Unit.Percent)),
                    width: new Dimension(80, Unit.Percent),
                    height: new Dimension(20, Unit.Percent),
                    fillBrush: new ColorBrush(new Color(0xff_ff_00_ff)),
                    children: [
                        new GeometricPath(
                            position: "5 10",
                            width: new Dimension(25, Unit.Percent),
                            height: new Dimension(15, Unit.Percent),
                            fillBrush: new ColorBrush(new Color(0xff_98_00_ff)),
                            outlineBrush: new ColorBrush(new Color(0x21_96_f3_ff)),
                            outlineParameters: new OutlineParams(
                                outlineWidth: 4,
                                lineCap: LineCapType.Round,
                                miterLimit: 5)
                        ),
                        new Rectangle(
                            position: new Dimension2(
                                new Dimension(55, Unit.Percent),
                                new Dimension(10, Unit.Percent)),
                            width: new Dimension(35, Unit.Percent),
                            height: new Dimension(15, Unit.Percent),
                            fillBrush: new ColorBrush(new Color(0x1d_ea_85_ff))
                        ),
                        new Label(
                            text: "He\u00adllo wor\u00adld!\nHe\u00adllo wor\u00adld!",
                            wordWrap: true,
                            position: new Dimension2(0, 0),
                            width: new Dimension(25, Unit.Percent),
                            themeOverrides: new ThemeDefinition<LabelThemeData>(new Dictionary<string, LabelThemeData>()
                            {
                                {
                                    Label.STYLE_NORMAL,
                                    new LabelThemeData(Label.STYLE_NORMAL){
                                        FontSize = 32,
                                        Background = new ColorBrush(new Color(0x00_ff_ff_80))
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
