using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;

namespace CatUISample.UI.Pages.UiElements
{
    public class ButtonsExample : ScrollContainer
    {
        public ButtonsExample()
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
                            new TextBlock("Buttons examples", TextAlignmentType.Center)
                            {
                                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                                FontSize = 32,
                                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                            },
                            new CheckBox(
                                CheckBox.CheckBoxState.Indeterminate,
                                "Checkbox 1",
                                16,
                                new ColorBrush(CatTheme.Colors.OnSurface))
                            {
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 100, "100%")
                                        .SetFixedHeight(20)
                            },
                            new RadioButton(
                                true,
                                "Radio button 1",
                                16,
                                new ColorBrush(CatTheme.Colors.OnSurface))
                            {
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 100, "100%")
                                        .SetFixedHeight(20)
                            },
                            new SwitchButton(
                                true,
                                "Switch button 1",
                                16,
                                new ColorBrush(CatTheme.Colors.OnSurface))
                            {
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 100, "100%")
                                        .SetFixedHeight(20)
                            }
                        ]
                    }
                ]
            };
        }
    }
}
