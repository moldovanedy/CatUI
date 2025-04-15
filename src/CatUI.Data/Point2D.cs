using System;
using SkiaSharp;

namespace CatUI.Data
{
    public readonly struct Point2D
    {
        public float X { get; }
        public float Y { get; }

        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        public Point2D(float dimension)
        {
            X = dimension;
            Y = dimension;
        }

        public Point2D(string literal)
        {
            Point2D point = literal;
            X = point.X;
            Y = point.Y;
        }

        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Point2D Zero => new();

        public static implicit operator Point2D(string literal)
        {
            string[] substrings = literal.Split(' ');
            if (substrings.Length == 1)
            {
                return new Point2D(float.Parse(substrings[0]));
            }

            if (substrings.Length == 2)
            {
                return new Point2D(float.Parse(substrings[0]), float.Parse(substrings[1]));
            }

            throw new FormatException($"Couldn't parse the \"{literal}\" Point2D literal");
        }

        public static implicit operator SKPoint(Point2D point)
        {
            return new SKPoint(point.X, point.Y);
        }

        public static implicit operator Point2D(SKPoint skPoint)
        {
            return new Point2D(skPoint.X, skPoint.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
