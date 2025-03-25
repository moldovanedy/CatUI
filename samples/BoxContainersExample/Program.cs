using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Elements.Containers;
using CatUI.Elements.Shapes;
using CatUI.Windowing.Desktop;

namespace BoxContainersExample
{
    internal static class Program
    {
        private static void Main()
        {
            CatApplication.NewBuilder().Build();

            var window = new DesktopWindow(
                900,
                600,
                "BoxContainers example",
                150,
                minHeight: 200);

            window.Document.BackgroundColor = new Color(0x21_21_21);
            window.Document.Root = new ColumnContainer();

            window.Document.Root.Children.AddItems(
            [
                new Rectangle
                {
                    FillBrush = new ColorBrush(new Color(0x80_00_ff)),
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(50)
                },
                new Rectangle
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetMinMaxHeight(250, Dimension.Unset),
                    FillBrush = new ColorBrush(new Color(0x75_75_75)),
                    ElementContainerSizing = new ColumnContainerSizing(1f)
                },
                new Rectangle
                {
                    FillBrush = new ColorBrush(new Color(0x40_00_80)),
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("15%")
                }
                // new Rectangle(
                //     maxHeight: "400dp",
                //     preferredHeight: "5%",
                //     minHeight: "250dp",
                //     fillBrush: new ColorBrush(new Color(0x75_75_75)),
                //     elementContainerSizing: new ColumnContainerSizing(vGrowthFactor: 3)
                // ),
            ]);

            window.Open();
            window.Run();
        }
    }
}
