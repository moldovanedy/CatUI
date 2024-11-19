using System;

namespace CatUI.Data
{
    public class Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size()
        {
            Width = 0;
            Height = 0;
        }

        public Size(string literal)
        {
            Size size = literal;
            Width = size.Width;
            Height = size.Height;
        }

        public Size(float dimension)
        {
            Width = dimension;
            Height = dimension;
        }

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator Size(string literal)
        {
            string[] substrings = literal.Split(' ');
            if (substrings.Length == 1)
            {
                return new Size(float.Parse(substrings[0]));
            }
            else if (substrings.Length == 2)
            {
                return new Size(float.Parse(substrings[0]), float.Parse(substrings[1]));
            }
            else
            {
                throw new FormatException($"Couldn't parse the \"{literal}\" Size literal");
            }
        }

        public override string ToString()
        {
            return $"{{W:{Width}, H:{Height}}}";
        }
    }
}
