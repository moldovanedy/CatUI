using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;

namespace CatUISample.UI.Pages.Layout
{
    public class ScrollContainerExamples : ColumnContainer
    {
        public ScrollContainerExamples()
        {
            Layout = new ElementLayout().SetFixedWidth("100%");
            Arrangement = LinearArrangement.SpacedBy(20);
        }

        protected override void EnterDocument(object sender)
        {
            base.EnterDocument(sender);

            Children =
            [
                new TextBlock("ScrollContainer examples", TextAlignmentType.Center)
                {
                    Layout = new ElementLayout().SetMinMaxWidth(0, "100%", true),
                    FontSize = 32,
                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                },
                new ScrollContainer
                {
                    Layout = new ElementLayout().SetFixedWidth(300).SetFixedHeight(400),
                    Background = new ColorBrush(CatTheme.Colors.SurfaceContainer),
                    Content = new PaddingElement(new EdgeInset(0, 10))
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                        Children =
                        [
                            new ColumnContainer
                            {
                                Layout = new ElementLayout().SetFixedWidth("100%"),
                                Children =
                                [
                                    new RectangleElement(new ColorBrush(CatTheme.Colors.Primary))
                                    {
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(200)
                                    },
                                    new RectangleElement(new ColorBrush(CatTheme.Colors.Tertiary))
                                    {
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(200)
                                    },
                                    new RectangleElement(new ColorBrush(CatTheme.Colors.Primary))
                                    {
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(200)
                                    },
                                    new RectangleElement(new ColorBrush(CatTheme.Colors.Tertiary))
                                    {
                                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(200)
                                    }
                                ]
                            }
                        ]
                    }
                }
            ];
        }
    }
}
