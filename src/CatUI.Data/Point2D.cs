using System;

namespace CatUI.Data
{
    public class Point2D
    {
        public float X { get; set; }
        public float Y { get; set; }

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

        public static Point2D Zero
        {
            get
            {
                return new Point2D();
            }
        }

        public static implicit operator Point2D(string literal)
        {
            string[] substrings = literal.Split(' ');
            if (substrings.Length == 1)
            {
                return new Point2D(float.Parse(substrings[0]));
            }
            else if (substrings.Length == 2)
            {
                return new Point2D(float.Parse(substrings[0]), float.Parse(substrings[1]));
            }
            else
            {
                throw new FormatException($"Couldn't parse the \"{literal}\" Point2D literal");
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
