using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input.Keyboard;
using CatUI.Data.Gestures;

namespace CatUI.Data.Managers;

public static class InputManager
{
    /// <summary>
    /// Represents all the currently bound shortcuts. You can add, remove, or update shortcuts from this dictionary.
    /// The key is the shortcut name, the value is a <see cref="Shortcut"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you use events for shortcuts instead of <see cref="IsShortcutCurrentlyTriggered"/>, try to limit the number
    /// of shortcuts to increase performance. It is generally preferred to use <see cref="IsShortcutCurrentlyTriggered"/>
    /// for increased performance (even if the API is lower-level, potentially leading to more complexity).
    /// </para>
    /// <para>
    /// By default, there are some shortcuts directly set by CatUI for critical actions (like backspace for text
    /// deletion, home, end, left arrow, or right arrow for navigation in a text field, etc.) or general-purpose
    /// actions (cut, copy, paste, etc.). If you remove any of the critical actions, some elements might no longer
    /// work properly.
    /// </para>
    /// </remarks>
    public static Dictionary<string, Shortcut> CurrentShortcuts { get; } = [];

    /// <summary>
    /// Represents a bitmap of all the pressed key modifiers (like "Ctrl", "Alt", etc.). Does not take into account
    /// whether the left key, the right key, or both keys were pressed (for modifiers that have 2 keys for the same
    /// function).
    /// </summary>
    /// <example>
    /// ((PressedKeyModifiers &amp; KeyModifiers.Shift) != 0) is true if one (or both) of the "Shift" keys is pressed,
    /// false otherwise.
    /// </example>
    public static KeyModifiers PressedKeyModifiers { get; private set; } = KeyModifiers.None;

    private static readonly Dictionary<PhysicalKey, bool> _pressedKeys = [];

    /// <summary>
    /// From the least significant bits: bit 0 is left "Shift", bit 1 is right "Shift", bit 2-3 for left/right "Ctrl",
    /// bit 4-5 for left/right "Alt", bit 6-7 for left/right "Super", bit 8 for "Caps Lock", bit 9 for "Num Lock".
    /// </summary>
    private static int _keyModifiersBitmap;

    /// <param name="key"></param>
    /// <returns>Returns true if the given key is currently pressed, false otherwise.</returns>
    public static bool IsKeyCurrentlyPressed(PhysicalKey key)
    {
        bool exists = _pressedKeys.TryGetValue(key, out bool result);
        return exists && result;
    }

    /// <param name="shortcutName">The name of the shortcut that you want to test.</param>
    /// <returns>
    /// Returns true if a shortcut with shortcutName exists and is currently triggered, false otherwise.
    /// </returns>
    public static bool IsShortcutCurrentlyTriggered(string shortcutName)
    {
        return CurrentShortcuts.TryGetValue(shortcutName, out Shortcut? result) && result.IsCurrentlyTriggered();
    }


    /// <summary>
    /// Will update the internal state of the pressed keys. Should only be called by UiDocument.
    /// </summary>
    /// <param name="e"></param>
    internal static void DocumentOnPhysicalKeyEvent(KeyEventArgs e)
    {
        _pressedKeys[e.Key] = e.Action != KeyAction.Released;

        if (!e.Key.IsModifierKey())
        {
            return;
        }

        int mask = 0;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case PhysicalKey.LeftShift:
                mask |= 1 << 0;
                break;
            case PhysicalKey.RightShift:
                mask |= 1 << 1;
                break;
            case PhysicalKey.LeftControl:
                mask |= 1 << 2;
                break;
            case PhysicalKey.RightControl:
                mask |= 1 << 3;
                break;
            case PhysicalKey.LeftAlt:
                mask |= 1 << 4;
                break;
            case PhysicalKey.RightAlt:
                mask |= 1 << 5;
                break;
            case PhysicalKey.LeftSuper:
                mask |= 1 << 6;
                break;
            case PhysicalKey.RightSuper:
                mask |= 1 << 7;
                break;
            case PhysicalKey.CapsLock:
                mask |= 1 << 8;
                break;
            case PhysicalKey.NumLock:
                mask |= 1 << 9;
                break;
            default:
#if DEBUG
                CatLogger.LogError($"Unexpected modifier key: {e.Key}.");
#endif
                return;
        }

        if (e.Action == KeyAction.Released)
        {
            _keyModifiersBitmap &= ~mask;
        }
        else
        {
            _keyModifiersBitmap |= mask;
        }

        UpdateModifierState(KeyModifiers.Shift, 0);
        UpdateModifierState(KeyModifiers.Control, 2);
        UpdateModifierState(KeyModifiers.Alt, 4);
        UpdateModifierState(KeyModifiers.Super, 6);
        UpdateModifierState(KeyModifiers.CapsLock, 7, 1);
        UpdateModifierState(KeyModifiers.NumLock, 8, 1);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateModifierState(KeyModifiers modifier, int offset, int bitCount = 2)
    {
        int mask = bitCount == 1 ? 1 << offset : 0b11 << offset;
        PressedKeyModifiers = (_keyModifiersBitmap & mask) == 0
            ? PressedKeyModifiers & ~modifier
            : PressedKeyModifiers | modifier;
    }
}
