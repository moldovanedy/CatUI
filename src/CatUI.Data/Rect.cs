using SkiaSharp;

namespace CatUI.Data
{
    public struct Rect
    {

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rect()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override readonly string ToString()
        {
            return $"{{X: {X}, Y: {Y}, W:{Width}, H:{Height}}}";
        }

        public readonly float CenterX
        {
            get
            {
                return (Width - X) / 2;
            }
        }
        public readonly float CenterY
        {
            get
            {
                return (Height - Y) / 2;
            }
        }

        public static implicit operator SKRect(Rect rect) =>
            new SKRect()
            {
                Left = rect.X,
                Top = rect.Y,
                Size = new SKSize(rect.Width, rect.Height)
            };
        public static implicit operator Rect(SKRect skRect) =>
            new Rect(
                (int)skRect.Left,
                (int)skRect.Right,
                (int)skRect.Size.Width,
                (int)skRect.Size.Height);
    }
}
