using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Themes.Text
{
    public class TextElementThemeData : ElementThemeData
    {
        public IBrush FillBrush { get; set; } = new ColorBrush(new Color(0x00_00_00));
        public IBrush OutlineBrush { get; set; } = new ColorBrush();
        public Dimension FontSize { get; set; } = new(16);

        /// <summary>
        /// A dimensionless value that will be multiplied with <see cref="FontSize"/> to get the element's line height.
        /// Default is 1.2.
        /// </summary>
        public float LineHeight { get; set; } = 1.2f;

        public TextElementThemeData()
        {
        }

        public TextElementThemeData(string forState) : base(forState)
        {
        }
    }
}
