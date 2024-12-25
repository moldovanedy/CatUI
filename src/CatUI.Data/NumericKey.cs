using System;

namespace CatUI.Data
{
    /// <summary>
    /// A struct similar to UInt128 (which is not available in .net6 and prior)
    /// that is used as a key in the internal cache dictionaries.
    /// </summary>
    public readonly struct NumericKey : IEquatable<NumericKey>
    {
        public static NumericKey Zero => new NumericKey();

        private readonly ulong _low;
        private readonly ulong _high;

        public NumericKey()
        {
            _low = 0;
            _high = 0;
        }
        public NumericKey(ulong low, ulong high)
        {
            _low = low;
            _high = high;
        }

        public override readonly int GetHashCode()
        {
            return _low.GetHashCode() ^ _high.GetHashCode();
        }

        public override readonly bool Equals(object? other)
        {
            if (other is NumericKey key)
            {
                return _low == key._low && _high == key._high;
            }
            return false;
        }

        public static bool operator ==(NumericKey left, NumericKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NumericKey left, NumericKey right)
        {
            return !(left == right);
        }

        public bool Equals(NumericKey other)
        {
            return _low == other._low && _high == other._high;
        }
    }
}
