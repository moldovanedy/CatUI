using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Windows.PInvoke
{
    internal static partial class Gdi32
    {
        private const string gdi32 = "gdi32.dll";

        public const int DIB_RGB_COLORS = 0;
        public const uint SRCCOPY = 0x00CC0020;
        public const uint BI_BITFIELDS = 3;

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public short bmPlanes;
            public short bmBitsPixel;
            public IntPtr bmBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public uint redMask;
            public uint greenMask;
            public uint blueMask;
            public uint alphaMask;
        }


        [LibraryImport(gdi32)]
        internal static partial IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [LibraryImport(gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeleteObject(IntPtr hObject);

        [LibraryImport(gdi32, EntryPoint = "GetObjectW")]
        internal static partial int GetObject(IntPtr hObject, int nSize, out BITMAP bm);

        [LibraryImport(gdi32)]
        internal static partial IntPtr CreateCompatibleDC(IntPtr hdc);

        [LibraryImport(gdi32)]
        internal static partial IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFOHEADER bmi, uint usage,
            out IntPtr bits,
            IntPtr hSection, uint offset);

        [LibraryImport(gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeleteDC(IntPtr hdc);

        [LibraryImport(gdi32, SetLastError = true)]
        internal static partial int StretchDIBits(
            IntPtr hdc,
            int xDest, int yDest,
            int DestWidth, int DestHeight,
            int xSrc, int ySrc,
            int SrcWidth, int SrcHeight,
            IntPtr lpBits,
            ref BITMAPINFO lpBitsInfo,
            uint iUsage,
            uint rop);
    }
}
