using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Elements;
using CatUI.Elements.Containers;
using CatUI.Elements.Shapes;
using CatUI.Windowing.Desktop;

namespace BoxContainersExample
{
    sealed class Program
    {
        static void Main(string[] args)
        {
            Window window = new Window(
                width: 900,
                height: 600,
                title: "BoxContainers example",
                minWidth: 250,
                minHeight: 200);

            window.Document.BackgroundColor = new Color(0x21_21_21);
            window.Document.Root = new HBoxContainer(
                preferredWidth: "100%",
                preferredHeight: "100%");

            window.Document.Root.AddChildren(
                new List<Element>() {
                    new Rectangle(
                        preferredWidth: "50dp",
                        fillBrush: new ColorBrush(new Color(0xff_00_ff))
                    ),
                    new Rectangle(
                        preferredWidth: "100dp",
                        minWidth: "250dp",
                        fillBrush: new ColorBrush(new Color(0xff_ff_ff)),
                        elementContainerSizing: new HBoxContainerSizing(hGrowthFactor: 1)
                    ),
                    new Rectangle(
                        preferredWidth: "75dp",
                        fillBrush: new ColorBrush(new Color(0xff_00_00))
                    )
                });

            window.Run();
        }
    }
}
