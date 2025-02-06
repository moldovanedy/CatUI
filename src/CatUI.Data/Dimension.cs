using System;
using System.Diagnostics.CodeAnalysis;
using CatUI.Data.Enums;

namespace CatUI.Data
{
    /// <summary>
    /// Represents any dimension by having a value and a measurement unit. It's used for widths, heights and more.
    /// </summary>
    public readonly struct Dimension : IEquatable<Dimension>
    {
        /// <summary>
        /// Represents the actual value. NaN represents the "unset dimension".
        /// </summary>
        public float Value { get; }

        public Unit MeasuringUnit { get; } = Unit.Dp;

        public static Dimension Unset => new(float.NaN);

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

        public override string ToString()
        {
            string measuringUnitText = MeasuringUnit switch
            {
                Unit.Dp => "dp",
                Unit.Pixels => "px",
                Unit.Percent => "%",
                Unit.ViewportWidth => "vw",
                Unit.ViewportHeight => "vh",
                _ => "?"
            };

            return $"{Value} {measuringUnitText}";
        }

        public Dimension Duplicate()
        {
            return new Dimension(Value, MeasuringUnit);
        }

        public bool IsUnset()
        {
            return float.IsNaN(Value);
        }

        public static implicit operator Dimension(float value)
        {
            return new Dimension(value);
        }

        public static implicit operator Dimension(int value)
        {
            return new Dimension(value);
        }

        public static implicit operator Dimension(string literal)
        {
            Unit unit;

            int unitStartPos;
            char lastChar = literal[^1];
            if (lastChar == '%')
            {
                unit = Unit.Percent;
                unitStartPos = literal.Length - 1;
            }
            else if (char.IsDigit(lastChar))
            {
                unit = Unit.Dp;
                unitStartPos = literal.Length;
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

        public static bool operator !=(Dimension? x, Dimension? y)
        {
            if (x is null)
            {
                return y is not null;
            }

            if (y is null)
            {
                return true;
            }

            //if one of them is NaN, but not both
            if (double.IsNaN(x.Value.Value) ^ double.IsNaN(y.Value.Value))
            {
                return true;
            }

            return Math.Abs(x.Value.Value - y.Value.Value) > 0.001 || x.Value.MeasuringUnit != y.Value.MeasuringUnit;
        }

        public static bool operator ==(Dimension? x, Dimension? y)
        {
            //this will silence the nullable warnings
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }

            //if one of them is NaN, but not both
            if (double.IsNaN(x.Value.Value) ^ double.IsNaN(y.Value.Value))
            {
                return false;
            }

            return Math.Abs(x.Value.Value - y.Value.Value) < 0.001 && x.Value.MeasuringUnit == y.Value.MeasuringUnit;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this == (Dimension)obj;
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException("Using Dimension as a key in a dictionary/hash map is not supported.");
        }

        public bool Equals(Dimension other)
        {
            return Value.Equals(other.Value) && MeasuringUnit == other.MeasuringUnit;
        }
    }

    /// <summary>
    /// A set of 2 dimensions for X and Y. Generally used for setting the position of an element.
    /// </summary>
    public readonly struct Dimension2 : IEquatable<Dimension2>
    {
        public Dimension X { get; }
        public Dimension Y { get; }

        public static Dimension2 Unset => new(Dimension.Unset, Dimension.Unset);

        public Dimension2()
        {
            X = Dimension.Unset;
            Y = Dimension.Unset;
        }

        public Dimension2(string literal)
        {
            Dimension2 dim = literal;
            X = dim.X;
            Y = dim.Y;
        }

        public Dimension2(Dimension x, Dimension y)
        {
            X = x;
            Y = y;
        }

        public Dimension2(float x, float y)
        {
            X = new Dimension(x);
            Y = new Dimension(y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public Dimension2 Duplicate()
        {
            return new Dimension2(X.Duplicate(), Y.Duplicate());
        }

        public bool IsUnset()
        {
            return X.IsUnset() && Y.IsUnset();
        }

        public static bool operator ==(Dimension2? x, Dimension2? y)
        {
            //this will silence the nullable warnings
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }

            return x.Value.X == y.Value.X && x.Value.Y == y.Value.Y;
        }

        public static bool operator !=(Dimension2? x, Dimension2? y)
        {
            //this will silence the nullable warnings
            if (x is null)
            {
                return y is not null;
            }

            if (y is null)
            {
                return true;
            }

            return x.Value.X != y.Value.X || x.Value.Y != y.Value.Y;
        }

        public static implicit operator Dimension2(string literal)
        {
            string[] substrings = literal.Split(' ');
            return substrings.Length switch
            {
                1 => new Dimension2(substrings[0], substrings[0]),
                2 => new Dimension2(substrings[0], substrings[1]),
                _ => throw new FormatException($"Couldn't parse the \"{literal}\" Dimension2 literal")
            };
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this == (Dimension2)obj;
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException("Using Dimension as a key in a dictionary/hash map is not supported.");
        }

        public bool Equals(Dimension2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}
