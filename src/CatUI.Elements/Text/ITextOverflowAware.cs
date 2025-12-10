using CatUI.Data;
using CatUI.Data.Enums;

namespace CatUI.Elements.Text;

/// <summary>
/// An element that inherits this interface will be able to handle text overflow.
/// </summary>
public interface ITextOverflowAware
{
    /// <summary>
    /// Specifies the behavior of the text element when the text is too large to render in the given space.
    /// The actual behavior depends on each element. See <see cref="TextOverflowMode"/> for information
    /// about possible values. The default value is <see cref="TextOverflowMode.Ellipsis"/>.
    /// </summary>
    TextOverflowMode OverflowMode { get; set; }

    ObservableProperty<TextOverflowMode> OverflowModeProperty { get; }

    /// <summary>
    /// Specifies the string that will be appended at the end of a row if the text cannot be drawn completely
    /// (because it will overflow the element, for example). The default value is "\u2026" (horizontal ellipsis).
    /// </summary>
    string OverflowString { get; set; }

    ObservableProperty<string> OverflowStringProperty { get; }
}
