using CatUI.Data;
using CatUI.Data.Events.Input;
using CatUI.Data.Events.Input.Gestures;

namespace CatUI.Elements.Behaviors
{
    /// <summary>
    /// An element that implements this interface automatically takes part in the focus workflow, meaning that the
    /// user can focus this element by clicking on it or by using keyboard navigation. Also, the element can receive
    /// keyboard events like <see cref="Element.OnKeyEvent"/> and <see cref="Element.OnCharTyped"/>, plus it can
    /// be in the <see cref="STATE_FOCUSED"/> <see cref="Element.State"/>.
    /// </summary>
    /// <remarks>
    /// Implementation must invoke <see cref="FocusChangedEvent"/> inside <see cref="OnFocusStateChanged"/>.
    /// </remarks>
    public interface IFocusable
    {
        /// <summary>
        /// Available for elements that can be focused, so the user can input keys or simply select the element
        /// using keyboard navigation.
        /// </summary>
        public const string STATE_FOCUSED = "focused";

        /// <summary>
        /// If true (generally the default), it means the element can receive and grab focus. If false, it means the
        /// element will not take part in the focus workflow, but it can be focused programatically through
        /// <see cref="FocusableExtensions.GrabFocus"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the element is disabled or invisible (<see cref="Element.IsCurrentlyEnabled"/> or
        /// <see cref="Element.IsCurrentlyVisible"/> is false), then the element can't have focus, so it takes
        /// precedence over this property.
        /// </para>
        /// <para>
        /// This only affects the current element, not its descendants. Its descendants can still be focused even if
        /// this property is false.
        /// </para>
        /// </remarks>
        public bool IsFocusEnabled { get; set; }

        public ObservableProperty<bool> IsFocusEnabledProperty { get; }

        /// <summary>
        /// Fired when the focus of the element has changed.
        /// </summary>
        public event FocusChangedEventHandler? FocusChangedEvent;

        public FocusChangedEventHandler? OnFocusChanged { get; set; }

        public void FocusChanged(object sender, FocusChangedEventHandler e);

        /// <summary>
        /// If not null, the contained element will be focused next when navigating with the keyboard (using the Tab key
        /// on most systems). If null, the framework will automatically decide the next focusable element by following
        /// the document order.
        /// </summary>
        /// <remarks>
        /// It's generally recommended to leave this as null so CatUI can automatically focus elements using
        /// predictable patterns.
        /// </remarks>
        public IFocusable? NextFocusableElement { get; set; }

        /// <summary>
        /// If not null, the contained element will be focused next when navigating with the keyboard and triggering
        /// the previous focus (using the Shift+Tab key on most systems). If null, the framework will automatically
        /// decide the "previous" focusable element by following the document order.
        /// </summary>
        /// <remarks>
        /// It's generally recommended to leave this as null so CatUI can automatically focus elements using
        /// predictable patterns.
        /// </remarks>
        public IFocusable? PreviousFocusableElement { get; set; }

        /// <summary>
        /// For internal use only! The implementation must invoke the <see cref="FocusChangedEvent"/> inside this method,
        /// as this method gets called internally when the focus of this element changes. This should also change
        /// <see cref="Element.State"/> accordingly (use <see cref="SetFocusedStateUtility"/>
        /// for this).
        /// </summary>
        /// <param name="hasEnteredFocus">
        /// True when the element entered (or got) focus, false when the element exited (or lost) focus.
        /// </param>
        public void OnFocusStateChanged(bool hasEnteredFocus);

        /// <summary>
        /// A utility to be used in implementations of <see cref="OnFocusStateChanged"/> to change
        /// <see cref="Element.State"/> accordingly.
        /// </summary>
        /// <param name="isFocusedNow">
        /// If true, this will set the state to <see cref="STATE_FOCUSED"/>; if false, it will decide
        /// what's the best state for the element.
        /// </param>
        public void SetFocusedStateUtility(bool isFocusedNow)
        {
            if (this is not Element directElement)
            {
                return;
            }

            if (isFocusedNow)
            {
                directElement.State = STATE_FOCUSED;
            }
            else
            {
                InputPointer? pointer =
                    directElement.Document?
                        .GetPointerByDeviceType(InputPointer.InputDeviceType.Mouse);
                if (pointer == null)
                {
                    directElement.State = Element.STATE_NORMAL;
                    return;
                }

                if (Rect.IsPointInside(directElement.Bounds, pointer.AbsolutePosition))
                {
                    directElement.State = pointer.IsConsideredPressed ? Element.STATE_PRESSED : Element.STATE_HOVER;
                }
                else
                {
                    directElement.State = Element.STATE_NORMAL;
                }
            }
        }
    }

    public static class FocusableExtensions
    {
        /// <summary>
        /// Will make the given element to receive focus by "stealing" the focus of another element. This should be
        /// called only by the implementing element itself when it receives focus, but can also be called to explicitly
        /// give focus to an element.
        /// </summary>
        /// <param name="element"></param>
        public static void GrabFocus(this IFocusable element)
        {
            if (element is not Element directElement)
            {
                return;
            }

            directElement.Document?.FocusManager.ElementWantsFocus(element);
        }

        /// <summary>
        /// This will make the element to lose focus, but no other element will get focus unless by calling
        /// <see cref="GrabFocus"/>, either manually or automatically by a user action. You should generally avoid
        /// calling this function, as it might impose accessibility issues to users that rely on keyboard navigation.
        /// </summary>
        /// <param name="element"></param>
        public static void ReleaseFocus(this IFocusable element)
        {
            if (element is not Element directElement)
            {
                return;
            }

            directElement.Document?.FocusManager.ElementReleasesFocus(element);
        }
    }
}
