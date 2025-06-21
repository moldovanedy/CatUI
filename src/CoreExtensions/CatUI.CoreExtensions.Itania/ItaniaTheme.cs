using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Shapes;
using CatUI.Data.Theming;
using CatUI.Elements;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;

namespace CatUI.CoreExtensions.Itania
{
    public static class ItaniaTheme
    {
        public static Theme GetTheme()
        {
            Theme theme = new();

            //text
            theme.AddOrUpdateElementTypeDefinition<TextElement>(
                new ThemeDefinition(el => ((TextElement)el).FontSize = "1em"));

            theme.AddOrUpdateElementTypeDefinition<TextBlock>(
                new ThemeDefinition(el => ((TextBlock)el).TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)));

            //scroll
            theme.AddOrUpdateElementTypeDefinition<ScrollContainer>(
                new ThemeDefinition(el =>
                {
                    var container = (ScrollContainer)el;
                    container.InternalHorizontalScrollBar.Layout?.SetFixedHeight(12);
                    container.InternalVerticalScrollBar.Layout?.SetFixedWidth(12);

                    Theme scrollBarTheme = new();
                    scrollBarTheme.AddOrUpdateElementTypeDefinition<ScrollBarBase>(
                        new ThemeDefinition(e =>
                        {
                            var scrollBar = (ScrollBarBase)e;
                            scrollBar.ShouldDisplayButtons = false;

                            scrollBar.InternalThumbElement.ClipPath
                                = new RoundedRectangleClipShape(new Dimension("50%"));

                            //override the button style from the scroll bar itself
                            Theme selfButtonTheme = new();
                            selfButtonTheme.AddOrUpdateElementTypeDefinition<Button>(
                                new ThemeDefinition(barButton =>
                                {
                                    var btn = (Button)barButton;
                                    btn.ClipPath = null;
                                }));
                            scrollBar.ThemeOverride = selfButtonTheme;
                        }));

                    container.InternalHorizontalScrollBar.ThemeOverride = scrollBarTheme;
                    container.InternalVerticalScrollBar.ThemeOverride = scrollBarTheme;
                }));

            //button
            theme.AddOrUpdateElementTypeDefinition<Button>(
                new ThemeDefinition(el =>
                {
                    var btn = (Button)el;
                    btn.Spacing = 10;
                    btn.Padding = new EdgeInset(5, 7);
                    btn.ClipPath = new RoundedRectangleClipShape(8);
                    btn.Background = new ColorBrush(CatTheme.Colors.Primary);
                }));

            //checkbox
            theme.AddOrUpdateElementTypeDefinition<CheckBox>(
                new ThemeDefinition(el =>
                {
                    //TODO: Itania needs to provide its own indicator element, as the default one has direct local
                    //properties set, meaning we can't modify those from a theme 
                    var checkBox = (CheckBox)el;
                    checkBox.Spacing = 10;

                    Theme checkboxIndicatorTheme = new();

                    var boxOutlineTheme = new ThemeDefinition(boxOutline =>
                    {
                        var rect = (RectangleElement)boxOutline;
                        rect.OutlineBrush = new ColorBrush(CatTheme.Colors.Outline);
                    });
                    checkboxIndicatorTheme.AddOrUpdateClassDefinition(
                        "CheckBox::Indicator::Checked::Outer", boxOutlineTheme);
                    checkboxIndicatorTheme.AddOrUpdateClassDefinition(
                        "CheckBox::Indicator::Unchecked::Outer", boxOutlineTheme);
                    checkboxIndicatorTheme.AddOrUpdateClassDefinition(
                        "CheckBox::Indicator::Indeterminate::Outer", boxOutlineTheme);

                    var boxColoredSectionTheme = new ThemeDefinition(boxColoredSection =>
                    {
                        var rect = (RectangleElement)boxColoredSection;
                        rect.FillBrush = new ColorBrush(CatTheme.Colors.Primary);
                    });
                    checkboxIndicatorTheme.AddOrUpdateClassDefinition(
                        "CheckBox::Indicator::Checked::Inner", boxColoredSectionTheme);
                    checkboxIndicatorTheme.AddOrUpdateClassDefinition(
                        "CheckBox::Indicator::Indeterminate::Inner", boxColoredSectionTheme);

                    checkBox.IndicatorElement.ThemeOverride = checkboxIndicatorTheme;
                }));
            return theme;
        }
    }
}
