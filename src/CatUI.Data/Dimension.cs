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
        public Dimension(float value, Unit measuringUnit = Unit.Dp)
        {
            Value = value;
            MeasuringUnit = measuringUnit;
        }

        public readonly bool IsUnset()
        {
            return float.IsNaN(Value);
        }

        public static implicit operator Dimension(float value) => new Dimension(value);
        public static implicit operator Dimension(int value) => new Dimension(value);

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
