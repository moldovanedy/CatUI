using System;
using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Themes
{
    public class ElementThemeData : ThemeData
    {
        public IBrush Background { get; set; } = new ColorBrush();
        public CornerInset CornerRadius { get; set; } = new CornerInset();

        public ElementThemeData() : base()
        { }
        public ElementThemeData(string forState) : base(forState)
        { }
    }
}