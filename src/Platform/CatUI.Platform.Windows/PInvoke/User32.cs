using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Windows.PInvoke
{
    internal static partial class User32
    {
        private const string user32 = "user32.dll";

        public const int WM_GETICON = 0x007F;
        public const int ICON_BIG = 1;
        public const int GCL_HICON = -14;
        public const int GCL_HICONSM = -34;
        public const int DI_NORMAL = 0x0003;

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport(user32, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(user32, SetLastError = true)]
        internal static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        [DllImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetIconInfo(IntPtr hIcon, ref ICONINFO iconInfo);

        [LibraryImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon,
            int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        [LibraryImport(user32, SetLastError = true)]
        internal static partial IntPtr CreateIconFromResourceEx(
            IntPtr presbits,
            uint dwResSize,
            [MarshalAs(UnmanagedType.Bool)] bool fIcon,
            int dwVer,
            int cxDesired,
            int cyDesired,
            uint Flags);

        [LibraryImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DestroyIcon(IntPtr hIcon);
    }
}
