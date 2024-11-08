using System;
using System.Diagnostics.CodeAnalysis;
using CatUI.Data.Enums;

namespace CatUI.Data
{
    /// <summary>
    /// Represents any dimension by having a value and a measurement unit. It's used for widths, heights and more.
    /// </summary>
    public struct Dimension
    {
        /// <summary>
        /// Represents the actual value. NaN represents the "unset dimension".
        /// </summary>
        public float Value { get; set; } = float.NaN;
        public Unit MeasuringUnit { get; set; } = Unit.Dp;

        public static Dimension Unset
        {
            get
            {
                return new Dimension()
                {
                    Value = float.NaN,
                    MeasuringUnit = Unit.Dp
                };
            }
        }

        public Dimension()
        {
            Value = float.NaN;
        }

        public Dimension(string literal)
        {
            Dimension dim = literal;
            Value = dim.Value;
            MeasuringUnit = dim.MeasuringUnit;
        }

        public Dimension(float value, Unit measuringUnit = Unit.Dp)
        {
            Value = value;
            MeasuringUnit = measuringUnit;
        }

        public override readonly string ToString()
        {
            string measuringUnitText;
            switch (MeasuringUnit)
            {
                case Unit.Dp:
                    measuringUnitText = "dp";
                    break;
                case Unit.Pixels:
                    measuringUnitText = "px";
                    break;
                case Unit.Percent:
                    measuringUnitText = "%";
                    break;
                case Unit.ViewportWidth:
                    measuringUnitText = "vw";
                    break;
                case Unit.ViewportHeight:
                    measuringUnitText = "vh";
                    break;
                default:
                    measuringUnitText = "?";
                    break;
            }

            return $"{Value} {measuringUnitText}";
        }

        public readonly bool IsUnset()
        {
            return float.IsNaN(Value);
        }

        public static implicit operator Dimension(float value) => new Dimension(value);
        public static implicit operator Dimension(int value) => new Dimension(value);

        public static implicit operator Dimension(string literal)
        {
            Unit unit;

            int unitStartPos;
            char lastChar = literal[literal.Length - 1];
            if (lastChar == '%')
            {
                unit = Unit.Percent;
                unitStartPos = literal.Length - 1;
            }
            else if (char.IsAsciiDigit(lastChar))
            {
                unit = Unit.Dp;
                unitStartPos = literal.Length - 1;
            }
            else
            {
                unitStartPos = literal.Length - 2;
                string unitLiteral = literal.Substring(literal.Length - 2);
                switch (unitLiteral)
                {
                    case "px":
                        unit = Unit.Pixels;
                        break;
                    case "dp":
                        unit = Unit.Dp;
                        break;
                    case "vw":
                        unit = Unit.ViewportWidth;
                        break;
                    case "vh":
                        unit = Unit.ViewportHeight;
                        break;
                    default:
                        unitStartPos = literal.Length;
                        unit = Unit.Dp;
                        break;
                }
            }

            float value = float.Parse(literal.AsSpan(0, unitStartPos));
            return new Dimension(value, unit);
        }

        public static bool operator !=(Dimension x, Dimension y)
        {
            if (x.Value != y.Value || x.MeasuringUnit != y.MeasuringUnit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(Dimension x, Dimension y)
        {
            if (x.Value == y.Value && x.MeasuringUnit == y.MeasuringUnit)
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
            return this == (Dimension)obj;
        }

        public override readonly int GetHashCode()
        {
            return System.HashCode.Combine(this.Value, this.MeasuringUnit);
        }
    }

    /// <summary>
    /// A set of 2 dimensions for X and Y. Generally used for setting the position of an element.
    /// </summary>
    public struct Dimension2
    {
        public Dimension X { get; set; } = new Dimension();
        public Dimension Y { get; set; } = new Dimension();

        public static Dimension2 Unset
        {
            get
            {
                return new Dimension2(Dimension.Unset, Dimension.Unset);
            }
        }

        public Dimension2()
        {
            this.X = Dimension.Unset;
            this.Y = Dimension.Unset;
        }
        public Dimension2(Dimension x, Dimension y)
        {
            this.X = x;
            this.Y = y;
        }
        public Dimension2(float x, float y)
        {
            X = new Dimension(x);
            Y = new Dimension(y);
        }

        public override readonly string ToString()
        {
            return $"({X}, {Y})";
        }

        public readonly bool IsUnset()
        {
            return X.IsUnset() && Y.IsUnset();
        }

        public static bool operator !=(Dimension2 x, Dimension2 y)
        {
            return x.X != y.X || x.Y != y.Y;
        }

        public static bool operator ==(Dimension2 x, Dimension2 y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public static implicit operator Dimension2(string literal)
        {
            string[] substrings = literal.Split(' ');
            if (substrings.Length == 1)
            {
                return new Dimension2(substrings[0], substrings[0]);
            }
            else if (substrings.Length == 2)
            {
                return new Dimension2(substrings[0], substrings[1]);
            }
            else
            {
                throw new FormatException($"Couldn't parse the \"{literal}\" Dimension2 literal");
            }
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            return this == (Dimension2)obj;
        }

        public override readonly int GetHashCode()
        {
            return System.HashCode.Combine(this.X, this.Y);
        }
    }
}
