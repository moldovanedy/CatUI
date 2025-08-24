using System;
using System.Runtime.InteropServices;
using CatUI.Platform.Essentials;
using CatUI.Platform.Windows.PInvoke;
using SkiaSharp;

namespace CatUI.Platform.Windows.OS
{
    public class WindowIconWindows : IWindowIcon
    {
        public SKImage? GetWindowIcon(IntPtr hwnd)
        {
            SKImage? icon = GetIconFromFile();
            if (icon != null)
            {
                return icon;
            }

            icon = GetIconFromWindow(hwnd);
            return icon;
        }

        private static SKImage? GetIconFromFile()
        {
            const int maxSize = 512;
            if (Environment.ProcessPath == null)
            {
                return null;
            }

            IntPtr hModule =
                Kernel32.LoadLibraryEx(Environment.ProcessPath, IntPtr.Zero, Kernel32.LOAD_LIBRARY_AS_DATAFILE);
            if (hModule == IntPtr.Zero)
            {
                return null;
            }

            IntPtr? groupId = FindFirstGroupIcon(hModule);
            if (groupId == null)
            {
                return null;
            }

            IntPtr hResInfo = Kernel32.FindResource(hModule, groupId.Value, Kernel32.RT_GROUP_ICON);
            if (hResInfo == IntPtr.Zero)
            {
                return null;
            }

            uint resSize = Kernel32.SizeofResource(hModule, hResInfo);
            IntPtr hResData = Kernel32.LoadResource(hModule, hResInfo);
            IntPtr pRes = Kernel32.LockResource(hResData);

            byte[] groupData = new byte[resSize];
            Marshal.Copy(pRes, groupData, 0, groupData.Length);

            //parse group icon header
            GCHandle handle = GCHandle.Alloc(groupData, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                var dir = Marshal.PtrToStructure<Kernel32.GRPICONDIR>(ptr);

                int entrySize = Marshal.SizeOf<Kernel32.GRPICONDIRENTRY>();
                IntPtr entryPtr = IntPtr.Add(ptr, Marshal.SizeOf<Kernel32.GRPICONDIR>());

                Kernel32.GRPICONDIRENTRY? bestEntry = null;
                for (int i = 0; i < dir.idCount; i++)
                {
                    var entry = Marshal.PtrToStructure<Kernel32.GRPICONDIRENTRY>(entryPtr);

                    int width = entry.bWidth == 0 ? maxSize : entry.bWidth;
                    int height = entry.bHeight == 0 ? maxSize : entry.bHeight;
                    if (width != height)
                    {
                        width = Math.Min(width, height);
                    }

                    if (bestEntry == null || width >= (bestEntry.Value.bWidth == 0 ? maxSize : bestEntry.Value.bWidth))
                    {
                        bestEntry = entry;
                    }

                    entryPtr = IntPtr.Add(entryPtr, entrySize);
                }

                if (bestEntry == null)
                {
                    return null;
                }

                //load the actual (best) RT_ICON resource
                IntPtr iconResInfo = Kernel32.FindResource(hModule, bestEntry.Value.nID, Kernel32.RT_ICON);
                if (iconResInfo == IntPtr.Zero)
                {
                    return null;
                }

                uint iconSize = Kernel32.SizeofResource(hModule, iconResInfo);
                IntPtr iconResData = Kernel32.LoadResource(hModule, iconResInfo);
                IntPtr iconPtr = Kernel32.LockResource(iconResData);

                IntPtr hIcon = User32.CreateIconFromResourceEx(
                    iconPtr, iconSize, true, 0x00030000,
                    maxSize, maxSize, 0);
                if (hIcon == IntPtr.Zero)
                {
                    return null;
                }

                try
                {
                    return FinalizeAndReturnImage(
                        hIcon,
                        bestEntry.Value.bWidth == 0 ? maxSize : bestEntry.Value.bWidth);
                }
                finally
                {
                    User32.DestroyIcon(hIcon);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private static SKImage? GetIconFromWindow(IntPtr hwnd)
        {
            IntPtr hIcon = User32.SendMessage(hwnd, User32.WM_GETICON, User32.ICON_BIG, IntPtr.Zero);

            //if that fails, we try class icons
            if (hIcon == IntPtr.Zero)
            {
                hIcon = User32.GetClassLongPtr(hwnd, User32.GCL_HICON);
            }

            if (hIcon == IntPtr.Zero)
            {
                hIcon = User32.GetClassLongPtr(hwnd, User32.GCL_HICONSM);
            }

            if (hIcon == IntPtr.Zero)
            {
                return null;
            }

            var iconInfo = new User32.ICONINFO();
            if (!User32.GetIconInfo(hIcon, ref iconInfo))
            {
                return null;
            }

            IntPtr hbmColor = iconInfo.hbmColor;
            if (hbmColor == IntPtr.Zero)
            {
                return null;
            }

            int noOfBytes = Gdi32.GetObject(hbmColor, Marshal.SizeOf<Gdi32.BITMAP>(), out Gdi32.BITMAP bitmap);
            if (noOfBytes == 0)
            {
                return null;
            }

            int width = bitmap.bmWidth;
            int height = bitmap.bmHeight;
            if (width != height)
            {
                width = Math.Min(width, height);
            }

            return FinalizeAndReturnImage(hIcon, width);
        }

        private static SKImage? FinalizeAndReturnImage(IntPtr hIcon, int size)
        {
            //we need to create a compatible device context
            IntPtr hdc = Gdi32.CreateCompatibleDC(IntPtr.Zero);
            var bmi = new Gdi32.BITMAPINFO
            {
                biSize = Marshal.SizeOf<Gdi32.BITMAPINFO>(),
                biWidth = size,
                biHeight = -size, //negative height for top-down bitmap
                biPlanes = 1,
                biBitCount = 32, //32-bit color
                biCompression = 0 //BI_RGB
            };

            IntPtr hBitmap = Gdi32.CreateDIBSection(
                hdc, ref bmi, Gdi32.DIB_RGB_COLORS,
                out IntPtr bits, IntPtr.Zero, 0);
            IntPtr old = Gdi32.SelectObject(hdc, hBitmap);

            User32.DrawIconEx(
                hdc, 0, 0, hIcon, size, size,
                0, IntPtr.Zero, User32.DI_NORMAL);

            //copy pixels into a managed buffer
            int stride = size * 4;
            byte[] pixelData = new byte[stride * size];
            Marshal.Copy(bits, pixelData, 0, pixelData.Length);

            Gdi32.SelectObject(hdc, old);
            _ = Gdi32.DeleteObject(hBitmap);

            return SKImage.FromPixels(
                new SKImageInfo(size, size, SKColorType.Rgba8888),
                SKData.CreateCopy(pixelData),
                stride);
        }

        private static IntPtr? FindFirstGroupIcon(IntPtr hModule)
        {
            IntPtr result = IntPtr.Zero;

            Kernel32.EnumResourceNames(hModule, Kernel32.RT_GROUP_ICON,
                (_, _, name, _) =>
                {
                    result = name;
                    //stop after first one
                    return false;
                },
                IntPtr.Zero);

            return result == IntPtr.Zero ? null : result;
        }
    }
}
