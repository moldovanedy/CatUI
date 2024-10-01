using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Styles
{
    public class ElementStyle
    {
        public IBrush Background { get; set; } = new ColorBrush();
        public CornerInset CornerRadius { get; set; } = new CornerInset();
    }
}
