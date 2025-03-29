using CatUI.Data.Events.Input.Pointer;

namespace CatUI.Data.Events.Input
{
    /// <summary>
    /// Specifies that the event can be cancelled by the user. For example, a <see cref="PointerUpEventArgs"/> can be
    /// cancelled if the user put the pointer down, then tried to cancel it by dragging it out of the hit zone while down.
    /// </summary>
    public interface ICancellableInputEvent
    {
        /// <summary>
        /// True if the input was cancelled by the user. For example, a <see cref="PointerUpEventArgs"/> can be cancelled
        /// if the user put the pointer down, then tried to cancel it by dragging it out of the hit zone while down.
        /// </summary>
        public bool WasCancelled { get; }
    }
}
