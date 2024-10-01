using SkiaSharp;

namespace CatUI.Data.Brushes
{
    public class ColorBrush : IBrush
    {
        public ColorBrush() { }
        public ColorBrush(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }

        public SKPaint ToSkiaPaint()
        {
            return new SKPaint()
            {
                Color = this.Color,
            };
        }
    }
}
