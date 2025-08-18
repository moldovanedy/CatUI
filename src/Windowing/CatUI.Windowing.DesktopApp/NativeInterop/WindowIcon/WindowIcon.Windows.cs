using System;
using System.Runtime.InteropServices;
using SkiaSharp;

// ReSharper disable InconsistentNaming

namespace CatUI.Windowing.DesktopApp.NativeInterop.WindowIcon
{
    public static partial class WindowIconWindows
    {
        private const string user32 = "user32.dll";
        private const string gdi32 = "gdi32.dll";

        [LibraryImport(user32, SetLastError = true)]
        private static partial IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref ICONINFO iconInfo);

        [LibraryImport(gdi32)]
        private static partial IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [LibraryImport(gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DeleteObject(IntPtr hObject);

        [LibraryImport(gdi32)]
        private static partial int GetObject(IntPtr hObject, int nSize, ref BITMAP bm);

        [LibraryImport(gdi32)]
        private static partial IntPtr CreateCompatibleDC(IntPtr hdc);

        [LibraryImport(gdi32)]
        private static partial IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint usage, ref IntPtr bits,
            IntPtr hSection, uint offset);

        [LibraryImport(gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int nDestWidth, int nDestHeight,
            IntPtr hdcSrc, int xSrc, int ySrc, uint dwRop);

        private const int GWL_HICON = -14;

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

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
        public struct BITMAPINFO
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


        public static SKImage? GetWindowIcon(DesktopWindow window)
        {
            IntPtr hIcon = GetWindowLongPtr(window.NativeHandle, GWL_HICON);
            if (hIcon == IntPtr.Zero)
            {
                return null;
            }

            var iconInfo = new ICONINFO();
            if (!GetIconInfo(hIcon, ref iconInfo))
            {
                return null;
            }

            IntPtr hbmColor = iconInfo.hbmColor;
            if (hbmColor == IntPtr.Zero)
            {
                return null;
            }

            var bitmap = new BITMAP();
            int noOfBytes = GetObject(hbmColor, Marshal.SizeOf<BITMAP>(), ref bitmap);
            if (noOfBytes == 0)
            {
                return null;
            }

            int width = bitmap.bmWidth;
            int height = bitmap.bmHeight;

            //we need to create a compatible device context
            IntPtr hdc = CreateCompatibleDC(IntPtr.Zero);

            var bmi = new BITMAPINFO
            {
                biSize = Marshal.SizeOf<BITMAPINFO>(),
                biWidth = width,
                biHeight = -height, //negative height for top-down bitmap
                biPlanes = 1,
                biBitCount = 32, //32-bit color
                biCompression = 0 //BI_RGB
            };

            IntPtr bits = IntPtr.Zero;
            IntPtr hBitmap = CreateDIBSection(hdc, ref bmi, 0, ref bits, IntPtr.Zero, 0);

            //select the icon's bitmap into the compatible DC
            _ = SelectObject(hdc, hbmColor);

            //copy the bitmap to the DIB section
            IntPtr dcMem = CreateCompatibleDC(hdc);
            SelectObject(dcMem, hBitmap);
            bool success = BitBlt(dcMem, 0, 0, width, height, hdc, 0, 0, 0x00CC0020);
            if (!success)
            {
                goto FreeData;
            }

            //now `bits` contains the pixel data in 32-bit RGBA format
            byte[] pixelData = new byte[width * height * 4];
            Marshal.Copy(bits, pixelData, 0, pixelData.Length);

        FreeData:
            _ = DeleteObject(hBitmap);
            _ = DeleteObject(dcMem);
            _ = DeleteObject(hdc);

            return SKImage.FromPixels(new SKImageInfo(width, height, SKColorType.Rgba8888), bits);
        }
    }
}
