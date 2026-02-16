using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Windows.PInvoke;

[SupportedOSPlatform("windows")]
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


    [LibraryImport(user32)]
    internal static partial IntPtr GetDC(IntPtr hWnd);

    [LibraryImport(user32)]
    internal static partial int ReleaseDC(IntPtr hWnd, IntPtr hDC);

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

    [LibraryImport(user32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon,
        int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

    [DllImport(user32, SetLastError = true)]
    internal static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

    [DllImport(user32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetIconInfo(IntPtr hIcon, ref ICONINFO iconInfo);

    [LibraryImport(
        user32,
        EntryPoint = "MessageBoxW",
        StringMarshalling = StringMarshalling.Utf16,
        SetLastError = true)]
    internal static partial int MessageBox(IntPtr hwnd, string text, string caption, uint type);

    [DllImport(user32, SetLastError = true)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    public enum MessageBoxResult
    {
        IDOK = 1,
        IDCANCEL = 2,
        IDABORT = 3,
        IDRETRY = 4,
        IDIGNORE = 5,
        IDYES = 6,
        IDNO = 7,
        IDCLOSE = 8,
        IDHELP = 9,
        IDTRYAGAIN = 10,
        IDCONTINUE = 11
    }

    [Flags]
    public enum MessageBoxType : uint
    {
        // Buttons
        MB_OK = 0x00000000,
        MB_OKCANCEL = 0x00000001,
        MB_ABORTRETRYIGNORE = 0x00000002,
        MB_YESNOCANCEL = 0x00000003,
        MB_YESNO = 0x00000004,
        MB_RETRYCANCEL = 0x00000005,
        MB_CANCELTRYCONTINUE = 0x00000006,

        // Icons
        MB_ICONHAND = 0x00000010,
        MB_ICONQUESTION = 0x00000020,
        MB_ICONEXCLAMATION = 0x00000030,
        MB_ICONASTERISK = 0x00000040,
        MB_ICONWARNING = MB_ICONEXCLAMATION,
        MB_ICONERROR = MB_ICONHAND,
        MB_ICONINFORMATION = MB_ICONASTERISK,

        // Default button
        MB_DEFBUTTON1 = 0x00000000,
        MB_DEFBUTTON2 = 0x00000100,
        MB_DEFBUTTON3 = 0x00000200,
        MB_DEFBUTTON4 = 0x00000300,

        // Modality
        MB_APPLMODAL = 0x00000000,
        MB_SYSTEMMODAL = 0x00001000,
        MB_TASKMODAL = 0x00002000,

        // Other options
        MB_TOPMOST = 0x00040000,
        MB_RIGHT = 0x00080000,
        MB_RTLREADING = 0x00100000,
        MB_SETFOREGROUND = 0x00010000
    }
}
