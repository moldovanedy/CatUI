using SkiaSharp;

namespace CatUI.Data
{
    public readonly struct Rect
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

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

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, W:{Width}, H:{Height}}}";
        }

        public float CenterX => X + (Width / 2);
        public float CenterY => Y + (Height / 2);

        public float EndX => X + Width;

        public float EndY => Y + Height;

        public static implicit operator SKRect(Rect rect)
        {
            return new SKRect() { Left = rect.X, Top = rect.Y, Size = new SKSize(rect.Width, rect.Height) };
        }

        public static implicit operator Rect(SKRect skRect)
        {
            return new Rect(
                (int)skRect.Left,
                (int)skRect.Right,
                (int)skRect.Size.Width,
                (int)skRect.Size.Height);
        }

        /// <summary>
        /// Will return the smallest rect containing all the given rects. It is NOT a union because it will return areas 
        /// that are not part of the given rects.
        /// </summary>
        /// <param name="rects">The rects for which to get the containing rect.</param>
        /// <returns>The smallest rect that contains all the given rects.</returns>
        public static Rect GetCommonBoundingRect(params Rect[] rects)
        {
            if (rects.Length == 0)
            {
                return new Rect();
            }

            float x = rects[0].X,
                  y = rects[0].Y,
                  endX = rects[0].EndX,
                  endY = rects[0].EndY;
            for (int i = 1; i < rects.Length; i++)
            {
                if (x > rects[i].X)
                {
                    x = rects[i].X;
                }

                if (y > rects[i].Y)
                {
                    y = rects[i].Y;
                }

                if (endX < rects[i].EndX)
                {
                    endX = rects[i].EndX;
                }

                if (endY < rects[i].EndY)
                {
                    endY = rects[i].EndY;
                }
            }

            return new Rect(x, y, endX - x, endY - y);
        }
    }
}
