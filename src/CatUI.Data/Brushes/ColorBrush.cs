using System.ComponentModel;
using System.Runtime.CompilerServices;
using CatUI.Data.Managers;
using SkiaSharp;

namespace CatUI.Data.Brushes
{
    public class ColorBrush : IBrush
    {
        public bool IsSkippable => Color.A == 0;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                NotifyPropertyChanged();
            }
        }

        private Color _color = Color.Default;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Creates a brush with a completely transparent color.
        /// </summary>
        public ColorBrush() { }

        public ColorBrush(Color color)
        {
            Color = color;
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public IBrush Duplicate()
        {
            return new ColorBrush(Color);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SKPaint ToSkiaPaint()
        {
            return PaintManager.GetPaint(paintColor: Color);
        }
    }
}
