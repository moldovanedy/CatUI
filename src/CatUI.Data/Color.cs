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

        /// <summary>
        /// Represents a completely transparent color (0 in RGBA).
        /// </summary>
        public static Color Default => new(0, ColorType.RGBA);

        /// <summary>
        /// Creates a completely transparent color (0 in RGBA).
        /// </summary>
        public Color() { }

        /// <summary>
        /// Creates a color based on the given value and color type.
        /// </summary>
        /// <param name="value">
        /// The value for the color. You can use hexadecimal numbers to represent numbers in the used color format.
        /// </param>
        /// <param name="colorType">The color type to use. Describes how will the value be treated.</param>
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

            var skiaColor = new SKColor(R, G, B, A);
            skiaColor.ToHsv(out float h, out float s, out float v);
            Hue = h;
            Saturation = s;
            Value = v;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        /// <summary>
        /// Creates a color using the RGB format.
        /// </summary>
        /// <param name="red">A value between 0 and 255 representing red. It will be clamped if it goes beyond limits.</param>
        /// <param name="green">A value between 0 and 255 representing green. It will be clamped if it goes beyond limits.</param>
        /// <param name="blue">A value between 0 and 255 representing blue. It will be clamped if it goes beyond limits.</param>
        /// <param name="alpha">A value between 0 and 255 representing alpha. It will be clamped if it goes beyond limits.</param>
        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            R = Math.Clamp(red, (byte)0, (byte)255);
            G = Math.Clamp(green, (byte)0, (byte)255);
            B = Math.Clamp(blue, (byte)0, (byte)255);
            A = Math.Clamp(alpha, (byte)0, (byte)255);

            var skiaColor = new SKColor(R, G, B, A);
            skiaColor.ToHsv(out float h, out float s, out float v);
            Hue = h;
            Saturation = s;
            Value = v;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        /// <summary>
        /// Creates a color using the HSV format, where each parameter is a floating-point number. It will eventually
        /// use RGB, so precision loss is there, but generally unnoticeable. 
        /// </summary>
        /// <param name="hue">A value between 0 and 360. Will be clamped if it goes beyond limits.</param>
        /// <param name="saturation">A value between 0 and 100. Will be clamped if it goes beyond limits.</param>
        /// <param name="value">A value between 0 and 100. Will be clamped if it goes beyond limits.</param>
        /// <param name="alpha">
        /// A value between 0 and 1 (0 is completely transparent, 1 is completely opaque). Will be clamped if it
        /// goes beyond limits.
        /// </param>
        public Color(float hue, float saturation, float value, float alpha = 1)
        {
            hue = Math.Clamp(hue, 0, 360);
            saturation = Math.Clamp(saturation, 0, 100);
            value = Math.Clamp(value, 0, 100);
            alpha = Math.Clamp(alpha, 0, 1);

            Hue = hue;
            Saturation = saturation;
            Value = value;
            A = (byte)Math.Round(alpha * 255);

            SKColor skiaColor = SKColor.FromHsv(hue, saturation, value, A);
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

            var skiaColor = new SKColor(R, G, B, A);
            skiaColor.ToHsv(out float h, out float s, out float v);
            Hue = h;
            Saturation = s;
            Value = v;

            skiaColor.ToHsl(out _, out _, out float l);
            Lightness = l;
        }

        public static implicit operator SKColor(Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static implicit operator Color(SKColor color)
        {
            return new Color(color.Red, color.Green, color.Blue, color.Alpha);
        }

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

        /// <summary>
        /// Describes how will the bytes of a value (generally uint) be used to create the color.
        /// </summary>
        public enum ColorType
        {
            // ReSharper disable InconsistentNaming
            /// <summary>
            /// Will use the less significant 24 bits to create a solid (opaque) color. The format is big-endian,
            /// meaning that in 0x24_12_a2, 0x24 is red, 0x12 is green, 0xa2 is blue.
            /// </summary>
            RGB = 0,

            /// <summary>
            /// Will use all 32 bits to create a color that can have transparency. The format is big-endian,
            /// meaning that in 0x24_12_a2_40, 0x24 is red, 0x12 is green, 0xa2 is blue, 0x40 is alpha
            /// (so 75% transparent, as in 0x40 / 0xff).
            /// </summary>
            RGBA = 1,

            /// <summary>
            /// Will use all 32 bits to create a color that can have transparency. The format is big-endian,
            /// but alpha is the most significant, meaning that in 0x24_12_a2_40, 0x24 is alpha, 0x12 is red,
            /// 0xa2 is green, 0x40 is blue.
            /// </summary>
            ARGB = 2
            // ReSharper restore InconsistentNaming
        }
    }
}
