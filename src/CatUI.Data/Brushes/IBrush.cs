using SkiaSharp;

namespace CatUI.Data.Brushes
{
    public interface IBrush
    {
        public SKPaint ToSkiaPaint();
        /// <summary>
        /// Returns true if the element to be drawn can skip the rendering because it would not create any difference.
        /// For example, a color brush that has a completely transparent color can instruct the element to skip rendering.
        /// </summary>
        public bool IsSkippable { get; }
    }
}
