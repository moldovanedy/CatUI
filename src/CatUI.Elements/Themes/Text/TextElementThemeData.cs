using CatUI.Data;

namespace CatUI.Elements.Themes.Text
{
    public class TextElementThemeData : ElementThemeData
    {
        public Color TextColor { get; set; } = new Color(0x00_00_00_ff);
        public Color OutlineColor { get; set; } = new Color();
        public Dimension FontSize { get; set; } = new Dimension(16);

        public TextElementThemeData() : base()
        { }
        public TextElementThemeData(string forState) : base(forState)
        { }
    }
}