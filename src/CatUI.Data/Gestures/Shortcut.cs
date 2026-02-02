using System.Collections.Generic;
using CatUI.Data.Enums;
using CatUI.Data.Managers;

namespace CatUI.Data.Gestures;

/// <summary>
/// A container class for all the default shortcut names that are used by the internal CatUI components.
/// </summary>
public static class DefaultShortcutNames
{
    public const string KEYBOARD_NAVIGATION_NEXT = nameof(KEYBOARD_NAVIGATION_NEXT);
    public const string KEYBOARD_NAVIGATION_PREVIOUS = nameof(KEYBOARD_NAVIGATION_PREVIOUS);
    public const string KEYBOARD_NAVIGATION_SELECT = nameof(KEYBOARD_NAVIGATION_SELECT);

    public const string TEXT_DELETE_FROM_END = nameof(TEXT_DELETE_FROM_END);
    public const string TEXT_DELETE_FROM_BEGINNING = nameof(TEXT_DELETE_FROM_BEGINNING);
    public const string TEXT_NAVIGATE_TO_LEFT = nameof(TEXT_NAVIGATE_TO_LEFT);
    public const string TEXT_NAVIGATE_TO_RIGHT = nameof(TEXT_NAVIGATE_TO_RIGHT);

    public const string TEXT_NAVIGATE_WORD_TO_LEFT = nameof(TEXT_NAVIGATE_WORD_TO_LEFT);
    public const string TEXT_NAVIGATE_WORD_TO_RIGHT = nameof(TEXT_NAVIGATE_WORD_TO_RIGHT);
    public const string TEXT_NAVIGATE_TO_ROW_LEFT = nameof(TEXT_NAVIGATE_TO_ROW_LEFT);
    public const string TEXT_NAVIGATE_TO_ROW_RIGHT = nameof(TEXT_NAVIGATE_TO_ROW_RIGHT);
}

/// <summary>
/// Describes the actual shortcut for an action. It has one or more <see cref="KeyCombination"/>s in a list.
/// Only one needs to be input for the shortcut to be triggered.
/// </summary>
public class Shortcut
{
    /// <summary>
    /// Describes an individual key combination. A shortcut has one or more of these combinations.
    /// </summary>
    /// <param name="Modifier">The modifiers (e.g., Ctrl, Shift) needed for the action.</param>
    /// <param name="Key">The actual physical key needed for the action.</param>
    public record KeyCombination(KeyModifiers Modifier, PhysicalKey Key);


    private readonly List<KeyCombination> _keyCombinations = [];

    public Shortcut(params KeyCombination[] keyCombinations)
    {
        _keyCombinations.AddRange(keyCombinations);
    }

    public Shortcut(IList<KeyCombination> keyCombinations)
    {
        _keyCombinations.AddRange(keyCombinations);
    }

    /// <returns>Returns a shallow clone of the internal key combinations list. Cache this value if used often.</returns>
    public List<KeyCombination> GetKeyCombinations()
    {
        return [.._keyCombinations];
    }

    /// <returns>
    /// Returns true if at least a key combination is currently active (pressed), false otherwise. Cache this value
    /// if used often.
    /// </returns>
    public bool IsCurrentlyTriggered()
    {
        foreach (KeyCombination keyCombination in _keyCombinations)
        {
            bool areModifiersOk =
                (keyCombination.Modifier == KeyModifiers.None && InputManager.PressedKeyModifiers == KeyModifiers.None)
             || (InputManager.PressedKeyModifiers & keyCombination.Modifier) != KeyModifiers.None;

            if (areModifiersOk && InputManager.IsKeyCurrentlyPressed(keyCombination.Key))
            {
                return true;
            }
        }

        return false;
    }

    /// <returns>Returns the key combinations list directly (no copying). For internal use only!</returns>
    internal List<KeyCombination> GetDirectKeyCombinations()
    {
        return _keyCombinations;
    }
}
