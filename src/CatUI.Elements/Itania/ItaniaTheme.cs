using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Shapes;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;

namespace CatUI.Elements.Itania
{
    public static class ItaniaTheme
    {
        public static Theme GetTheme()
        {
            Theme theme = new();

            //text
            theme.AddOrUpdateElementTypeDefinition(
                typeof(TextElement),
                new ThemeDefinition(el => ((TextElement)el).FontSize = "1em"));

            theme.AddOrUpdateElementTypeDefinition(
                typeof(TextBlock),
                new ThemeDefinition(el => ((TextBlock)el).TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)));

            //scroll
            theme.AddOrUpdateElementTypeDefinition(
                typeof(ScrollContainer),
                new ThemeDefinition(el =>
                {
                    var container = (ScrollContainer)el;
                    container.InternalHorizontalScrollBar.Layout?.SetFixedHeight(12);
                    container.InternalVerticalScrollBar.Layout?.SetFixedWidth(12);

                    Theme scrollBarTheme = new();
                    scrollBarTheme.AddOrUpdateElementTypeDefinition(
                        typeof(ScrollBarBase),
                        new ThemeDefinition(e =>
                        {
                            var scrollBar = (ScrollBarBase)e;
                            scrollBar.ShouldDisplayButtons = false;

                            scrollBar.InternalThumbElement.ClipPath
                                = new RoundedRectangleClipShape(new Dimension("50%"));

                            //override the button style from the scroll bar itself
                            Theme selfButtonTheme = new();
                            selfButtonTheme.AddOrUpdateElementTypeDefinition(
                                typeof(Button),
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
            theme.AddOrUpdateElementTypeDefinition(
                typeof(Button),
                new ThemeDefinition(el =>
                {
                    var btn = (Button)el;
                    btn.Spacing = 10;
                    btn.Padding = new EdgeInset(5, 7);
                    btn.ClipPath = new RoundedRectangleClipShape(8);
                    btn.Background = new ColorBrush(CatTheme.Colors.Primary);
                }));
            return theme;
        }
    }
}
