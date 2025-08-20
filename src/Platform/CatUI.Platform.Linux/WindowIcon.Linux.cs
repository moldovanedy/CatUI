using System;
using CatUI.Platform.Essentials;
using CatUI.Platform.Linux.PInvoke;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

namespace CatUI.Platform.Linux
{
    public class WindowIconLinux : IWindowIcon
    {
        public unsafe SKImage? GetWindowIcon(IntPtr windowHandle)
        {
            if (GLFW.GetPlatform() != OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11)
            {
                return null;
            }

            IntPtr x11Display = GLFW.GetX11Display();
            IntPtr netWmIcon = X11.XInternAtom(x11Display, "_NET_WM_ICON", false);
            IntPtr cardinal = X11.XInternAtom(x11Display, "CARDINAL", false);

            int status = X11.XGetWindowProperty(x11Display, windowHandle, netWmIcon,
                IntPtr.Zero, 1024 * 1024 * 4, false, cardinal,
                out IntPtr _, out int _, out IntPtr nItems, out IntPtr _, out IntPtr prop);

            if (status != 0 || prop == IntPtr.Zero)
            {
                return null;
            }

            int items = nItems.ToInt32();

            nuint* baseData = (nuint*)prop.ToPointer();
            uint width = (uint)baseData[0];
            uint height = (uint)baseData[1];
            int pixelCount = (int)(width * height);

            //only process the first icon's pixels (might contain multiple icons)
            if (pixelCount <= 0 || pixelCount + 2 > items)
            {
                X11.XFree(prop);
                return null;
            }

            byte[] buffer = new byte[pixelCount * 4];
            for (int i = 0; i < pixelCount; i++)
            {
                //convert ARGB -> RGBA
                uint pixel = (uint)baseData[2 + i];
                uint rgba = ((pixel & 0x00FFFFFFu) << 8) | (pixel >> 24);

                int b = i * 4;
                buffer[b + 0] = (byte)((rgba >> 0) & 0xFF);
                buffer[b + 1] = (byte)((rgba >> 8) & 0xFF);
                buffer[b + 2] = (byte)((rgba >> 16) & 0xFF);
                buffer[b + 3] = (byte)((rgba >> 24) & 0xFF);
            }

            X11.XFree(prop);

            //create an image by copying the pixel data so memory lifetime is safe.
            var info = new SKImageInfo((int)width, (int)height, SKColorType.Rgba8888);
            fixed (byte* p = buffer)
            {
                return SKImage.FromPixelCopy(info, (IntPtr)p, info.RowBytes);
            }
        }
    }
}
