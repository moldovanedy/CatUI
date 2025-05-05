namespace CatUI.Windowing.Common
{
    /// <summary>
    /// Describes a window with a classic lifecycle, where the window object can be created, configured, opened and run.
    /// This is applicable on desktop platforms only.
    /// </summary>
    public interface IClassicLifecycle
    {
        /// <summary>
        /// Creates and displays a graphical window, taking into consideration the supported parameters, if any.
        /// </summary>
        public void Open();

        /// <summary>
        /// Starts listening for platform events, handles the rendering and the general window lifecycle. Will return
        /// only when the window was closed either by the user or programmatically and only if the invoked
        /// UiDocument.OnCloseRequested returns true.
        /// </summary>
        public void Run();
    }
}
