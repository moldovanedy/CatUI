using System.Collections.Generic;
using CatUI.Data.Enums;
using SkiaSharp;

namespace CatUI.RenderingEngine.GraphicsCaching
{
    public static class PaintDatabase
    {
        public static SKPaint DefaultPainter { get; } = new SKPaint()
        {
            Color = new SKColor(0xff_00_00_00),
            TextEncoding = SKTextEncoding.Utf8,
            TextSize = 16,
            IsAntialias = true,
        };

        /// <summary>
        /// Contains all the cached text paints that are used in the application.
        /// Use the "TryGet" methods of this class to get a paint to be used in Skia.
        /// </summary>
        /// <remarks>
        /// For the numeric key, the usage is as follows (from most significant bits to least significant bits):
        /// <list type="bullet">
        /// <item>Byte 0: fractional part of the font size (0-99)</item>
        /// <item>Byte 1: whole part of the font size (0-255)</item>
        /// <item>Bytes 2-5: fill (paint) color (RGBA as big-endian, so R is 5th byte, G is 4th etc.)</item>
        /// <item>Byte 6: bit 0 is 0 for fill, 1 for stroke,
        /// bits 1-2 are 00 for align left, 01 for align center and 10 for align right</item>
        /// <item>Byte 7: the font used (max. 256)</item>
        /// </list>
        /// </remarks>
        private static readonly Dictionary<ulong, SKPaint> _paints = new Dictionary<ulong, SKPaint>();

        /// <summary>
        /// Searches the internal cache for a font paint to be used by Skia.
        /// </summary>
        /// <param name="textSize">The text size. Must be 256 units. Only the first 2 decimals are stored and searched!</param>
        /// <param name="color">The color used.</param>
        /// <param name="textPaintMode">Whether the paint is used for filling (default) or stroking (outline only). Those can't be combined here.</param>
        /// <param name="alignmentType">The alignment type used by the paint. Doesn't support <see cref="HorizontalAlignmentType.Stretch"/>.</param>
        /// <param name="paint">The paint that will be returned with out if it exists. Null otherwise.</param>
        /// <returns>True if a paint was found, false otherwise.</returns>
        public static bool TryGetFontPaint(
            float textSize,
            SKColor color,
            PaintMode textPaintMode,
            HorizontalAlignmentType alignmentType,
            out SKPaint? paint)
        {
            paint = null;

            //will only get the fractional part, multiply it by 100 (2 decimals) and then use it as the final byte
            //(converting to byte will leave only 2 decimals as a byte, as it's always between 0-99)
            ulong searchedKey = (byte)(textSize % 1f * 100);

            //the whole part must be less than 256, converting it to a ulong will remove the fractional part,
            //this will be the second byte
            if (textSize >= 256f)
            {
                return false;
            }
            searchedKey |= ((ulong)textSize) << 8;

            //the color
            searchedKey |= (ulong)color.Red << 16;
            searchedKey |= (ulong)color.Green << 24;
            searchedKey |= (ulong)color.Blue << 32;
            searchedKey |= (ulong)color.Alpha << 40;

            //the paint mode
            if (textPaintMode == PaintMode.FillAndStroke)
            {
                textPaintMode = PaintMode.Fill;
            }
            searchedKey |= ((ulong)textPaintMode & 0b1) << 48;

            //the alignment
            if (alignmentType == HorizontalAlignmentType.Stretch)
            {
                alignmentType = HorizontalAlignmentType.Left;
            }
            searchedKey |= ((ulong)(alignmentType - 1) & 0b11) << 49;

            return _paints.TryGetValue(searchedKey, out paint);
        }


        public static bool TryGetPaint(SKColor color, out SKPaint? paint)
        {
            ulong searchedKey = 0;
            searchedKey |= (ulong)color.Red << 16;
            searchedKey |= (ulong)color.Green << 24;
            searchedKey |= (ulong)color.Blue << 32;
            searchedKey |= (ulong)color.Alpha << 40;
            return _paints.TryGetValue(searchedKey, out paint);
        }

        /// <summary>
        /// Adds a new paint to the internal cache.
        /// Usually used when TryGetValue doesn't find a matching paint so you create a new one and cache it.
        /// </summary>
        /// <remarks>
        /// Even if the paint is used for shapes instead of text,
        /// the <see cref="SKPaint.TextSize"/> must be less than 256.</remarks>
        /// <param name="paint">The paint to be cached.</param>
        /// <returns>True if the paint was valid or the paint was already cached, false if some parameters were invalid (such as the TextSize).</returns>
        public static bool CacheNewPaint(SKPaint paint)
        {
            ulong newKey = GenerateKeyFromPaint(paint);
            if (newKey == 0)
            {
                return false;
            }

            if (_paints.ContainsKey(newKey))
            {
                return true;
            }
            else
            {
                _paints[newKey] = paint;
                return true;
            }
        }

        public static void RemovePaint(SKPaint paint)
        {
            ulong newKey = GenerateKeyFromPaint(paint);
            _paints.Remove(newKey);
        }

        public static void PurgeCache()
        {
            _paints.Clear();
        }

        private static ulong GenerateKeyFromPaint(SKPaint paint)
        {
            //will only get the fractional part, multiply it by 100 (2 decimals) and then use it as the final byte
            //(converting to byte will leave only 2 decimals as a byte, as it's always between 0-99)
            ulong newKey = (byte)(paint.TextSize % 1f * 100);

            //the whole part must be less than 256, converting it to a ulong will remove the fractional part,
            //this will be the second byte
            if (paint.TextSize >= 256f)
            {
                return 0;
            }
            newKey |= ((ulong)paint.TextSize) << 8;

            //the color
            newKey |= (ulong)paint.Color.Red << 16;
            newKey |= (ulong)paint.Color.Green << 24;
            newKey |= (ulong)paint.Color.Blue << 32;
            newKey |= (ulong)paint.Color.Alpha << 40;

            //the paint mode
            PaintMode textPaintMode = paint.IsStroke ? PaintMode.Stroke : PaintMode.Fill;
            newKey |= (ulong)textPaintMode << 48;

            //the alignment
            newKey |= ((ulong)paint.TextAlign & 0b11) << 49;

            return newKey;
        }
    }
}
