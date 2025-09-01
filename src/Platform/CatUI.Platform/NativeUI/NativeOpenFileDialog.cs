using System;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data.Exceptions;
using CatUI.Platform.CommonInterface;

namespace CatUI.Platform.NativeUI
{
    /// <summary>
    /// A platform-specific "open file dialog".
    /// </summary>
    public class NativeOpenFileDialog : NativeFileDialogBase
    {
        /// <summary>
        /// Controls whether the user can select multiple files or not.
        /// </summary>
        public bool CanSelectMultipleItems { get; set; }

        /// <summary>
        /// The filter pattern to apply to restrict the file types the user is allowed to choose. The runtime
        /// platform might ignore this, so you need to handle the case where the files are in an unexpected format.
        /// </summary>
        public IFilePicker.FileFiltersArgument? FilterPattern { get; set; }

        /// <summary>
        /// Opens the picker if possible on the runtime platform. Does not block the running thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to abort this operation.</param>
        /// <returns>
        /// Returns the selection in a <see cref="IFilePicker.OpenFilesResponse"/> if the user successfully selected
        /// files, null if: <br/> the user cancelled the operation<br/> -or- <br/> the <c>cancellationToken</c> was
        /// triggered<br/> -or- <br/> there was an unknown error (generally this will throw an exception, though).
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The runtime platform doesn't yet have an implementation for file pickers.
        /// </exception>
        /// <exception cref="InternalPlatformException">
        /// Something went wrong while the user was picking files.
        /// </exception>
        public async Task<IFilePicker.OpenFilesResponse?> OpenAsync(CancellationToken? cancellationToken = null)
        {
            if (OS.FilePicker == null)
            {
                throw new PlatformNotSupportedException("The runtime platform does not support opening a file picker.");
            }

            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                return await OS.FilePicker.OpenFilesAsync(
                    DialogTitle ?? string.Empty,
                    CanSelectMultipleItems,
                    FilterPattern,
                    InitialLocation,
                    CustomSubmitButtonText,
                    PickerChoices,
                    ParentWindowIdentifier);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        /// <summary>
        /// Synchronously opens the picker if possible on the runtime platform. Blocks the calling thread until the user
        /// picks a file or cancels the operation.
        /// </summary>
        /// <returns>
        /// Returns the selection in a <see cref="IFilePicker.OpenFilesResponse"/> if the user successfully selected
        /// files, null if the user cancelled the operation or there was an unknown error (generally this will throw
        /// an exception, though).
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The runtime platform doesn't yet have an implementation for file pickers.
        /// </exception>
        /// <exception cref="InternalPlatformException">
        /// Something went wrong while the user was picking files.
        /// </exception>
        public IFilePicker.OpenFilesResponse? Open()
        {
            return OpenAsync().GetAwaiter().GetResult();
        }
    }
}
