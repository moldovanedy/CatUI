using System;
using SkiaSharp;

namespace CatUI.Data
{
    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public Color()
        {
            R = 0;
            G = 0;
            B = 0;
            A = 0;
        }

        public Color(uint value, bool isArgb = false)
        {
            if (isArgb)
            {
                B = (byte)(value & 0xff);
                G = (byte)((value >> 8) & 0xff);
                R = (byte)((value >> 16) & 0xff);
                A = (byte)((value >> 24) & 0xff);
            }
            else
            {
                A = (byte)(value & 0xff);
                B = (byte)((value >> 8) & 0xff);
                G = (byte)((value >> 16) & 0xff);
                R = (byte)((value >> 24) & 0xff);
            }
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(string hexString)
        {
            if (!hexString.StartsWith('#'))
            {
                A = 255;
                R = 0;
                G = 0;
                B = 0;
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
        }

        public static implicit operator SKColor(Color color) => new SKColor(color.R, color.G, color.B, color.A);
        public static implicit operator Color(SKColor color) => new Color(color.Red, color.Green, color.Blue, color.Alpha);
    }
}
