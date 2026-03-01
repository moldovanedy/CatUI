using System.Collections.Generic;
using CatUI.Data.Enums;
using CatUI.Data.Managers;

namespace CatUI.Data.Gestures;

/// <summary>
/// A container class for all the default shortcut names that are used by the internal CatUI components.
/// </summary>
public static class DefaultShortcutNames
{
    /// <summary>
    /// Next focusable element enters focus (default is Tab).
    /// </summary>
    public const string KEYBOARD_NAVIGATION_NEXT = nameof(KEYBOARD_NAVIGATION_NEXT);

    /// <summary>
    /// Previous focusable element enters focus (default is Shift+Tab).
    /// </summary>
    public const string KEYBOARD_NAVIGATION_PREVIOUS = nameof(KEYBOARD_NAVIGATION_PREVIOUS);

    /// <summary>
    /// The currently focused element is selected (action is triggered) (default is Enter).
    /// </summary>
    public const string KEYBOARD_NAVIGATION_SELECT = nameof(KEYBOARD_NAVIGATION_SELECT);


    /// <summary>
    /// Deletes the next character from the caret (so the character towards the row end) (default is Delete).
    /// </summary>
    public const string TEXT_DELETE_FROM_END = nameof(TEXT_DELETE_FROM_END);

    /// <summary>
    /// Deletes the previous character from the caret (so the character towards the row beginning)
    /// (default is Backspace).
    /// </summary>
    public const string TEXT_DELETE_FROM_BEGINNING = nameof(TEXT_DELETE_FROM_BEGINNING);

    /// <summary>
    /// Moves the caret one position to the left (all languages) (default is Left Arrow).
    /// </summary>
    public const string TEXT_NAVIGATE_TO_LEFT = nameof(TEXT_NAVIGATE_TO_LEFT);

    /// <summary>
    /// Moves the caret one position to the right (all languages) (default is Right Arrow).
    /// </summary>
    public const string TEXT_NAVIGATE_TO_RIGHT = nameof(TEXT_NAVIGATE_TO_RIGHT);


    /// <summary>
    /// Moves the caret one word to the next word (right in most languages) (default is Ctrl + Right Arrow).
    /// </summary>
    public const string TEXT_NAVIGATE_TO_NEXT_WORD = nameof(TEXT_NAVIGATE_TO_NEXT_WORD);

    /// <summary>
    /// Moves the caret one word to the previous word (left in most languages) (default is Ctrl + Left Arrow).
    /// </summary>
    public const string TEXT_NAVIGATE_TO_PREVIOUS_WORD = nameof(TEXT_NAVIGATE_TO_PREVIOUS_WORD);

    /// <summary>
    /// Moves the caret to the row beginning (left in most languages) (default is Home).
    /// </summary>
    public const string TEXT_NAVIGATE_TO_ROW_BEGINNING = nameof(TEXT_NAVIGATE_TO_ROW_BEGINNING);

    /// <summary>
    /// Moves the caret to the row end (right in most languages) (default is End).
    /// </summary>
    public const string TEXT_NAVIGATE_TO_ROW_END = nameof(TEXT_NAVIGATE_TO_ROW_END);


    /// <summary>
    /// Removes the text and adds it to the clipboard.
    /// </summary>
    public const string TEXT_CUT = nameof(TEXT_CUT);

    /// <summary>
    /// Adds the text to the clipboard.
    /// </summary>
    public const string TEXT_COPY = nameof(TEXT_COPY);

    /// <summary>
    /// Adds the text from the clipboard.
    /// </summary>
    public const string TEXT_PASTE = nameof(TEXT_PASTE);
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
