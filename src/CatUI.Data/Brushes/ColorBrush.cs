using System.Runtime.InteropServices;
using CatUI.Data.Managers;
using SkiaSharp;

namespace CatUI.Data.Brushes
{
    public class ColorBrush : IBrush
    {
        public bool IsSkippable
        {
            get
            {
                return Color.A == 0;
            }
        }

        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Creates a brush with a completely transparent color.
        /// </summary>
        public ColorBrush() { }

        public ColorBrush(Color color)
        {
            Color = color;
        }

        public SKPaint ToSkiaPaint()
        {
            return PaintManager.GetPaint(paintColor: Color);
        }
    }
}
