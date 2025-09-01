using System;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data.Exceptions;
using CatUI.Platform.CommonInterface;

namespace CatUI.Platform.NativeUI
{
    /// <summary>
    /// A platform-specific "open directory dialog". This can be used either for reading files from a directory,
    /// for writing into a directory, or for both (read-write).
    /// </summary>
    public class NativeOpenDirectoryDialog : NativeFileDialogBase
    {
        /// <summary>
        /// Controls whether the user can select multiple directories or not. This is generally ignored, as most
        /// platforms can't select multiple directories.
        /// </summary>
        public bool CanSelectMultipleItems { get; set; }

        /// <summary>
        /// If true, it will try to get write access for the selected directory. Most platforms will ignore this and
        /// give you read-write access anyway, but this ensures you get read-write access. The default value is true.
        /// </summary>
        public bool WantsWriteAccess { get; set; } = true;

        /// <summary>
        /// Opens the picker if possible on the runtime platform. Does not block the running thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to abort this operation.</param>
        /// <returns>
        /// Returns the selection in a <see cref="IFilePicker.OpenDirectoriesResponse"/> if the user successfully
        /// selected directories, null if: <br/> the user cancelled the operation<br/> -or- <br/> the
        /// <c>cancellationToken</c> was triggered<br/> -or- <br/> there was an unknown error (generally this will
        /// throw an exception, though).
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The runtime platform doesn't yet have an implementation for file pickers.
        /// </exception>
        /// <exception cref="InternalPlatformException">
        /// Something went wrong while the user was picking files.
        /// </exception>
        public async Task<IFilePicker.OpenDirectoriesResponse?> OpenAsync(CancellationToken? cancellationToken = null)
        {
            if (OS.FilePicker == null)
            {
                throw new PlatformNotSupportedException("The runtime platform does not support opening a file picker.");
            }

            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                return await OS.FilePicker.OpenDirectoriesAsync(
                    DialogTitle ?? string.Empty,
                    CanSelectMultipleItems,
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
        /// Returns the selection in a <see cref="IFilePicker.OpenDirectoriesResponse"/> if the user successfully
        /// selected directories, null if the user cancelled the operation or there was an unknown error (generally
        /// this will throw an exception, though).
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The runtime platform doesn't yet have an implementation for file pickers.
        /// </exception>
        /// <exception cref="InternalPlatformException">
        /// Something went wrong while the user was picking files.
        /// </exception>
        public IFilePicker.OpenDirectoriesResponse? Open()
        {
            return OpenAsync().GetAwaiter().GetResult();
        }
    }
}
