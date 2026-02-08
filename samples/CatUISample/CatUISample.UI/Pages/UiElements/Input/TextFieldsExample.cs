using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Input;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;

namespace CatUISample.UI.Pages.UiElements.Input;

public class TextFieldsExample : ScrollContainer
{
    public TextFieldsExample()
    {
        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");

        Content = new PaddingElement(new EdgeInset(0, 5))
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
            Children =
            [
                new ColumnContainer
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                    Arrangement = LinearArrangement.SpacedBy(5),
                    Children =
                    [
                        new Label("Text fields examples", TextAlignmentType.Center)
                        {
                            Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                            FontSize = 32,
                            TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                        },
                        new TextField
                        {
                            Layout =
                                new ElementLayout()
                                    .SetMinMaxAndPreferredWidth(250, 250, Dimension.Unset)
                                    .SetFixedHeight(32)
                        }
                    ]
                }
            ]
        };
    }
}
