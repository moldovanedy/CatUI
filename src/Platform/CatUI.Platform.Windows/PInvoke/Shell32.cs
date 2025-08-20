using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Windows.PInvoke
{
    internal static partial class Shell32
    {
        private const string shell32 = "Shell32.dll";

        [DllImport(shell32, CharSet = CharSet.Unicode, EntryPoint = "SHGetFileInfoW")]
        internal static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [LibraryImport(shell32, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial uint ExtractIconExW(string lpszFile, int nIconIndex,
            IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}
