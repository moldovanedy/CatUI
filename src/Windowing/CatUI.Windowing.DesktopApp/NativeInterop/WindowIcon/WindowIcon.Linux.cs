using System;
using System.Runtime.InteropServices;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

// ReSharper disable InconsistentNaming

namespace CatUI.Windowing.DesktopApp.NativeInterop.WindowIcon
{
    public static partial class WindowIconLinux
    {
        private const string libX11 = "libX11";

        [LibraryImport(libX11)]
        private static partial int XGetWindowProperty(
            IntPtr display, IntPtr w, IntPtr property, IntPtr long_offset,
            IntPtr long_length, [MarshalAs(UnmanagedType.Bool)] bool delete, IntPtr req_type,
            out IntPtr actual_type_return, out int actual_format_return,
            out IntPtr nitems_return, out IntPtr bytes_after_return, out IntPtr prop_return);

        [LibraryImport(libX11, StringMarshalling = StringMarshalling.Utf8)]
        private static partial IntPtr XInternAtom(
            IntPtr display,
            string atom_name,
            [MarshalAs(UnmanagedType.Bool)] bool only_if_exists);

        [LibraryImport(libX11)]
        private static partial void XFree(IntPtr data);


        public static unsafe SKImage? GetWindowIcon(DesktopWindow window)
        {
            if (GLFW.GetPlatform() != OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11)
            {
                return null;
            }

            IntPtr x11Window = window.NativeHandle;
            IntPtr x11Display = GLFW.GetX11Display();

            IntPtr netWmIcon = XInternAtom(x11Display, "_NET_WM_ICON", false);
            IntPtr cardinal = XInternAtom(x11Display, "CARDINAL", false);

            int status = XGetWindowProperty(x11Display, x11Window, netWmIcon,
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
                XFree(prop);
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

            XFree(prop);

            //create an image by copying the pixel data so memory lifetime is safe.
            var info = new SKImageInfo((int)width, (int)height, SKColorType.Rgba8888);
            fixed (byte* p = buffer)
            {
                return SKImage.FromPixelCopy(info, (IntPtr)p, info.RowBytes);
            }
        }
    }
}
