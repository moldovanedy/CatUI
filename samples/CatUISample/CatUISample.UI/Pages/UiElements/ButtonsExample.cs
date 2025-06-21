using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;

namespace CatUISample.UI.Pages.UiElements
{
    public class ButtonsExample : ScrollContainer
    {
        public ButtonsExample()
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");

            Content = new ColumnContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                Children =
                [
                    new TextBlock("Buttons examples", TextAlignmentType.Center)
                    {
                        Layout = new ElementLayout().SetMinMaxWidth(0, "100%", true),
                        FontSize = 32,
                        TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                    },
                    new CheckBox(
                        CheckBox.CheckBoxState.Indeterminate,
                        "Checkbox 1",
                        16,
                        new ColorBrush(CatTheme.Colors.OnSurface))
                    {
                        Layout = new ElementLayout().SetMinMaxWidth(100, "100%", true).SetFixedHeight(20)
                    },
                    new RadioButton(
                        true,
                        "Radio button 1",
                        16,
                        new ColorBrush(CatTheme.Colors.OnSurface))
                    {
                        Layout = new ElementLayout().SetMinMaxWidth(100, "100%", true).SetFixedHeight(20)
                    },
                    new SwitchButton(
                        true,
                        "Switch button 1",
                        16,
                        new ColorBrush(CatTheme.Colors.OnSurface))
                    {
                        Layout = new ElementLayout().SetMinMaxWidth(100, "100%", true).SetFixedHeight(20)
                    }
                ]
            };
        }
    }
}
