using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace CatUI.Data
{
    public readonly struct Rect
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        public static Rect Empty { get; } = new(0, 0, 0, 0);

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
            return new SKRect { Left = rect.X, Top = rect.Y, Size = new SKSize(rect.Width, rect.Height) };
        }

        public static implicit operator Rect(SKRect skRect)
        {
            return new Rect(
                skRect.Left,
                skRect.Right,
                skRect.Size.Width,
                skRect.Size.Height);
        }

        public static Rect GetCommonBoundingRect(params Rect[] rects)
        {
            return GetCommonBoundingRect(rects.AsEnumerable());
        }

        /// <summary>
        /// Will return the smallest rect containing all the given rects. It is NOT a union because it will return areas 
        /// that are not part of the given rects.
        /// </summary>
        /// <param name="rects">The rects for which to get the containing rect.</param>
        /// <returns>The smallest rect that contains all the given rects.</returns>
        public static Rect GetCommonBoundingRect(IEnumerable<Rect> rects)
        {
            IEnumerator<Rect> enumerator = rects.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return Empty;
            }

            float x = enumerator.Current.X,
                  y = enumerator.Current.Y,
                  endX = enumerator.Current.EndX,
                  endY = enumerator.Current.EndY;

            while (enumerator.MoveNext())
            {
                if (x > enumerator.Current.X)
                {
                    x = enumerator.Current.X;
                }

                if (y > enumerator.Current.Y)
                {
                    y = enumerator.Current.Y;
                }

                if (endX < enumerator.Current.EndX)
                {
                    endX = enumerator.Current.EndX;
                }

                if (endY < enumerator.Current.EndY)
                {
                    endY = enumerator.Current.EndY;
                }
            }

            enumerator.Dispose();
            return new Rect(x, y, endX - x, endY - y);
        }

        public static Rect GetIntersectingRect(ref Rect rect1, ref Rect rect2)
        {
            if (DoRectsIntersect(ref rect1, ref rect2))
            {
                float x = Math.Max(rect1.X, rect2.X);
                float y = Math.Max(rect1.Y, rect2.Y);
                float width = Math.Min(rect1.EndX, rect2.EndX) - x;
                float height = Math.Min(rect1.EndY, rect2.EndY) - y;

                return new Rect(x, y, width, height);
            }

            return Empty;
        }

        public static bool DoRectsIntersect(ref Rect rect1, ref Rect rect2)
        {
            if (rect2.X >= rect1.EndX)
            {
                return false;
            }

            if (rect2.EndX <= rect1.X)
            {
                return false;
            }

            if (rect2.Y >= rect1.EndY)
            {
                return false;
            }

            if (rect2.EndY <= rect1.Y)
            {
                return false;
            }

            return true;
        }
    }
}
