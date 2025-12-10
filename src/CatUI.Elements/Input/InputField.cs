namespace CatUI.Elements.Input;

/// <summary>
/// An abstract element that can handle user input, like text, numbers, etc.
/// </summary>
public abstract class InputField : Element
{
    /// <summary>
    /// This is the element that will contain the actual input, for example, a label with all the user-provided text in
    /// a text field, a label with the number of a numerical field, etc.
    /// </summary>
    public abstract Element ActualValueElement { get; set; }

    public abstract string GetInputTextualRepresentation();

    public InputField() { }
    public InputField(InputField other) : base(other) { }
}
