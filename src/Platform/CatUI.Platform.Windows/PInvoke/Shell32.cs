using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Windows.PInvoke;

[SupportedOSPlatform("windows")]
internal static partial class Shell32
{
    private const string shell32 = "Shell32.dll";

    [DllImport(shell32, CharSet = CharSet.Unicode, EntryPoint = "SHGetFileInfoW")]
    internal static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes,
        ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

    [LibraryImport(shell32, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial uint ExtractIconExW(string lpszFile, int nIconIndex,
        IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

    [DllImport(shell32, CharSet = CharSet.Auto, EntryPoint = "SHBrowseForFolderW")]
    internal static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

    [LibraryImport(shell32, EntryPoint = "SHGetPathFromIDListW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    public const uint BIF_RETURNONLYFSDIRS = 0x0001;
    public const uint BIF_NEWDIALOGSTYLE = 0x0040;
    public const uint BIF_EDITBOX = 0x0010;


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

    [StructLayout(LayoutKind.Sequential)]
    public struct BROWSEINFO
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public IntPtr pszDisplayName;
        [MarshalAs(UnmanagedType.LPTStr)] public string lpszTitle;
        public uint ulFlags;
        public IntPtr lpfn;
        public IntPtr lParam;
        public int iImage;
    }
}
