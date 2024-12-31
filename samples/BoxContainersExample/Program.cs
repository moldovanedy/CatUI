using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Elements;
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
            
            Window window = new Window(
                width: 900,
                height: 600,
                title: "BoxContainers example",
                minWidth: 150,
                minHeight: 200);

            window.Document.BackgroundColor = new Color(0x21_21_21);
            window.Document.Root = new VBoxContainer();

            window.Document.Root.AddChildren(
                new List<Element>() {
                    new Rectangle(
                        preferredHeight: "50dp",
                        fillBrush: new ColorBrush(new Color(0x80_00_ff))
                    ),
                    new Rectangle(
                        //maxHeight: "400dp",
                        //preferredHeight: "5%",
                        minHeight: "250dp",
                        fillBrush: new ColorBrush(new Color(0x75_75_75)),
                        elementContainerSizing: new VBoxContainerSizing(vGrowthFactor: 1f)
                    ),
                    new Rectangle(
                        preferredHeight: "15%",
                        fillBrush: new ColorBrush(new Color(0x40_00_80))
                    ),
                    // new Rectangle(
                    //     maxHeight: "400dp",
                    //     preferredHeight: "5%",
                    //     minHeight: "250dp",
                    //     fillBrush: new ColorBrush(new Color(0x75_75_75)),
                    //     elementContainerSizing: new VBoxContainerSizing(vGrowthFactor: 3)
                    // ),
                });

            window.Run();
        }
    }
}
