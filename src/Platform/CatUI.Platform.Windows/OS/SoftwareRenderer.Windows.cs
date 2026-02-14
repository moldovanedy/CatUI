using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CatUI.Platform.CommonInterface;
using CatUI.Platform.Windows.PInvoke;

namespace CatUI.Platform.Windows.OS;

[SupportedOSPlatform("windows")]
public class SoftwareRendererWindows : ISoftwareRenderer
{
    public void Draw(
        IntPtr nativeWindow,
        IntPtr pixelBuffer,
        int framebufferWidth,
        int framebufferHeight,
        int bytesPerRow,
        int windowWidth,
        int windowHeight)
    {
        if (nativeWindow == IntPtr.Zero || pixelBuffer == IntPtr.Zero)
        {
            return;
        }

        Gdi32.BITMAPINFO dib = new();
        dib.bmiHeader.biSize = Marshal.SizeOf<Gdi32.BITMAPINFOHEADER>();
        dib.bmiHeader.biWidth = framebufferWidth;
        dib.bmiHeader.biHeight = -framebufferHeight; //negative = top-down
        dib.bmiHeader.biPlanes = 1;
        dib.bmiHeader.biBitCount = 32; // 32-bit BGRA
        dib.bmiHeader.biCompression = (int)Gdi32.BI_BITFIELDS;
        dib.bmiHeader.biSizeImage = bytesPerRow * framebufferHeight;

        //RGBA 32-bit
        dib.redMask = 0x000000FF;
        dib.greenMask = 0x0000FF00;
        dib.blueMask = 0x00FF0000;
        dib.alphaMask = 0xFF000000;

        IntPtr hdc = User32.GetDC(nativeWindow);
        try
        {
            int rc = Gdi32.StretchDIBits(
                hdc,
                0, 0, framebufferWidth, framebufferHeight,
                0, 0, framebufferWidth, framebufferHeight,
                pixelBuffer,
                ref dib,
                Gdi32.DIB_RGB_COLORS,
                Gdi32.SRCCOPY);

            if (rc == 0)
            {
                //ERROR
            }
        }
        finally
        {
            _ = User32.ReleaseDC(nativeWindow, hdc);
        }
    }

    public void Resized(int newWidth, int newHeight)
    {
    }
}
