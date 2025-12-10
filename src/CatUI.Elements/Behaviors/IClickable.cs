using CatUI.Data;
using CatUI.Data.Events.Input.Gestures;

namespace CatUI.Elements.Behaviors;

public interface IClickable
{
    /// <summary>
    /// If true, it means the user can cancel the click event by dragging the pointer (while it is still down) outside
    /// the element hit zone or any other common platform click cancelling method. Most of the applications implement
    /// this behavior, so you should generally let it as the default value, true.
    /// </summary>
    /// <remarks>
    /// If false, it means that even if the user tried to cancel the click (for example, dragging the mouse or finger
    /// outside the element hit zone), the event will still fire, which is generally bad UX. Set this to false only
    /// if your use case really needs it.
    /// </remarks>
    bool CanUserCancelClick { get; set; }

    ObservableProperty<bool> CanUserCancelClickProperty { get; }

    /// <summary>
    /// Fired when the user clicks on the element. For differences between Click and PointerDown, see the documentation
    /// for <see cref="ClickEventArgs"/>.
    /// </summary>
    event ClickEventHandler? ClickEvent;

    ClickEventHandler? OnClick { get; set; }

    /// <summary>
    /// Will simulate a click programatically. This will fire <see cref="ClickEvent"/> on this element.
    /// The event will have <see cref="ClickEventArgs"/> with the <see cref="ClickEventArgs.Position"/> set to 0, while
    /// <see cref="ClickEventArgs.AbsolutePosition"/> will be the top-left point of the element.
    /// </summary>
    void Click();

    /// <summary>
    /// This will be fired by the document when the element is focused and the user triggered the "select" action
    /// (usually by pressing Enter on the keyboard). Normally this should automatically trigger the
    /// <see cref="ClickEvent"/>, but some elements like text fields might perform a different action.
    /// Don't call this in UI code, as it will be called by the document!
    /// </summary>
    void FocusedSelectActionTriggered();
}
