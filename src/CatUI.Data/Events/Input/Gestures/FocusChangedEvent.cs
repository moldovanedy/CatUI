namespace CatUI.Data.Events.Input.Gestures
{
    /// <summary>
    /// Represents a focus state change in an element. <c>hasEnteredFocus</c> is true when the element entered
    /// (or got) focus, false when the element exited (or lost) focus.
    /// </summary>
    public delegate void FocusChangedEventHandler(object sender, bool hasEnteredFocus);
}
