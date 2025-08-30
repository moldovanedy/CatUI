using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Utils;

namespace CatUI.Platform.Essentials
{
    /// <summary>
    /// Describes the interface for presenting the user with a native file picker for opening files, saving files,
    /// or selecting directories.
    /// </summary>
    public interface IFilePicker
    {
        /// <summary>
        /// Will return the necessary info of the file (or multiple files if <c>canSelectMultiple</c> is set) the user
        /// wants to open (the files are read-only).
        /// </summary>
        /// <param name="dialogTitle">The title of the dialog.</param>
        /// <param name="canSelectMultiple">Whether the user can select multiple files or not.</param>
        /// <param name="filterPattern">
        /// The filter pattern to apply to restrict the file types the user is allowed to choose. The runtime
        /// platform might ignore this, so you need to handle the case where the files are in an unexpected format.
        /// </param>
        /// <param name="initialLocation">The initial location the picker will open in. This might get ignored.</param>
        /// <param name="customSubmitButtonText">
        /// If supported by the platform, sets a custom label for the "Submit" (or Open) button.
        /// </param>
        /// <param name="choices">
        /// If supported by the platform, uses additional "options" so the user can further customize what your
        /// application should do to the file they selected. See <see cref="PickerChoicesRequest"/> for more info.
        /// </param>
        /// <param name="windowIdentifier">
        /// The platform-specific window ID for which the picker will be a modal. This is different for each platform,
        /// consult the manual for more info.
        /// </param>
        /// <param name="cancellationToken">The cancellation token that will abort this operation if triggered.</param>
        /// <returns>A response if the user selected files, null if the user dismissed the dialog.</returns>
        Task<OpenFilesResponse?> OpenFilesAsync(
            string dialogTitle,
            bool canSelectMultiple,
            FileFiltersArgument? filterPattern = null,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// Will return the necessary info of the directory (or multiple files if <c>canSelectMultiple</c> is set)
        /// the user wants to open (all the files/subdirectories from the selected directories are either read-only
        /// or read-write, depending on the set flags).
        /// </summary>
        /// <remarks>
        /// Note that requesting read-write access might trigger some additional prompts on the runtime platform or
        /// need to declare some permissions (possibly on iOS and macOS).
        /// </remarks>
        /// <param name="dialogTitle">The title of the dialog.</param>
        /// <param name="canSelectMultiple">Whether the user can select multiple files or not.</param>
        /// <param name="initialLocation">The initial location the picker will open in. This might get ignored.</param>
        /// <param name="customSubmitButtonText">
        /// If supported by the platform, sets a custom label for the "Submit" (or Open) button.
        /// </param>
        /// <param name="choices">
        /// If supported by the platform, uses additional "options" so the user can further customize what your
        /// application should do to the file they selected. See <see cref="PickerChoicesRequest"/> for more info.
        /// </param>
        /// <param name="windowIdentifier">
        /// The platform-specific window ID for which the picker will be a modal. This is different for each platform,
        /// consult the manual for more info.
        /// </param>
        /// <param name="cancellationToken">The cancellation token that will abort this operation if triggered.</param>
        /// <returns>A response if the user selected directories, null if the user dismissed the dialog.</returns>
        Task<OpenDirectoriesResponse?> OpenDirectoriesAsync(
            string dialogTitle,
            bool canSelectMultiple,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// Will return the necessary info for writing into the user-selected file (the file is write-only, but you
        /// might need to create it if it doesn't exist).
        /// </summary>
        /// <param name="dialogTitle">The title of the dialog.</param>
        /// <param name="fileName">The proposed name for the file to be saved.</param>
        /// <param name="filterPattern">
        /// The filter pattern to apply to restrict the file types the user is allowed to choose. The runtime
        /// platform might ignore this, so you need to handle the case where the files are in an unexpected format.
        /// </param>
        /// <param name="initialLocation">The initial location the picker will open in. This might get ignored.</param>
        /// <param name="customSubmitButtonText">
        /// If supported by the platform, sets a custom label for the "Submit" (or Open) button.
        /// </param>
        /// <param name="choices">
        /// If supported by the platform, uses additional "options" so the user can further customize what your
        /// application should do to the file they selected. See <see cref="PickerChoicesRequest"/> for more info.
        /// </param>
        /// <param name="windowIdentifier">
        /// The platform-specific window ID for which the picker will be a modal. This is different for each platform,
        /// consult the manual for more info.
        /// </param>
        /// <param name="cancellationToken">The cancellation token that will abort this operation if triggered.</param>
        /// <returns>A response if the user selected a file, null if the user dismissed the dialog.</returns>
        Task<SaveFileResponse?> SaveFileAsync(
            string dialogTitle,
            string fileName,
            FileFiltersArgument? filterPattern = null,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// Will return the necessary info for writing into the user-selected directory (the files are write-only, but
        /// you might need to create them if they don't exist).
        /// </summary>
        /// <param name="dialogTitle">The title of the dialog.</param>
        /// <param name="fileNames">The file names for all the files you want to be saved.</param>
        /// <param name="initialLocation">The initial location the picker will open in. This might get ignored.</param>
        /// <param name="customSubmitButtonText">
        /// If supported by the platform, sets a custom label for the "Submit" (or Open) button.
        /// </param>
        /// <param name="choices">
        /// If supported by the platform, uses additional "options" so the user can further customize what your
        /// application should do to the file they selected. See <see cref="PickerChoicesRequest"/> for more info.
        /// </param>
        /// <param name="windowIdentifier">
        /// The platform-specific window ID for which the picker will be a modal. This is different for each platform,
        /// consult the manual for more info.
        /// </param>
        /// <param name="cancellationToken">The cancellation token that will abort this operation if triggered.</param>
        /// <returns>A response if the user selected a file, null if the user dismissed the dialog.</returns>
        Task<SaveFilesInDirectoryResponse?> SaveFilesInDirectoryAsync(
            string dialogTitle,
            string[] fileNames,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null);

        class OpenFilesResponse : PickerResponse
        {
            /// <summary>
            /// The URIs for all the files selected by the user. The URI is used to access the files; it is
            /// platform-dependent, so don't make assumptions about its format, and instead use `FUNCTION` to access
            /// the file or `FUNCTION` to convert it to a regular path.
            /// </summary>
            /// <remarks>If the user couldn't have chosen multiple files, this will have a single element.</remarks>
            public Uri[] FileUris { get; }

            /// <summary>
            /// If the runtime platform supports it, returns the file filter selected by the user. It is different from
            /// the object that you have given in the request.
            /// </summary>
            public FileFilter? SelectedFileFilter { get; }

            public OpenFilesResponse(
                Uri[] fileUris,
                FileFilter? selectedFileFilter = null,
                Dictionary<string, string>? pickerChoicesResponse = null) : base(pickerChoicesResponse)
            {
                FileUris = fileUris;
                SelectedFileFilter = selectedFileFilter;
            }
        }

        class OpenDirectoriesResponse : PickerResponse
        {
            /// <summary>
            /// See <see cref="OpenFilesResponse.FileUris"/>. It's exactly the same, but for directories instead of
            /// files. Note that selecting multiple directories is not widely supported (only Windows, macOS,
            /// and possibly Linux), so you'll generally get only one directory.
            /// </summary>
            public Uri[] DirectoryUris => InodeUris;

            public OpenDirectoriesResponse(
                Uri[] directoryUris,
                Dictionary<string, string>? pickerChoicesResponse = null) : base(pickerChoicesResponse)
            {
                InodeUris = directoryUris;
            }
        }

        class SaveFileResponse : PickerResponse
        {
            /// <summary>
            /// The URI for the file selected by the user (you will write into it). The URI is used to create the file;
            /// it is platform-dependent, so don't make assumptions about its format, and instead use `FUNCTION` to
            /// access the file or `FUNCTION` to convert it to a regular path.
            /// </summary>
            public Uri FileUri => InodeUris[0];

            /// <inheritdoc cref="OpenFilesResponse.SelectedFileFilter"/>
            public FileFilter? SelectedFileFilter { get; }

            public SaveFileResponse(
                Uri fileUri,
                FileFilter? selectedFileFilter = null,
                Dictionary<string, string>? pickerChoicesResponse = null) : base(pickerChoicesResponse)
            {
                InodeUris = [fileUri];
                SelectedFileFilter = selectedFileFilter;
            }
        }

        class SaveFilesInDirectoryResponse : PickerResponse
        {
            /// <summary>
            /// The URIs for all the files selected by the user (you will write into them). The URIs are used to create
            /// the files; it is platform-dependent, so don't make assumptions about its format, and instead use
            /// `FUNCTION` to access the file or `FUNCTION` to convert it to a regular path.
            /// </summary>
            public Uri[] FileUris => InodeUris;

            public SaveFilesInDirectoryResponse(
                Uri[] fileUris,
                Dictionary<string, string>? pickerChoicesResponse = null) : base(pickerChoicesResponse)
            {
                InodeUris = fileUris;
            }
        }

        class PickerResponse
        {
            /// <summary>
            /// If you have given <see cref="PickerChoicesRequest"/>s in the request AND the runtime platform supports
            /// choices, this will contain the choices the user made: the key is the ID given in <c>Options</c>, the
            /// value is the value selected by the user. For boolean options, the value can be "true" or "false",
            /// respectively.
            /// </summary>
            /// <remarks>
            /// You know when the platform does not support this option when you set <see cref="PickerChoicesRequest"/>,
            /// but this is null.
            /// </remarks>
            public Dictionary<string, string>? PickerChoicesResponse { get; }

            protected Uri[] InodeUris { get; set; } = [];

            public PickerResponse(Dictionary<string, string>? pickerChoicesResponse = null)
            {
                PickerChoicesResponse = pickerChoicesResponse;
            }
        }

        /// <summary>
        /// Represents a file filter that you can apply on the file picker to limit user selection to just some file
        /// types. Note that some platforms might allow the user to select any file, regardless of these filters, so
        /// ensure you handle that case correctly.
        /// </summary>
        /// <param name="Label">
        /// The human-readable label that appears for this filter (e.g. "Image files", "Text files", "All files").
        /// </param>
        /// <param name="Pattern">
        /// The glob-like pattern used for filtering files. Try to use simple patterns (they are usually only for
        /// checking extensions) like "*.png" or "*.*", avoid using complex patterns that include "[...]" or negation
        /// ("!"), as those might not be supported on all platforms.
        /// </param>
        record FileFilter(string Label, FileGlobPattern Pattern);

        /// <summary>
        /// The argument you give to filtering files on the open file dialog.
        /// </summary>
        /// <param name="Filters">The filters the user can select from.</param>
        /// <param name="DefaultFilterIndex">
        /// The index of the filter that is selected by default. An out-of-range index will fall back to the first
        /// filter.
        /// </param>
        record FileFiltersArgument(FileFilter[] Filters, int DefaultFilterIndex);

        /// <summary>
        /// Represents additional options to be shown in the picker if supported by the runtime platform (this is only
        /// supported on Windows and most Linux systems).
        /// </summary>
        /// <param name="ID">
        /// The ID for this option, used in code. It can be any valid string, except an empty string.
        /// </param>
        /// <param name="Label">The human-readable label that appears for this option (e.g. "Encoding").</param>
        /// <param name="Options">
        /// The options the user has, generally displayed as a list-box. It is an array of tuples, where the first item
        /// of the tuple is the ID, the second item of the tuple is the human-readable name for that option. If the
        /// array is empty, it will be treated as a boolean and will try to show it as a checkbox (e.g.
        /// [("utf8", "UTF-8"), ("utf16", "UTF-16"), ("iso-2", "ISO 8859-2")]).
        /// </param>
        /// <param name="DefaultOptionIndex">
        /// The index of the option that is selected by default. An out-of-range index will fall back to the first option
        /// (on true/false it will be false).
        /// </param>
        record PickerChoicesRequest(
            string ID,
            string Label,
            (string, string)[] Options,
            int DefaultOptionIndex);
    }
}
