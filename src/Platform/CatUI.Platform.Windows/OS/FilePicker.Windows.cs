using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data;
using CatUI.Platform.CommonInterface;
using CatUI.Platform.Windows.PInvoke;

namespace CatUI.Platform.Windows.OS
{
    public class FilePickerWindows : IFilePicker
    {
        private const int SINGLE_FILE_BUFFER = 4096;
        private const int MULTI_FILE_BUFFER = 65536;

        /// <inheritdoc />
        /// <remarks>
        /// <para>
        /// On Windows: <c>initialLocation</c>, <c>customSubmitButtonText</c>, and <c>choices</c> are ignored,
        /// as those are not supported by the win32 dialogs.
        /// </para>
        /// <para><inheritdoc/></para>
        /// </remarks>
        public async Task<IFilePicker.OpenFilesResponse?> OpenFilesAsync(
            string dialogTitle,
            bool canSelectMultiple,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            FilePath? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                Comdlg32.OPENFILENAME ofn = GetFileCommonStruct(
                    dialogTitle,
                    canSelectMultiple,
                    filterPattern,
                    initialLocation,
                    windowIdentifier);

                bool success = false;
                await Task.Run(() =>
                {
                    success = Comdlg32.GetOpenFileName(ref ofn);
                });

                if (!success)
                {
                    return null;
                }

                try
                {
                    unsafe
                    {
                        byte* rawPaths = (byte*)ofn.lpstrFile.ToPointer();

                        FilePath[] paths;
                        //a memory guard so we don't have buffer overflows and, implicitly, AccessViolationException
                        int parsedBytes = 0;

                        if (canSelectMultiple)
                        {
                            int dirPathLen = StrlenW(rawPaths);
                            parsedBytes += dirPathLen + 2;
                            if (parsedBytes >= MULTI_FILE_BUFFER)
                            {
                                throw new Exception(
                                    "OpenFilesAsync: result buffer was overflown; memory guard stopped a segfault.");
                            }

                            string dir = Encoding.UTF8.GetString(rawPaths, dirPathLen);
                            //a NULL byte is put after each path
                            rawPaths += dirPathLen + 2;

                            List<string> files = [];
                            while (*rawPaths != 0)
                            {
                                int pathLen = StrlenW(rawPaths);
                                parsedBytes += dirPathLen + 2;
                                if (parsedBytes >= MULTI_FILE_BUFFER)
                                {
                                    throw new Exception(
                                        "OpenFilesAsync: result buffer was overflown; memory guard stopped a segfault.");
                                }

                                files.Add(Encoding.UTF8.GetString(rawPaths, pathLen));
                                rawPaths += pathLen + 2;
                            }

                            if (files.Count == 0)
                            {
                                //this means that dir is the actual path
                                paths = [new FilePath(dir, false)];
                            }
                            else
                            {
                                paths = new FilePath[files.Count];
                                for (int i = 0; i < files.Count; i++)
                                {
                                    paths[i] = new FilePath(Path.Combine(dir, files[i]), false);
                                }
                            }
                        }
                        else
                        {
                            int pathLen = StrlenW(rawPaths);
                            if (pathLen >= SINGLE_FILE_BUFFER)
                            {
                                throw new Exception(
                                    "OpenFilesAsync: result buffer was overflown; memory guard stopped a segfault.");
                            }

                            paths = [new FilePath(Encoding.UTF8.GetString(rawPaths, pathLen), false)];
                        }

                        IFilePicker.FileFilter? selectedFilter = null;
                        if (filterPattern != null
                         && ofn.nFilterIndex > 0
                         && ofn.nFilterIndex <= filterPattern.Filters.Length)
                        {
                            selectedFilter = filterPattern.Filters[ofn.nFilterIndex - 1];
                        }

                        return new IFilePicker.OpenFilesResponse(paths, selectedFilter);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ofn.lpstrFile);
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para>
        /// On Windows: <c>canSelectMultiple</c>, <c>initialLocation</c>, <c>customSubmitButtonText</c>, and
        /// <c>choices</c> are ignored, as those are not supported by the win32 dialogs.
        /// </para>
        /// <para><inheritdoc/></para>
        /// </remarks>
        [SupportedOSPlatform("windows")]
        public async Task<IFilePicker.OpenDirectoriesResponse?> OpenDirectoriesAsync(
            string dialogTitle,
            bool canSelectMultiple,
            FilePath? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                string finalPath = "";
                Thread t = new(() =>
                {
                    Shell32.BROWSEINFO bi = new();
                    if (windowIdentifier is IntPtr hwnd)
                    {
                        bi.hwndOwner = hwnd;
                    }

                    bi.pidlRoot = IntPtr.Zero;
                    bi.lpszTitle = dialogTitle;
                    bi.ulFlags = Shell32.BIF_RETURNONLYFSDIRS | Shell32.BIF_NEWDIALOGSTYLE | Shell32.BIF_EDITBOX;

                    IntPtr pidl = Shell32.SHBrowseForFolder(ref bi);
                    if (pidl == IntPtr.Zero)
                    {
                        return;
                    }

                    unsafe
                    {
                        IntPtr pathPtr = Marshal.AllocHGlobal(SINGLE_FILE_BUFFER);
                        try
                        {
                            if (!Shell32.SHGetPathFromIDList(pidl, pathPtr))
                            {
                                return;
                            }

                            byte* rawPath = (byte*)pathPtr.ToPointer();
                            int pathLen = StrlenW(rawPath);
                            if (pathLen > SINGLE_FILE_BUFFER)
                            {
                                throw new Exception(
                                    "OpenDirectoriesAsync: result buffer was overflown; memory guard stopped a segfault.");
                            }

                            finalPath = Encoding.UTF8.GetString(rawPath, pathLen);
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(pathPtr);
                        }
                    }
                });

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                await Task.Run(() =>
                {
                    t.Join();
                });

                return
                    string.IsNullOrWhiteSpace(finalPath)
                        ? null
                        : new IFilePicker.OpenDirectoriesResponse([new FilePath(finalPath, true)]);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para>
        /// On Windows: <c>fileName</c>, <c>initialLocation</c>, <c>customSubmitButtonText</c>, and
        /// <c>choices</c> are ignored, as those are not supported by the win32 dialogs.
        /// </para>
        /// <para><inheritdoc/></para>
        /// </remarks>
        public async Task<IFilePicker.SaveFileResponse?> SaveFileAsync(
            string dialogTitle,
            string fileName,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            FilePath? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                Comdlg32.OPENFILENAME ofn = GetFileCommonStruct(
                    dialogTitle,
                    false,
                    filterPattern,
                    initialLocation,
                    windowIdentifier);

                bool success = false;
                await Task.Run(() =>
                {
                    success = Comdlg32.GetSaveFileName(ref ofn);
                });

                if (!success)
                {
                    return null;
                }

                try
                {
                    unsafe
                    {
                        byte* rawPath = (byte*)ofn.lpstrFile.ToPointer();

                        IFilePicker.FileFilter? selectedFilter = null;
                        if (filterPattern != null && ofn.nFilterIndex > 0 &&
                            ofn.nFilterIndex <= filterPattern.Filters.Length)
                        {
                            selectedFilter = filterPattern.Filters[ofn.nFilterIndex - 1];
                        }

                        int pathLen = StrlenW(rawPath);
                        if (pathLen > SINGLE_FILE_BUFFER)
                        {
                            throw new Exception(
                                "SaveFilesAsync: result buffer was overflown; memory guard stopped a segfault.");
                        }

                        return new IFilePicker.SaveFileResponse(
                            new FilePath(Encoding.UTF8.GetString(rawPath, pathLen), false),
                            selectedFilter);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ofn.lpstrFile);
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private static Comdlg32.OPENFILENAME GetFileCommonStruct(
            string dialogTitle,
            bool canSelectMultiple,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            FilePath? initialLocation = null,
            object? windowIdentifier = null)
        {
            var ofn = new Comdlg32.OPENFILENAME();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.Flags =
                Comdlg32.OFN_EXPLORER
              | Comdlg32.OFN_FILEMUSTEXIST
              | Comdlg32.OFN_PATHMUSTEXIST
              | Comdlg32.OFN_ENABLESIZING;

            if (canSelectMultiple)
            {
                ofn.Flags |= Comdlg32.OFN_ALLOWMULTISELECT;
            }

            if (filterPattern != null)
            {
                ofn.lpstrFilter = GetFilterString(filterPattern);
                ofn.nFilterIndex = Math.Clamp(filterPattern.DefaultFilterIndex, 0, filterPattern.Filters.Length) + 1;
            }

            if (initialLocation != null)
            {
                ofn.lpstrInitialDir = initialLocation.NativePath as string ?? "";
            }

            ofn.lpstrTitle = dialogTitle;

            //prepare a large buffer, so it can hold possibly hundreds of files if multiple selection is allowed
            int bufferSize = canSelectMultiple ? MULTI_FILE_BUFFER : SINGLE_FILE_BUFFER;
            ofn.lpstrFile = Marshal.AllocHGlobal(bufferSize);

            //fill the buffer with zeroes to avoid errors
            unsafe
            {
                byte* traverser = (byte*)ofn.lpstrFile.ToPointer();
                for (int i = 0; i < bufferSize; i++)
                {
                    *traverser = 0;
                    traverser++;
                }
            }

            ofn.nMaxFile = bufferSize;

            if (windowIdentifier is IntPtr hwnd)
            {
                ofn.hwndOwner = hwnd;
            }

            return ofn;
        }

        private static string GetFilterString(IFilePicker.FileFiltersArgument filterArg)
        {
            StringBuilder sb = new();
            foreach (IFilePicker.FileFilter filter in filterArg.Filters)
            {
                sb.Append(filter.Label);
                sb.Append((char)0);

                sb.Append(filter.Pattern.ToString(';'));
                sb.Append((char)0);
            }

            sb.Append((char)0);
            return sb.ToString();
        }

        private static unsafe int StrlenW(byte* ptr)
        {
            int c = 0;
            while (*ptr != 0)
            {
                c += 2;
                ptr += 2;
            }

            return c;
        }
    }
}
