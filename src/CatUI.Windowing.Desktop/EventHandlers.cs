using System;

namespace CatUI.Windowing.Desktop
{
    /// <summary>
    /// The sender is the window and the arguments describe the changes.
    /// </summary>
    public delegate void WindowModeChangedEventHandler(object sender, WindowModeChangedEventArgs e);

    public class WindowModeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new mode in which the window just entered.
        /// </summary>
        public DesktopWindow.WindowMode NewMode { get; }

        /// <summary>
        /// The old window mode.
        /// </summary>
        public DesktopWindow.WindowMode OldMode { get; }

        public WindowModeChangedEventArgs(DesktopWindow.WindowMode newMode, DesktopWindow.WindowMode oldMode)
        {
            NewMode = newMode;
            OldMode = oldMode;
        }
    }
}
