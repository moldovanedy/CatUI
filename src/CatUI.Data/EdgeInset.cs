using System;
using System.Diagnostics.CodeAnalysis;

namespace CatUI.Data
{
    /// <summary>
    /// A set of offsets in each of the 4 cardinal directions: top, left, bottom, right.
    /// </summary>
    /// <remarks>
    /// The inset represents different dimensions depending on its use:
    /// for margins, it represents the distance from the current element to the next one in one of the four directions (top, left, bottom, right);
    /// for position, it represents the distance from the current element to one or more of the containing element's border.
    /// </remarks>
    public class EdgeInset : CatObject
    {
        public Dimension Top { get; set; } = new();
        public Dimension Right { get; set; } = new();
        public Dimension Bottom { get; set; } = new();
        public Dimension Left { get; set; } = new();

        public EdgeInset() { }

        public EdgeInset(Dimension dimension)
        {
            Top = dimension;
            Right = dimension;
            Bottom = dimension;
            Left = dimension;
        }

        public EdgeInset(string literal)
        {
            EdgeInset inset = literal;
            Top = inset.Top;
            Right = inset.Right;
            Bottom = inset.Bottom;
            Left = inset.Left;
        }

        public EdgeInset(Dimension topBottom, Dimension leftRight)
        {
            Top = topBottom;
            Bottom = topBottom;
            Left = leftRight;
            Right = leftRight;
        }

        public EdgeInset(Dimension top, Dimension right, Dimension bottom, Dimension left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public static bool operator !=(EdgeInset x, EdgeInset y)
        {
            if (x.Top != y.Top || x.Right != y.Right || x.Bottom != y.Bottom || x.Left != y.Left)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(EdgeInset x, EdgeInset y)
        {
            if (x.Top == y.Top && x.Right == y.Right && x.Bottom == y.Bottom && x.Left == y.Left)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this == (EdgeInset)obj;
        }

        public static implicit operator EdgeInset(string literal)
        {
            string[] substrings = literal.Split(' ');
            return substrings.Length switch
            {
                1 => new EdgeInset(substrings[0]),
                2 => new EdgeInset(substrings[0], substrings[1]),
                4 => new EdgeInset(substrings[0], substrings[1], substrings[2], substrings[3]),
                _ => throw new FormatException($"Couldn't parse the \"{literal}\" EdgeInset literal")
            };
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException("Using EdgeInset as a key in a dictionary/hash map is not supported.");
        }

        public override string ToString()
        {
            return $"({Top}, {Right}, {Bottom}, {Left})";
        }

        public override EdgeInset Duplicate()
        {
            return new EdgeInset(Top, Right, Bottom, Left);
        }
    }
}
