using System;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Elements;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.ControlFlow;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.Utils;
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
        private static readonly ObjectRef<SwitchElement<int>> _switchElementRef = new();

        private static readonly ObservableList<string> _foreachData = ["A", "B", "C"];

        private static void Main()
        {
            ObservableProperty<bool> ifElseState = new(true);

            try
            {
                Init();
                //var font = AssetsManager.LoadFromAssembly<FontAsset>("/Assets/NotoSerif-Italic.ttf", typeof(Program));

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
                                new Button("If/else test", 22, new ColorBrush(new Color(0xff_ff_ff)))
                                {
                                    Layout = new ElementLayout().SetFixedWidth(150).SetFixedHeight(40),
                                    Background = new ColorBrush(new Color(0x42_42_42)),
                                    OnClick = (_, _) => ifElseState.Value = !ifElseState.Value
                                },
                                new Button("Switch test", 22, new ColorBrush(new Color(0xff_ff_ff)))
                                {
                                    Layout = new ElementLayout().SetFixedWidth(150).SetFixedHeight(40),
                                    OnClick = (_, _) => _switchElementRef.Value!.Value = Random.Shared.Next(0, 20),
                                    Background = new ColorBrush(new Color(0x42_42_42))
                                }
                            ]
                        },
                        new Spacer(50, Orientation.Vertical),
                        new RowContainer
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%"),
                            Arrangement = LinearArrangement.SpacedBy(15),
                            Children =
                            [
                                //if/else
                                new IfElement(
                                    ifElseState,
                                    new RectangleElement(new ColorBrush(new Color(0x00_00_ff)))
                                    {
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                        Children =
                                        [
                                            new GeometricPathElement(
                                                "M0,0.054V20h21V0.054H0z M15.422,18.129l-5.264-2.768l-5.265,2.768l1.006-5.863L1.64,8.114l5.887-0.855\n\tl2.632-5.334l2.633,5.334l5.885,0.855l-4.258,4.152L15.422,18.129z",
                                                new ColorBrush(new Color(0xff_ff_ff)))
                                            {
                                                Layout = new ElementLayout()
                                                         .SetFixedWidth("100%")
                                                         .SetFixedHeight("100%"),
                                                ShouldApplyScaling = true
                                            }
                                        ]
                                    },
                                    new RectangleElement(new ColorBrush(new Color(0xff_00_00)))
                                    {
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                        Children =
                                        [
                                            new TextBlock("False")
                                            {
                                                Layout = new ElementLayout()
                                                         .SetFixedWidth("100%")
                                                         .SetFixedHeight("100%"),
                                                //Font = font,
                                                TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                                            }
                                        ]
                                    }) { Layout = new ElementLayout().SetFixedWidth("20%").SetFixedHeight(50) },

                                //foreach
                                new ForEachElement<string>(
                                    new ColumnContainer
                                    {
                                        Arrangement = LinearArrangement.SpacedBy(5),
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                                    },
                                    _foreachData,
                                    (_, item) => new Element
                                    {
                                        Layout = new ElementLayout().SetFixedWidth(70).SetFixedHeight(40),
                                        Background = new ColorBrush(new Color(0x61_61_61)),
                                        Children =
                                        [
                                            new TextBlock($"El {item}")
                                            {
                                                Layout =
                                                    new ElementLayout()
                                                        .SetFixedWidth("100%")
                                                        .SetFixedHeight("100%"),
                                                TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                                            }
                                        ]
                                    }) { Layout = new ElementLayout().SetFixedWidth(70).SetFixedHeight(500) },

                                //switch
                                new SwitchElement<int>(
                                    0,
                                    [
                                        new SwitchElement<int>.ExactCaseLabel(1, _ => CreateSwitchElement(1)),
                                        new SwitchElement<int>.ExactCaseLabel(2, _ => CreateSwitchElement(2)),
                                        new SwitchElement<int>.ExactCaseLabel(3, _ => CreateSwitchElement(3)),
                                        new SwitchElement<int>.ExactCaseLabel(4, _ => CreateSwitchElement(4)),
                                        new SwitchElement<int>.EvaluationCaseLabel(
                                            x => x >= 5 && x <= 10,
                                            CreateSwitchElementRange),
                                        new SwitchElement<int>.EvaluationCaseLabel(
                                            x => x >= 10 && x <= 20,
                                            CreateSwitchElementRange)
                                    ],
                                    new Element
                                    {
                                        Layout = new ElementLayout().SetFixedWidth(130).SetFixedHeight(40),
                                        Background = new ColorBrush(new Color(0x61_61_61)),
                                        Children =
                                        [
                                            new TextBlock("Default")
                                            {
                                                Layout = new ElementLayout().SetFixedWidth("100%")
                                                                            .SetFixedHeight("100%"),
                                                TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                                            }
                                        ]
                                    })
                                {
                                    Ref = _switchElementRef,
                                    Layout = new ElementLayout().SetFixedWidth(130).SetFixedHeight(40)
                                }
                            ]
                        }
                    ]
                };

                CancellationTokenSource cts = new();
                _ = ForeachTesterAsync(cts.Token);

                _window.Open();
                _window.Run();

                cts.Cancel();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
            }
        }

        private static async Task ForeachTesterAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                char lastLetter = 'D';
                while (true)
                {
                    await Task.Delay(1500, token);
                    _foreachData.RemoveAt(0);
                    _foreachData.Add(lastLetter.ToString());
                    lastLetter = (char)(lastLetter + 1);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static Element CreateSwitchElement(int value)
        {
            return new Element
            {
                Layout = new ElementLayout().SetFixedWidth(130).SetFixedHeight(40),
                Background = new ColorBrush(new Color(0x61_61_61)),
                Children =
                [
                    new TextBlock($"Case {value}")
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                        TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                    }
                ]
            };
        }

        private static Element CreateSwitchElementRange(int actualValue)
        {
            return new Element
            {
                Layout = new ElementLayout().SetFixedWidth(130).SetFixedHeight(40),
                Background = new ColorBrush(new Color(0x61_61_61)),
                Children =
                [
                    new TextBlock($"Eval case: {actualValue}")
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                        TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                    }
                ]
            };
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
