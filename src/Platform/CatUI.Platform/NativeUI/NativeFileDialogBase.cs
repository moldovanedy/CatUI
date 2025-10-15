using CatUI.Data;
using CatUI.Platform.CommonInterface;

namespace CatUI.Platform.NativeUI;

public abstract class NativeFileDialogBase
{
    /// <summary>
    /// The title of the dialog. This should be supported on all platforms.
    /// </summary>
    public string? DialogTitle { get; set; }

    /// <summary>
    /// The initial location the picker will open in. This might get ignored on some dialogs.
    /// </summary>
    public FilePath? InitialLocation { get; set; }

    /// <summary>
    /// If supported by the platform, sets a custom label for the "submit" button. Most platforms ignore this.
    /// </summary>
    public string? CustomSubmitButtonText { get; set; }

    /// <summary>
    /// The platform-specific window ID for which the picker will be a modal. This is different for each platform,
    /// consult the manual for more info.
    /// </summary>
    public object? ParentWindowIdentifier { get; set; }

    /// <summary>
    /// If supported by the platform, uses additional "options" so the user can further customize what your
    /// application should do to the file they selected. See <see cref="IFilePicker.PickerChoicesRequest"/> for
    /// more info.
    /// </summary>
    /// <remarks>
    /// Most platforms ignore this. As of now, only Linux supports this (Windows support is coming soon), but other
    /// platforms might never support this.
    /// </remarks>
    public IFilePicker.PickerChoicesRequest[]? PickerChoices { get; set; }
}
