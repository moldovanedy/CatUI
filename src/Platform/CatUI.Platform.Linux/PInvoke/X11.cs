using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable InconsistentNaming

namespace CatUI.Platform.Linux.PInvoke;

[SupportedOSPlatform("linux")]
internal static partial class X11
{
    private const string libX11 = "libX11";

    [LibraryImport(libX11)]
    internal static partial int XGetWindowProperty(
        IntPtr display, IntPtr w, IntPtr property, IntPtr long_offset,
        IntPtr long_length, [MarshalAs(UnmanagedType.Bool)] bool delete, IntPtr req_type,
        out IntPtr actual_type_return, out int actual_format_return,
        out IntPtr nitems_return, out IntPtr bytes_after_return, out IntPtr prop_return);

    [LibraryImport(libX11, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr XInternAtom(
        IntPtr display,
        string atom_name,
        [MarshalAs(UnmanagedType.Bool)] bool only_if_exists);

    [LibraryImport(libX11)]
    internal static partial void XFree(IntPtr data);
}
