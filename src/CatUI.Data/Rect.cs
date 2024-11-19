using System;
using SkiaSharp;

namespace CatUI.Data
{
    public class Rect
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

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, W:{Width}, H:{Height}}}";
        }

        public float CenterX
        {
            get => X + (Width / 2);
            set => X += value - CenterX;
        }
        public float CenterY
        {
            get => Y + (Height / 2);
            set => Y += value - CenterY;
        }

        public float EndX
        {
            get => X + Width;
            set
            {
                if (value < X)
                {
                    throw new ArgumentException("Cannot have end point X smaller than start point X", nameof(EndX));
                }
                Width = value - X;
            }
        }

        public float EndY
        {
            get => Y + Height;
            set
            {
                if (value < Y)
                {
                    throw new ArgumentException("Cannot have end point Y smaller than start point Y", nameof(EndY));
                }
                Height = value - Y;
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

        /// <summary>
        /// Will return the smallest rect containing all of the given rects. It is NOT an union because it will return areas 
        /// that are not part of any of the given rects.
        /// </summary>
        /// <param name="rects">The rects for which to get the containing rect.</param>
        /// <returns>The smallest rect that contains all of the given rects.</returns>
        public static Rect GetCommonBoundingRect(params Rect[] rects)
        {
            if (rects.Length == 0)
            {
                return new Rect();
            }

            Rect boundingRect = rects[0];
            for (int i = 1; i < rects.Length; i++)
            {
                if (boundingRect.X > rects[i].X)
                {
                    boundingRect.X = rects[i].X;
                }

                if (boundingRect.Y > rects[i].Y)
                {
                    boundingRect.Y = rects[i].Y;
                }

                if (boundingRect.EndX < rects[i].EndX)
                {
                    boundingRect.EndX = rects[i].EndX;
                }

                if (boundingRect.EndY < rects[i].EndY)
                {
                    boundingRect.EndY = rects[i].EndY;
                }
            }

            return boundingRect;
        }
    }
}
