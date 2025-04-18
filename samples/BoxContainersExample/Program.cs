using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Shapes;
using CatUI.Windowing.Desktop;

namespace BoxContainersExample
{
    internal static class Program
    {
        private static void Main()
        {
            Init();

            DesktopWindow window = new DesktopWindow(
                900,
                600,
                "BoxContainers example",
                150,
                minHeight: 200);

            window.Document.BackgroundColor = new Color(0x21_21_21);
            window.Document.Root = new ColumnContainer();

            window.Document.Root.Children.AddItems(
            [
                new RectangleElement
                {
                    FillBrush = new ColorBrush(new Color(0x80_00_ff)),
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(50)
                },
                new RectangleElement
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetMinMaxHeight(250, Dimension.Unset),
                    FillBrush = new ColorBrush(new Color(0x75_75_75)),
                    ElementContainerSizing = new ColumnContainerSizing()
                },
                new RectangleElement
                {
                    FillBrush = new ColorBrush(new Color(0x40_00_80)),
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("15%")
                }
            ]);

            window.Open();
            window.Run();
        }

        public static void Init()
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
