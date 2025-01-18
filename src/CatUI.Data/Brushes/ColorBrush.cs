using CatUI.Data.Managers;
using SkiaSharp;

namespace CatUI.Data.Brushes
{
    public class ColorBrush : CatObject, IBrush
    {
        public bool IsSkippable => Color.A == 0;

        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Creates a brush with a completely transparent color.
        /// </summary>
        public ColorBrush() { }

        public ColorBrush(Color color)
        {
            Color = color;
        }

        public override ColorBrush Duplicate()
        {
            return new ColorBrush(Color);
        }

        public SKPaint ToSkiaPaint()
        {
            return PaintManager.GetPaint(paintColor: Color);
        }
    }
}
