// ReSharper disable InconsistentNaming

using System;
using System.Runtime.InteropServices;

namespace CatUI.Platform.Windows.PInvoke
{
    internal static class Comdlg32
    {
        private const string comdlg32 = "comdlg32.dll";

        [DllImport(comdlg32, SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "GetOpenFileNameW")]
        public static extern bool GetOpenFileName(ref OPENFILENAME ofn);

        [DllImport(comdlg32, SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "GetSaveFileNameW")]
        public static extern bool GetSaveFileName(ref OPENFILENAME ofn);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OPENFILENAME
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;

            // public IntPtr lpEditInfo;
            // public string lpstrPrompt;

            public IntPtr pvReserved;
            public int dwReserved;
            public int FlagsEx;
        }

        public const int OFN_EXPLORER = 0x00080000;
        public const int OFN_FILEMUSTEXIST = 0x00001000;
        public const int OFN_PATHMUSTEXIST = 0x00000800;
        public const int OFN_ALLOWMULTISELECT = 0x00000200;
        public const int OFN_ENABLESIZING = 0x00800000;
    }
}
