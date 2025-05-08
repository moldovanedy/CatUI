namespace CatUI.Elements.Behaviors
{
    /// <summary>
    /// If an element implements this, it means that it this element is not relevant visually and any property 
    /// used for visuals will be ignored (but might still trigger a layout recalculation). Mostly used for control-flow
    /// elements.
    /// </summary>
    public interface INonVisualElement;
}
