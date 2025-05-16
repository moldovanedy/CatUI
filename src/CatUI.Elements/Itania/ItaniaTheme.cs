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
        public static Theme BuildFinalTheme()
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
                    container.InternalHorizontalScrollBar.ShouldDisplayButtons = false;
                    container.InternalHorizontalScrollBar.ClipPath =
                        new RoundedRectangleClipShape((Dimension)"50%");

                    container.InternalVerticalScrollBar.Layout?.SetFixedWidth(12);
                    container.InternalVerticalScrollBar.ShouldDisplayButtons = false;
                    container.InternalVerticalScrollBar.ClipPath =
                        new RoundedRectangleClipShape((Dimension)"50%");
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
