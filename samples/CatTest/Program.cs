using System.Collections.Generic;
using System.Diagnostics;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Elements;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Themes.Text;
using CatUI.Windowing.Desktop;

namespace CatTest
{
    internal class Program
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
                    rectBrush: new ColorBrush(new Color(0x00_ff_ff_ff))),
                new Rectangle(
                    doc: window.Document,
                    position: new Dimension2(
                        10, new Dimension(60, Unit.Percent)),
                    width: new Dimension(80, Unit.Percent),
                    height: new Dimension(20, Unit.Percent),
                    rectBrush: new ColorBrush(new Color(0xff_ff_00_ff)),
                    children: [
                        new Rectangle(
                            position: new Dimension2(
                                new Dimension(5, Unit.Percent),
                                new Dimension(10, Unit.Percent)),
                            width: new Dimension(25, Unit.Percent),
                            height: new Dimension(15, Unit.Percent),
                            rectBrush: new ColorBrush(new Color(0xff_98_00_ff))
                        ),
                        new Rectangle(
                            position: new Dimension2(
                                new Dimension(55, Unit.Percent),
                                new Dimension(10, Unit.Percent)),
                            width: new Dimension(35, Unit.Percent),
                            height: new Dimension(15, Unit.Percent),
                            rectBrush: new ColorBrush(new Color(0x1d_ea_85_ff))
                        ),
                        new Label(
                            text: "He\u00adllo wor\u00adld!",
                            position: new Dimension2(
                                10,
                                new Dimension(55, Unit.Percent)),
                            width: new Dimension(25, Unit.Percent),
                            themeOverrides: new Dictionary<string, LabelThemeData>(){
                                {
                                    Label.STYLE_NORMAL,
                                    new LabelThemeData(Label.STYLE_NORMAL){
                                        FontSize = 32,
                                        TextColor = new Color(0x00_00_00_ff)
                                    }
                                }
                            }
                        ),
                    ]
                )
            );

            // window.Document.Root
            //     .SetDocument(window.Document)
            //     .SetInitialWidth(new Dimension(100, Unit.Percent))
            //     .SetInitialHeight(new Dimension(100, Unit.Percent))
            //     .Instantiate()
            //     .AddChildren(
            //         new Rectangle()
            //             .SetInitialBrush(new ColorBrush(new Color(0x00_ff_ff_ff)))
            //             .SetInitialPosition(new Dimension2(10, 5))
            //             .SetInitialWidth(new Dimension(80, Unit.Percent))
            //             .SetInitialHeight(new Dimension(20, Unit.Percent))
            //             .SetInitialMinWidth(10)
            //             .SetInitialMaxWidth(350)
            //             .SetInitialMinHeight(20)
            //             .SetInitialMaxHeight(250)
            //             .Instantiate(),
            //         new Rectangle()
            //             .SetInitialBrush(new ColorBrush(new Color(0x00_ff_00_ff)))
            //             .SetDocument(window.Document)
            //             .SetInitialPosition(new Dimension2(10, new Dimension(60, Unit.Percent)))
            //             .SetInitialWidth(new Dimension(80, Unit.Percent))
            //             .SetInitialHeight(new Dimension(20, Unit.Percent))
            //             .Instantiate()
            //             .AddChildren(
            //                 new Rectangle()
            //                     .SetInitialBrush(new ColorBrush(new Color(0xff_98_00_ff)))
            //                     .SetInitialPosition(new Dimension2(
            //                         new Dimension(5, Unit.Percent),
            //                         new Dimension(10, Unit.Percent)))
            //                     .SetInitialWidth(new Dimension(25, Unit.Percent))
            //                     .SetInitialHeight(new Dimension(15, Unit.Percent))
            //                     .Instantiate(),
            //                 new Rectangle()
            //                     .SetInitialBrush(new ColorBrush(new Color(0x1d_ea_85_ff)))
            //                     .SetInitialPosition(new Dimension2(
            //                         new Dimension(55, Unit.Percent),
            //                         new Dimension(10, Unit.Percent)))
            //                     .SetInitialWidth(new Dimension(35, Unit.Percent))
            //                     .SetInitialHeight(new Dimension(15, Unit.Percent))
            //                     .Instantiate(),
            //                 new Label()
            //                     .SetInitialText("He\u00adllo wor\u00adld!")
            //                     .SetInitialFontSize(32)
            //                     .SetInitialPosition(new Dimension2(
            //                         10,
            //                         new Dimension(55, Unit.Percent)))
            //                     .SetInitialWidth(new Dimension(25, Unit.Percent))
            //                     .SetInitialStyle(
            //                         new TextElementStyle()
            //                         {
            //                             TextColor = new Color(0x00_00_00_ff)
            //                         })
            //                     .Instantiate()
            //             )
            //     );

            window.RequestAnimationFrame(CallbackTest);

            window.Run();
        }

        private static void CallbackTest(double delta)
        {
            Debug.WriteLine(delta);
            //window?.RequestAnimationFrame(CallbackTest);
        }
    }
}
