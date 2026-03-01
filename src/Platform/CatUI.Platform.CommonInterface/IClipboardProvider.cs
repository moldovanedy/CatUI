namespace CatUI.Platform.CommonInterface;

public interface IClipboardProvider
{
    /// <summary>
    /// Returns the string from the clipboard, or empty string if there is no clipboard content.
    /// </summary>
    /// <returns>The string from the clipboard, or empty string if there is no clipboard content.</returns>
    string GetClipboardContent();

    /// <summary>
    /// Sets the clipboard string.
    /// </summary>
    /// <param name="content">The string to set in the clipboard.</param>
    /// <returns>True if the operation was successful, false otherwise.</returns>
    bool SetClipboardContent(string content);
}
