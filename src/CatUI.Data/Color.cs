using System;

using SkiaSharp;

namespace CatUI.Data
{
    public readonly struct Color : ICloneable
    {
        public byte A { get; } = 0;

        public byte R { get; } = 0;
        public byte G { get; } = 0;
        public byte B { get; } = 0;

        public float Hue { get; } = 0;
        public float Saturation { get; } = 0;
        public float Value { get; } = 0;
        public float Lightness { get; } = 0;

        public static Color Default => new Color(0, ColorType.RGBA);

        public Color() { }

        public Color(uint value, ColorType colorType = ColorType.RGB)
        {
            switch (colorType)
            {
                case ColorType.RGB:
                    B = (byte)(value & 0xff);
                    G = (byte)((value >> 8) & 0xff);
                    R = (byte)((value >> 16) & 0xff);
                    A = 255;
                    break;
                default:
                case ColorType.RGBA:
                    A = (byte)(value & 0xff);
                    B = (byte)((value >> 8) & 0xff);
                    G = (byte)((value >> 16) & 0xff);
                    R = (byte)((value >> 24) & 0xff);
                    break;
                case ColorType.ARGB:
                    B = (byte)(value & 0xff);
                    G = (byte)((value >> 8) & 0xff);
                    R = (byte)((value >> 16) & 0xff);
                    A = (byte)((value >> 24) & 0xff);
                    break;
            }

            SKColor skiaColor = new SKColor(R, G, B, A);
            skiaColor.ToHsv(out float h, out float s, out float v);
            Hue = h;
            Saturation = s;
            Value = v;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;

            SKColor skiaColor = new SKColor(R, G, B, A);
            skiaColor.ToHsv(out float h, out float s, out float v);
            Hue = h;
            Saturation = s;
            Value = v;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        public Color(float hue, float saturation, float value, byte alpha = 255)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
            A = alpha;

            SKColor skiaColor = SKColor.FromHsv(hue, saturation, value, alpha);
            R = skiaColor.Red;
            G = skiaColor.Green;
            B = skiaColor.Blue;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        public Color(string hexString)
        {
            if (!hexString.StartsWith('#'))
            {
                return;
            }

            //#RGB
            if (hexString.Length == 4)
            {
                R = Convert.ToByte(new string(hexString[1], 2), 16);
                G = Convert.ToByte(new string(hexString[2], 2), 16);
                B = Convert.ToByte(new string(hexString[3], 2), 16);
                A = 255;
            }
            //#RGBA
            else if (hexString.Length == 5)
            {
                R = Convert.ToByte(new string(hexString[1], 2), 16);
                G = Convert.ToByte(new string(hexString[2], 2), 16);
                B = Convert.ToByte(new string(hexString[3], 2), 16);
                A = Convert.ToByte(new string(hexString[4], 2), 16);
            }
            //#RRGGBB
            else if (hexString.Length == 7)
            {
                R = Convert.ToByte(hexString.Substring(1, 2), 16);
                G = Convert.ToByte(hexString.Substring(3, 2), 16);
                B = Convert.ToByte(hexString.Substring(5, 2), 16);
                A = 255;
            }
            //#RRGGBBAA
            else if (hexString.Length == 9)
            {
                R = Convert.ToByte(hexString.Substring(1, 2), 16);
                G = Convert.ToByte(hexString.Substring(3, 2), 16);
                B = Convert.ToByte(hexString.Substring(5, 2), 16);
                A = Convert.ToByte(hexString.Substring(7, 2), 16);
            }
            else
            {
                throw new FormatException($"The string \"{hexString}\" is not a valid hexadecimal color string.");
            }

            SKColor skiaColor = new SKColor(R, G, B, A);
            skiaColor.ToHsv(out float h, out float s, out float v);
            Hue = h;
            Saturation = s;
            Value = v;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        public static implicit operator SKColor(Color color) => new SKColor(color.R, color.G, color.B, color.A);
        public static implicit operator Color(SKColor color) => new Color(color.Red, color.Green, color.Blue, color.Alpha);

        public static implicit operator Color(string literal)
        {
            return new Color(literal);
        }

        public override string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
        }

        public object Clone()
        {
            return new Color(R, G, B, A);
        }

        /// <summary>
        /// Returns a copy of this color but with the specified alpha channel value.
        /// </summary>
        /// <param name="a">The alpha value, between 0 and 255.</param>
        /// <returns>A copy of this color with a modified alpha value.</returns>
        public Color WithAlpha(byte a)
        {
            return new Color(R, G, B, a);
        }

        /// <summary>
        /// Returns a copy of this color but with the specified red channel value.
        /// </summary>
        /// <param name="r">The red value, between 0 and 255.</param>
        /// <returns>A copy of this color with a modified red value.</returns>
        public Color WithRed(byte r)
        {
            return new Color(r, G, B, A);
        }

        /// <summary>
        /// Returns a copy of this color but with the specified green channel value.
        /// </summary>
        /// <param name="g">The green value, between 0 and 255.</param>
        /// <returns>A copy of this color with a modified green value.</returns>
        public Color WithGreen(byte g)
        {
            return new Color(R, g, B, A);
        }

        /// <summary>
        /// Returns a copy of this color but with the specified blue channel value.
        /// </summary>
        /// <param name="b">The blue value, between 0 and 255.</param>
        /// <returns>A copy of this color with a modified blue value.</returns>
        public Color WithBlue(byte b)
        {
            return new Color(R, G, b, A);
        }

        public enum ColorType
        {
            // ReSharper disable InconsistentNaming
            RGB = 0,
            RGBA = 1,
            ARGB = 2
            // ReSharper restore InconsistentNaming
        }
    }
}
