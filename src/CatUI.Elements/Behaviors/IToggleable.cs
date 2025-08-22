namespace CatUI.Elements.Behaviors
{
    /// <summary>
    /// An element that implements this interface can be toggled on/off, also it can be in the <see cref="STATE_ACTIVE"/>
    /// <see cref="Element.State"/>.
    /// </summary>
    public interface IToggleable
    {
        /// <summary>
        /// Available for elements that can be toggled, like checkboxes, switches, radio buttons, etc. It means that
        /// the element is currently active/on/selected.
        /// </summary>
        const string STATE_ACTIVE = "active";

        /// <summary>
        /// Available for elements that can be toggled, like checkboxes, switches, radio buttons, etc. It means that
        /// the element is currently active/on/selected. Has a priority of 50.
        /// </summary>
        const string PSEUDO_CLASS_ACTIVE = "active";
    }
}
