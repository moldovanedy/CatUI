using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Themes
{
    public class ElementThemeData
    {
        public string ForState { get; }
        public IBrush Background { get; set; } = new ColorBrush();
        public CornerInset CornerRadius { get; set; } = new CornerInset();

        public ElementThemeData()
        {
            ForState = Element.STYLE_NORMAL;
        }
        public ElementThemeData(string forState)
        {
            ForState = forState;
        }
    }
}