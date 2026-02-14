using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Windows.PInvoke;

[SupportedOSPlatform("windows")]
internal static partial class Kernel32
{
    private const string kernel32 = "kernel32.dll";

    internal const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
    internal static readonly IntPtr RT_GROUP_ICON = 14;
    internal static readonly IntPtr RT_ICON = 3;

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct GRPICONDIR
    {
        public ushort idReserved; // must be 0
        public ushort idType; // 1 for icons
        public ushort idCount; // number of images
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct GRPICONDIRENTRY
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public ushort wPlanes;
        public ushort wBitCount;
        public uint dwBytesInRes;
        public ushort nID; // resource ID
    }

    [LibraryImport(kernel32, StringMarshalling = StringMarshalling.Utf16, EntryPoint = "LoadLibraryExW")]
    internal static partial IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

    [LibraryImport(kernel32, EntryPoint = "FindResourceA")]
    internal static partial IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

    [LibraryImport(kernel32)]
    internal static partial IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

    [LibraryImport(kernel32)]
    internal static partial IntPtr LockResource(IntPtr hResData);

    [LibraryImport(kernel32)]
    internal static partial uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

    [LibraryImport(kernel32, SetLastError = true, EntryPoint = "EnumResourceNamesW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EnumResourceNames(
        IntPtr hModule,
        IntPtr lpszType,
        EnumResNameProc lpEnumFunc,
        IntPtr lParam);

    internal delegate bool EnumResNameProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);
}
