using System;

namespace CatUI.Windowing.Common
{
    public delegate void ResizedEventHandler(object sender, ResizedEventArgs e);

    public class ResizedEventArgs : EventArgs
    {
        public ResizedEventArgs(int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            OldWidth = oldWidth;
            OldHeight = oldHeight;
            NewWidth = newWidth;
            NewHeight = newHeight;
        }

        /// <summary>
        /// The new width of the window.
        /// </summary>
        public int NewWidth { get; }

        /// <summary>
        /// The new height of the window.
        /// </summary>
        public int NewHeight { get; }

        /// <summary>
        /// The old width of the window.
        /// </summary>
        public int OldWidth { get; }

        /// <summary>
        /// The old height of the window.
        /// </summary>
        public int OldHeight { get; }
    }
}
