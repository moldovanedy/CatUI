namespace CatUI.Elements.Behaviors
{
    public interface IToggleable
    {
        /// <summary>
        /// Available for elements that can be toggled, like checkboxes, switches, radio buttons, etc. It means that
        /// the element is currently active/on/selected.
        /// </summary>
        public const string STATE_ACTIVE = "active";
    }
}
