using SkiaSharp;

namespace CatUI.Data.Brushes
{
    public interface IBrush
    {
        public abstract SKPaint ToSkiaPaint();
    }
}
