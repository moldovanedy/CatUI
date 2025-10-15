using System;
using System.Diagnostics.CodeAnalysis;
using SkiaSharp;

namespace CatUI.Data;

public readonly struct Color : ICloneable, IEquatable<Color>
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
    /// <param name="isHsv">
    /// Ignored, it's just to avoid confusion with the <see cref="Color(byte, byte, byte, byte)"/> overload.
    /// </param>
    public Color(float hue, float saturation, float value, float alpha, bool isHsv)
    {
        //silence the compiler
        _ = isHsv;

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

    #region Implicit and explicit conversions

    /// <summary>
    /// Returns an equivalent <see cref="SKColor"/>.
    /// </summary>
    public static implicit operator SKColor(Color color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Creates a new color from an <see cref="SKColor"/>.
    /// </summary>
    public static implicit operator Color(SKColor color)
    {
        return new Color(color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Creates a new color from a hex string literal
    /// </summary>
    public static implicit operator Color(string literal)
    {
        return new Color(literal);
    }

    /// <summary>
    /// Creates a new color from an ARGB 32-bit uint.
    /// </summary>
    public static implicit operator Color(uint argb)
    {
        return new Color(argb, ColorType.ARGB);
    }

    /// <summary>
    /// Allows casting a color to an RGBA 32-bit uint.
    /// </summary>
    public static explicit operator uint(Color color)
    {
        return
            ((uint)color.R << 24) |
            ((uint)color.G << 16) |
            ((uint)color.B << 8) |
            color.A;
    }

    #endregion

    #region Arithmetic and relational operators

    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Color left, Color right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Returns a new color where each of the R, G, B, and A components is the sum of the corresponding components
    /// from the two given colors. If a value exceeds <see cref="byte.MaxValue"/>, it will be clamped to it.
    /// </summary>
    /// <returns>A new color with its components being the sum of the other two colors.</returns>
    public static Color operator +(Color left, Color right)
    {
        return new Color(
            (byte)Math.Min(left.R + right.R, byte.MaxValue),
            (byte)Math.Min(left.G + right.G, byte.MaxValue),
            (byte)Math.Min(left.B + right.B, byte.MaxValue),
            (byte)Math.Min(left.A + right.A, byte.MaxValue));
    }

    /// <summary>
    /// Returns a new color where each of the R, G, B, and A components is the difference of the corresponding
    /// components from the two given colors. If a value goes below <see cref="byte.MinValue"/>, it will be clamped to it.
    /// </summary>
    /// <returns>A new color with its components being the difference of the first color and the second one.</returns>
    public static Color operator -(Color left, Color right)
    {
        return new Color(
            (byte)Math.Max(left.R - right.R, byte.MinValue),
            (byte)Math.Max(left.G - right.G, byte.MinValue),
            (byte)Math.Max(left.B - right.B, byte.MinValue),
            (byte)Math.Max(left.A - right.A, byte.MinValue));
    }

    #endregion

    public override string ToString()
    {
        return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    }

    public bool Equals(Color other)
    {
        return A == other.A && R == other.R && G == other.G && B == other.B;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Color color && Equals(color);
    }

    public override int GetHashCode()
    {
        return ((uint)this).GetHashCode();
    }

    public object Clone()
    {
        return new Color(R, G, B, A);
    }

    /// <summary>
    /// Returns a weighted average of red, green, and blue components based on how the human eye perceives
    /// brightness. Does not take <see cref="A"/> into account!
    /// </summary>
    /// <returns>The color luminance.</returns>
    public double CalculateLuminance()
    {
        //adapted from https://stackoverflow.com/a/9733420/23361865
        const double RED = 0.2126;
        const double GREEN = 0.7152;
        const double BLUE = 0.0722;
        const double GAMMA = 2.4;

        double[] components = [R, G, B];
        for (int i = 0; i < components.Length; i++)
        {
            components[i] /= 255.0;
            if (components[i] <= 0.03928)
            {
                components[i] /= 12.92;
            }
            else
            {
                components[i] = Math.Pow((components[i] + 0.055) / 1.055, GAMMA);
            }
        }

        return (components[0] * RED) + (components[1] * GREEN) + (components[2] * BLUE);
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
    /// Calculates the contrast ratio between two colors. Ranges between 1 and 21. Does not take <see cref="A"/>
    /// into account!
    /// </summary>
    /// <param name="col1"></param>
    /// <param name="col2"></param>
    /// <returns>The contrast ratio.</returns>
    public static double CalculateContrastRatio(Color col1, Color col2)
    {
        //adapted from https://stackoverflow.com/a/9733420/23361865
        double lum1 = col1.CalculateLuminance();
        double lum2 = col2.CalculateLuminance();
        double brightest = Math.Max(lum1, lum2);
        double darkest = Math.Min(lum1, lum2);
        return (brightest + 0.05) / (darkest + 0.05);
    }

    /// <summary>
    /// Performs a linear interpolation between two colors by using <see cref="double.Lerp"/> on each component
    /// (R, G, B, and A) individually.
    /// </summary>
    /// <remarks>
    /// The weight can also go outside the range [0,1]. If a value exceeds the RGBA limits (i.e. lower than 0 or
    /// higher than 255), <see cref="Math.Clamp(double, double, double)"/> will be used to keep colors in the range.
    /// </remarks>
    /// <param name="from">The start color ("lower bound").</param>
    /// <param name="to">The end color ("upper bound").</param>
    /// <param name="t">The interpolation "weight", generally between 0 and 1.</param>
    /// <returns>The value of the linear interpolation as a color.</returns>
    public static Color Lerp(Color from, Color to, float t)
    {
        return new Color(
            (byte)Math.Clamp(double.Lerp(from.R, to.R, t), 0, 255),
            (byte)Math.Clamp(double.Lerp(from.G, to.G, t), 0, 255),
            (byte)Math.Clamp(double.Lerp(from.B, to.B, t), 0, 255),
            (byte)Math.Clamp(double.Lerp(from.A, to.A, t), 0, 255)
        );
    }

    /// <summary>
    /// Describes how the bytes of a value (generally uint) will be used to create the color.
    /// </summary>
    public enum ColorType
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Will use the less significant 24 bits to create a solid (opaque) color.
        /// </summary>
        /// <example>
        /// In 0x24_12_a2, 0x24 is red, 0x12 is green, 0xa2 is blue.
        /// </example>
        RGB = 0,

        /// <summary>
        /// Will use all 32 bits to create a color that can have transparency.
        /// </summary>
        /// <example>
        /// In 0x24_12_a2_40, 0x24 is red, 0x12 is green, 0xa2 is blue, 0x40 is alpha (so 75% transparent,
        /// as in 0x40 / 0xff).
        /// </example>
        RGBA = 1,

        /// <summary>
        /// Will use all 32 bits to create a color that can have transparency.
        /// </summary>
        /// <example>
        /// In 0x24_12_a2_40, 0x24 is alpha, 0x12 is red, 0xa2 is green, 0x40 is blue.
        /// </example>
        ARGB = 2
        // ReSharper restore InconsistentNaming
    }
}
