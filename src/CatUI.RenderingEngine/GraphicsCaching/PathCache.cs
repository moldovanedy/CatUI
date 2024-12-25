using System.Collections.Generic;

using CatUI.Data;

using SkiaSharp;

namespace CatUI.RenderingEngine.GraphicsCaching
{
    public static class PathCache
    {
        /// <summary>
        /// Contains all the cached paths that are used in the application.
        /// Use the "TryGet" methods of this class to get a path to be used in Skia.
        /// </summary>
        /// <remarks>
        /// For the numeric key, the usage is as follows (from least significant bits to most significant bits):
        /// <list type="bullet">
        /// <item>Byte 0-1: the number of control points in the path (0-65535)</item>
        /// <item>Byte 2-4: the start X position of the path's bounds (as fixed point number (see below))</item>
        /// <item>Byte 5-7: the start Y position of the path's bounds (as fixed point number (see below))</item>
        /// <item>Byte 8-10: the width of the path's bounds (as fixed point number (see below))</item>
        /// <item>Byte 11-13: the height of the path's bounds (as fixed point number (see below))</item>
        /// <item>Byte 14-15: the number of verbs of this path (0-65535)</item>
        /// </list>
        /// 
        /// <para>
        /// A fixed point number in this case uses a byte for the fractional part (the one after the decimal, 0-99)
        /// and 2 bytes for the whole part (0-65535). Floating-point values will be rounded to 2 decimals in order to 
        /// use this scheme, so take this into account as it might cause bugs in rare cases (the caching system confusing 
        /// 2 different paths because their bounds are the same, although there are mechanisms to prevent this).
        /// </para>
        /// </remarks>
        private static readonly Dictionary<NumericKey, SKPath> _paths = new Dictionary<NumericKey, SKPath>();

        public static bool TryGetPath(
            ushort pointCount,
            Rect pathBounds,
            ushort verbCount,
            out SKPath? skiaPath)
        {
            ulong searchedKeyLow = pointCount;
            searchedKeyLow |= ((ulong)ConvertToFixedNumber(pathBounds.X)) << 16;
            searchedKeyLow |= ((ulong)ConvertToFixedNumber(pathBounds.Y)) << 40;

            ulong searchedKeyHigh = 0;
            searchedKeyHigh |= ConvertToFixedNumber(pathBounds.Width);
            searchedKeyHigh |= ((ulong)ConvertToFixedNumber(pathBounds.Height)) << 24;

            searchedKeyHigh |= ((ulong)verbCount) << 48;

            return _paths.TryGetValue(new NumericKey(searchedKeyLow, searchedKeyHigh), out skiaPath);
        }

        // <summary>
        /// Adds a new path to the internal cache.
        /// Usually used when TryGetValue doesn't find a matching path so you create a new one and cache it.
        /// </summary>
        /// <param name="path">The path to be cached.</param>
        /// <returns>
        /// True if the path was valid or the path was already cached, false if some parameters were invalid 
        /// (such as having more than 65535 points).
        /// </returns>
        public static bool CacheNewPath(SKPath path)
        {
            NumericKey newKey = GenerateKeyFromPath(path);
            if (newKey == new NumericKey(0L, 0L))
            {
                return false;
            }

            if (_paths.ContainsKey(newKey))
            {
                return true;
            }
            else
            {
                _paths[newKey] = path;
                return true;
            }
        }

        public static void RemovePath(SKPath skiaPath)
        {
            _paths.Remove(GenerateKeyFromPath(skiaPath));
        }

        public static void PurgeCache()
        {
            _paths.Clear();
        }

        private static NumericKey GenerateKeyFromPath(SKPath skiaPath)
        {
            if (skiaPath.PointCount > (1 << 16) || skiaPath.VerbCount > (1 << 16))
            {
                return NumericKey.Zero;
            }
            if (skiaPath.Bounds.Left > (1 << 16) ||
                skiaPath.Bounds.Top > (1 << 16) ||
                skiaPath.Bounds.Width > (1 << 16) ||
                skiaPath.Bounds.Height > (1 << 16))
            {
                return NumericKey.Zero;
            }

            ulong newKeyLow = (ulong)skiaPath.PointCount;
            Rect bounds = skiaPath.Bounds;

            newKeyLow |= ((ulong)ConvertToFixedNumber(bounds.X)) << 16;
            newKeyLow |= ((ulong)ConvertToFixedNumber(bounds.Y)) << 40;

            ulong newKeyHigh = 0;
            newKeyHigh |= ConvertToFixedNumber(bounds.Width);
            newKeyHigh |= ((ulong)ConvertToFixedNumber(bounds.Height)) << 24;
            newKeyHigh |= ((ulong)skiaPath.VerbCount) << 48;

            return new NumericKey(newKeyLow, newKeyHigh);
        }

        private static uint ConvertToFixedNumber(float number)
        {
            uint fixedNumber = (byte)(number % 1f * 100);

            uint wholePart = (uint)number;
            fixedNumber |= (uint)((byte)(wholePart % 256) << 8);
            fixedNumber |= (uint)((byte)(wholePart >> 8) << 16);

            return fixedNumber;
        }
    }
}