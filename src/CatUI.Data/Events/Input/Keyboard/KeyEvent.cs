using CatUI.Data.Enums;

namespace CatUI.Data.Events.Input.Keyboard;

public delegate void KeyEventHandler(object sender, KeyEventArgs e);

/// <summary>
/// Represents a key event on the user's keyboard. Unlike <see cref="CharTypedEventArgs"/>, this
/// event takes into account the physical keys, so this might be better if you don't want text input but rather
/// keyboard shortcuts or other special cases.
/// </summary>
/// <remarks>
/// Don't use this event for text input. Use <see cref="CharTypedEventArgs"/>.
/// </remarks>
public class KeyEventArgs : InputEventArgs
{
    /// <summary>
    /// The pressed key. This can also be a modifier key ("Ctrl", "Alt", etc.) when those are pressed or released.
    /// Already pressed modifiers appear in <see cref="Modifiers"/>. If only a modifier key is pressed, without an
    /// alphanumeric key (e.g. "A", "Z", "1"), this will be the modifier itself.
    /// </summary>
    public PhysicalKey Key { get; }

    /// <summary>
    /// The pressed key in US ANSI 104 keyboard layout. This is not translated to the user's keyboard, so it's
    /// generally better to use <see cref="Key"/>, except rare circumstances where this behavior is desired.
    /// Otherwise, it's the same as <see cref="Key"/>.
    /// </summary>
    public PhysicalKey RawKey { get; }

    /// <summary>
    /// The modifier keys that are currently held down.
    /// </summary>
    public KeyModifiers Modifiers { get; }

    /// <summary>
    /// Determines whether a key was pressed, released, or the event is repeated by the platform.
    /// </summary>
    public KeyAction Action { get; }

    /// <summary>
    /// If true, it means it uses the direct key events from the runtime platform for <see cref="Key"/>. If false,
    /// it means it tries to translate the key codes from <see cref="RawKey"/> using a workaround.
    /// <see cref="Key"/> will still work correctly for most cases; for edge cases, <see cref="Key"/> will be the
    /// same as <see cref="RawKey"/>.
    /// </summary>
    public bool IsUsingNativeTranslation { get; }

    public KeyEventArgs(KeyEventArgs other) :
        this(other.Key, other.RawKey, other.Modifiers, other.Action, other.IsUsingNativeTranslation)
    {
    }

    public KeyEventArgs(
        PhysicalKey key,
        PhysicalKey rawKey,
        KeyModifiers modifiers,
        KeyAction action,
        bool isUsingNativeTranslation = false)
    {
        Key = key;
        RawKey = rawKey;
        Modifiers = modifiers;
        Action = action;
        IsUsingNativeTranslation = isUsingNativeTranslation;
    }
}
