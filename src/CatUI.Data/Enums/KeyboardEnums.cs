using System;
using CatUI.Data.Events.Input.Keyboard;

namespace CatUI.Data.Enums
{
    public static class PhysicalKeyExtensions
    {
        /// <summary>
        /// Returns true if this is a modifier key (Shift, Ctrl, Alt, Super, Caps Lock, Num Lock).
        /// </summary>
        /// <param name="key">The key to verify</param>
        /// <returns></returns>
        public static bool IsModifierKey(this PhysicalKey key)
        {
            return key == PhysicalKey.LeftShift
                || key == PhysicalKey.RightShift
                || key == PhysicalKey.LeftControl
                || key == PhysicalKey.RightControl
                || key == PhysicalKey.LeftAlt
                || key == PhysicalKey.RightAlt
                || key == PhysicalKey.LeftSuper
                || key == PhysicalKey.RightSuper
                || key == PhysicalKey.CapsLock
                || key == PhysicalKey.NumLock;
        }

        /// <summary>
        /// Attempts to convert this key to a modifier. Returns <see cref="KeyModifiers.None"/> if this is not a modifier
        /// key.
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>
        /// The corresponding <see cref="KeyModifiers"/> or <see cref="KeyModifiers.None"/> if this is not a modifier
        /// key.
        /// </returns>
        public static KeyModifiers ToModifier(this PhysicalKey key)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (key)
            {
                case PhysicalKey.LeftShift:
                case PhysicalKey.RightShift:
                    return KeyModifiers.Shift;
                case PhysicalKey.LeftControl:
                case PhysicalKey.RightControl:
                    return KeyModifiers.Control;
                case PhysicalKey.LeftAlt:
                case PhysicalKey.RightAlt:
                    return KeyModifiers.Alt;
                case PhysicalKey.LeftSuper:
                case PhysicalKey.RightSuper:
                    return KeyModifiers.Super;
                case PhysicalKey.CapsLock:
                    return KeyModifiers.CapsLock;
                case PhysicalKey.NumLock:
                    return KeyModifiers.NumLock;
                default:
                    return KeyModifiers.None;
            }
        }
    }

    //NOTE: this is taken from GLFW as-is, except that Unknown is 0 instead of -1. Only some names
    // and descriptions were changed

    /// <summary>
    /// Represents a physical key on the keyboard. You should NOT use this to get text input, this is generally used
    /// for keyboard shortcuts. Always use <see cref="CharTypedEventArgs"/> for character input.
    /// </summary>
    public enum PhysicalKey
    {
        /// <summary>
        /// An unknown key.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The "space bar" key.
        /// </summary>
        Space = 32,

        /// <summary>
        /// The apostrophe key ("'").
        /// </summary>
        Apostrophe = 39 /* ' */,

        /// <summary>
        /// The comma key (",").
        /// </summary>
        Comma = 44 /* , */,

        /// <summary>
        /// The minus key ("-").
        /// </summary>
        Minus = 45 /* - */,

        /// <summary>
        /// The period key (".").
        /// </summary>
        Period = 46 /* . */,

        /// <summary>
        /// The slash key ("/").
        /// </summary>
        Slash = 47 /* / */,

        /// <summary>
        /// The "0" key.
        /// </summary>
        Digit0 = 48,

        /// <summary>
        /// The "1" key.
        /// </summary>
        Digit1 = 49,

        /// <summary>
        /// The "2" key.
        /// </summary>
        Digit2 = 50,

        /// <summary>
        /// The "3" key.
        /// </summary>
        Digit3 = 51,

        /// <summary>
        /// The "4" key.
        /// </summary>
        Digit4 = 52,

        /// <summary>
        /// The "5" key.
        /// </summary>
        Digit5 = 53,

        /// <summary>
        /// The "6" key.
        /// </summary>
        Digit6 = 54,

        /// <summary>
        /// The "7" key.
        /// </summary>
        Digit7 = 55,

        /// <summary>
        /// The "8" key.
        /// </summary>
        Digit8 = 56,

        /// <summary>
        /// The "9" key.
        /// </summary>
        Digit9 = 57,

        /// <summary>
        /// The semicolon key (";").
        /// </summary>
        Semicolon = 59 /* ; */,

        /// <summary>
        /// The equal key ("=").
        /// </summary>
        Equal = 61 /* = */,

        /// <summary>
        /// The "A" key.
        /// </summary>
        A = 65,

        /// <summary>
        /// The "B" key.
        /// </summary>
        B = 66,

        /// <summary>
        /// The "C" key.
        /// </summary>
        C = 67,

        /// <summary>
        /// The "D" key.
        /// </summary>
        D = 68,

        /// <summary>
        /// The "E" key.
        /// </summary>
        E = 69,

        /// <summary>
        /// The "F" key.
        /// </summary>
        F = 70,

        /// <summary>
        /// The "G" key.
        /// </summary>
        G = 71,

        /// <summary>
        /// The "H" key.
        /// </summary>
        H = 72,

        /// <summary>
        /// The "I" key.
        /// </summary>
        I = 73,

        /// <summary>
        /// The "J" key.
        /// </summary>
        J = 74,

        /// <summary>
        /// The "K" key.
        /// </summary>
        K = 75,

        /// <summary>
        /// The "L" key.
        /// </summary>
        L = 76,

        /// <summary>
        /// The "M" key.
        /// </summary>
        M = 77,

        /// <summary>
        /// The "N" key.
        /// </summary>
        N = 78,

        /// <summary>
        /// The "O" key.
        /// </summary>
        O = 79,

        /// <summary>
        /// The "P" key.
        /// </summary>
        P = 80,

        /// <summary>
        /// The "Q" key.
        /// </summary>
        Q = 81,

        /// <summary>
        /// The "R" key.
        /// </summary>
        R = 82,

        /// <summary>
        /// The "S" key.
        /// </summary>
        S = 83,

        /// <summary>
        /// The "T" key.
        /// </summary>
        T = 84,

        /// <summary>
        /// The "U" key.
        /// </summary>
        U = 85,

        /// <summary>
        /// The "V" key.
        /// </summary>
        V = 86,

        /// <summary>
        /// The "W" key.
        /// </summary>
        W = 87,

        /// <summary>
        /// The "X" key.
        /// </summary>
        X = 88,

        /// <summary>
        /// The "Y" key.
        /// </summary>
        Y = 89,

        /// <summary>
        /// The "Z" key.
        /// </summary>
        Z = 90,

        /// <summary>
        /// The left bracket ("[") key.
        /// </summary>
        LeftBracket = 91 /* [ */,

        /// <summary>
        /// The backslash ("\").
        /// </summary>
        Backslash = 92 /* \ */,

        /// <summary>
        /// The right bracket ("]") key.
        /// </summary>
        RightBracket = 93 /* ] */,

        /// <summary>
        /// The grave accent key ("`").
        /// </summary>
        GraveAccent = 96 /* ` */,

        /// <summary>
        /// The "Escape" key.
        /// </summary>
        Escape = 256,

        /// <summary>
        /// The "Enter" key.
        /// </summary>
        Enter = 257,

        /// <summary>
        /// The "Tab" key.
        /// </summary>
        Tab = 258,

        /// <summary>
        /// The "Backspace" key.
        /// </summary>
        Backspace = 259,

        /// <summary>
        /// The "Insert" key.
        /// </summary>
        Insert = 260,

        /// <summary>
        /// The "Delete" key.
        /// </summary>
        Delete = 261,

        /// <summary>
        /// The "right" arrow key.
        /// </summary>
        RightArrow = 262,

        /// <summary>
        /// The "left" arrow key.
        /// </summary>
        LeftArrow = 263,

        /// <summary>
        /// The "down" arrow key.
        /// </summary>
        DownArrow = 264,

        /// <summary>
        /// The "up" arrow key.
        /// </summary>
        UpArrow = 265,

        /// <summary>
        /// The "page up" key.
        /// </summary>
        PageUp = 266,

        /// <summary>
        /// The "page down" key.
        /// </summary>
        PageDown = 267,

        /// <summary>
        /// The "Home" key.
        /// </summary>
        Home = 268,

        /// <summary>
        /// The "End" key.
        /// </summary>
        End = 269,

        /// <summary>
        /// The "caps lock" key.
        /// </summary>
        CapsLock = 280,

        /// <summary>
        /// The "scroll lock" key.
        /// </summary>
        ScrollLock = 281,

        /// <summary>
        /// The "num lock" key.
        /// </summary>
        NumLock = 282,

        /// <summary>
        /// The "print screen" key.
        /// </summary>
        PrintScreen = 283,

        /// <summary>
        /// The "Pause" key.
        /// </summary>
        Pause = 284,

        /// <summary>
        /// The "F1" key.
        /// </summary>
        F1 = 290,

        /// <summary>
        /// The "F2" key.
        /// </summary>
        F2 = 291,

        /// <summary>
        /// The "F3" key.
        /// </summary>
        F3 = 292,

        /// <summary>
        /// The "F4" key.
        /// </summary>
        F4 = 293,

        /// <summary>
        /// The "F5" key.
        /// </summary>
        F5 = 294,

        /// <summary>
        /// The "F6" key.
        /// </summary>
        F6 = 295,

        /// <summary>
        /// The "F7" key.
        /// </summary>
        F7 = 296,

        /// <summary>
        /// The "F8" key.
        /// </summary>
        F8 = 297,

        /// <summary>
        /// The "F9" key.
        /// </summary>
        F9 = 298,

        /// <summary>
        /// The "F10" key.
        /// </summary>
        F10 = 299,

        /// <summary>
        /// The "F11" key.
        /// </summary>
        F11 = 300,

        /// <summary>
        /// The "F12" key.
        /// </summary>
        F12 = 301,

        /// <summary>
        /// The "F13" key.
        /// </summary>
        F13 = 302,

        /// <summary>
        /// The "F14" key.
        /// </summary>
        F14 = 303,

        /// <summary>
        /// The "F15" key.
        /// </summary>
        F15 = 304,

        /// <summary>
        /// The "F16" key.
        /// </summary>
        F16 = 305,

        /// <summary>
        /// The "F17" key.
        /// </summary>
        F17 = 306,

        /// <summary>
        /// The "F18" key.
        /// </summary>
        F18 = 307,

        /// <summary>
        /// The "F19" key.
        /// </summary>
        F19 = 308,

        /// <summary>
        /// The "F20" key.
        /// </summary>
        F20 = 309,

        /// <summary>
        /// The "F21" key.
        /// </summary>
        F21 = 310,

        /// <summary>
        /// The "F22" key.
        /// </summary>
        F22 = 311,

        /// <summary>
        /// The "F23" key.
        /// </summary>
        F23 = 312,

        /// <summary>
        /// The "F24" key.
        /// </summary>
        F24 = 313,

        /// <summary>
        /// The "F25" key.
        /// </summary>
        F25 = 314,

        /// <summary>
        /// The "0" key on the key pad.
        /// </summary>
        KeyPad0 = 320,

        /// <summary>
        /// The "1" key on the key pad.
        /// </summary>
        KeyPad1 = 321,

        /// <summary>
        /// The "2" key on the key pad.
        /// </summary>
        KeyPad2 = 322,

        /// <summary>
        /// The "3" key on the key pad.
        /// </summary>
        KeyPad3 = 323,

        /// <summary>
        /// The "4" key on the key pad.
        /// </summary>
        KeyPad4 = 324,

        /// <summary>
        /// The "5" key on the key pad.
        /// </summary>
        KeyPad5 = 325,

        /// <summary>
        /// The "6" key on the key pad.
        /// </summary>
        KeyPad6 = 326,

        /// <summary>
        /// The "7" key on the key pad.
        /// </summary>
        KeyPad7 = 327,

        /// <summary>
        /// The "8" key on the key pad.
        /// </summary>
        KeyPad8 = 328,

        /// <summary>
        /// The "9" key on the key pad.
        /// </summary>
        KeyPad9 = 329,

        /// <summary>
        /// The "decimal" key on the key pad.
        /// </summary>
        KeyPadDecimal = 330,

        /// <summary>
        /// The "divide" key on the key pad.
        /// </summary>
        KeyPadDivide = 331,

        /// <summary>
        /// The "multiply" key on the key pad.
        /// </summary>
        KeyPadMultiply = 332,

        /// <summary>
        /// The "subtract" key on the key pad.
        /// </summary>
        KeyPadSubtract = 333,

        /// <summary>
        /// The "add" key on the key pad.
        /// </summary>
        KeyPadAdd = 334,

        /// <summary>
        /// The "enter" key on the key pad.
        /// </summary>
        KeyPadEnter = 335,

        /// <summary>
        /// The "equal" key on the key pad.
        /// </summary>
        KeyPadEqual = 336,

        /// <summary>
        /// The left "Shift" key.
        /// </summary>
        LeftShift = 340,

        /// <summary>
        /// The left "Ctrl" key.
        /// </summary>
        LeftControl = 341,

        /// <summary>
        /// The left "Alt" key.
        /// </summary>
        LeftAlt = 342,

        /// <summary>
        /// The left "Super" key ("Win" key or "Command" on macOS).
        /// </summary>
        LeftSuper = 343,

        /// <summary>
        /// The right "Shift" key.
        /// </summary>
        RightShift = 344,

        /// <summary>
        /// The right "Ctrl" key.
        /// </summary>
        RightControl = 345,

        /// <summary>
        /// The right "Alt" key ("AltGr" on some keyboards).
        /// </summary>
        RightAlt = 346,

        /// <summary>
        /// The right "Super" key ("Win" key or "Command" on macOS).
        /// </summary>
        RightSuper = 347,

        /// <summary>
        /// The "Menu" key.
        /// </summary>
        Menu = 348,

        /// <summary>
        /// The last valid key in this enum.
        /// </summary>
        LastKey = Menu
    }


    public static class KeyModifiersExtensions
    {
        /// <summary>
        /// Tries to convert the modifier to a <see cref="PhysicalKey"/>. Will return <see cref="PhysicalKey.Unknown"/>
        /// if the modifier is <see cref="KeyModifiers.None"/>.
        /// </summary>
        /// <param name="modifiers">The modifier to convert.</param>
        /// <param name="useLeftKey">
        /// Determines whether to use the left key or the left key if there is such distinction (relevant only for
        /// <see cref="KeyModifiers.Shift"/>, <see cref="KeyModifiers.Control"/>, <see cref="KeyModifiers.Alt"/>, and
        /// <see cref="KeyModifiers.Super"/>).
        /// </param>
        /// <returns>The corresponding <see cref="PhysicalKey"/>.</returns>
        public static PhysicalKey ToPhysicalKey(this KeyModifiers modifiers, bool useLeftKey = true)
        {
            switch (modifiers)
            {
                case KeyModifiers.Shift:
                    return useLeftKey ? PhysicalKey.LeftShift : PhysicalKey.RightShift;
                case KeyModifiers.Control:
                    return useLeftKey ? PhysicalKey.LeftControl : PhysicalKey.RightControl;
                case KeyModifiers.Alt:
                    return useLeftKey ? PhysicalKey.LeftAlt : PhysicalKey.RightAlt;
                case KeyModifiers.Super:
                    return useLeftKey ? PhysicalKey.LeftSuper : PhysicalKey.RightSuper;
                case KeyModifiers.CapsLock:
                    return PhysicalKey.CapsLock;
                case KeyModifiers.NumLock:
                    return PhysicalKey.NumLock;
                case KeyModifiers.None:
                default:
                    return PhysicalKey.Unknown;
            }
        }
    }

    /// <summary>
    /// Key modifiers, such as Shift or CTRL.
    /// </summary>
    [Flags]
    public enum KeyModifiers
    {
        /// <summary>
        /// No modifiers are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// If one or more "Shift" keys were held down.
        /// </summary>
        Shift = 0x01,

        /// <summary>
        /// If one or more "Control" keys were held down.
        /// </summary>
        Control = 0x02,

        /// <summary>
        /// If one or more "Alt" keys were held down.
        /// </summary>
        Alt = 0x04,

        /// <summary>
        /// If one or more "Super" ("Win" on most keyboards, "Command" on macOS) keys were held down.
        /// </summary>
        Super = 0x08,

        /// <summary>
        /// If "Caps Lock" is enabled.
        /// </summary>
        CapsLock = 0x10,

        /// <summary>
        /// If "Num Lock" is enabled.
        /// </summary>
        NumLock = 0x20
    }

    /// <summary>
    /// Determines whether a key was pressed, released, or the event is repeated by the platform.
    /// </summary>
    public enum KeyAction
    {
        Released = 0,
        Pressed = 1,

        /// <summary>
        /// This means the key is held down, so the platform sends the same event repeatedly. The frequency of the
        /// repeating events is platform-specific, generally depending on the user settings on the runtime platform. 
        /// </summary>
        Repeat = 2
    }
}
