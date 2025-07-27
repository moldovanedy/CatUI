namespace CatUI.Data.Events.Input.Keyboard
{
    public delegate void CharTypedEventHandler(object sender, CharTypedEventArgs e);

    /// <summary>
    /// Represents a text character input. This is different from <see cref="KeyEventArgs"/>, as that event
    /// handles physical keyboard keys, while this one handles actual text input.
    /// </summary>
    public class CharTypedEventArgs : InputEventArgs
    {
        /// <summary>
        /// The typed character.
        /// </summary>
        public char Character { get; }

        public CharTypedEventArgs(CharTypedEventArgs other) :
            this(other.Character)
        {
        }

        public CharTypedEventArgs(char character)
        {
            Character = character;
        }
    }
}
