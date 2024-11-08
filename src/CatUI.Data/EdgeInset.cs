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
    public struct EdgeInset
    {
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

        public Dimension Top { get; set; } = new Dimension();
        public Dimension Right { get; set; } = new Dimension();
        public Dimension Bottom { get; set; } = new Dimension();
        public Dimension Left { get; set; } = new Dimension();

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

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
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
            if (substrings.Length == 1)
            {
                return new EdgeInset(substrings[0]);
            }
            else if (substrings.Length == 2)
            {
                return new EdgeInset(substrings[0], substrings[1]);
            }
            else if (substrings.Length == 4)
            {
                return new EdgeInset(substrings[0], substrings[1], substrings[2], substrings[3]);
            }
            else
            {
                throw new FormatException($"Couldn't parse the \"{literal}\" EdgeInset literal");
            }
        }

        public override readonly int GetHashCode()
        {
            return System.HashCode.Combine(this.Top, this.Right, this.Bottom, this.Left);
        }

        public override readonly string ToString()
        {
            return $"({Top}, {Right}, {Bottom}, {Left})";
        }
    }
}
