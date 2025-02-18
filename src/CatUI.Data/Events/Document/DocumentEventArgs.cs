using System;

namespace CatUI.Data.Events.Document
{
    public class ChildLayoutChangedEventArgs : EventArgs
    {
        public int ChildIndex { get; }
        public LayoutChangeFlags ChangeFlags { get; }

        public ChildLayoutChangedEventArgs(int childIndex, LayoutChangeFlags changeFlags = LayoutChangeFlags.All)
        {
            ChildIndex = childIndex;
            ChangeFlags = changeFlags;
        }

        /// <summary>
        /// Represents the properties of an element that are changed. Use these hints inside Element.RecomputeLayout
        /// to only update the required info, therefore improving performance.
        /// </summary>
        [Flags]
        public enum LayoutChangeFlags
        {
            /// <summary>
            /// No change. This happens when the parent already took care of the children layout, like in containers.
            /// </summary>
            None = 0,

            /// <summary>
            /// The position is changed. If this is the only change, it means that a measure is not necessary, 
            /// so the change will be made really fast. This is an important feature.
            /// </summary>
            Position = 1,

            /// <summary>
            /// The width has changed. Measuring is necessary.
            /// </summary>
            Width = 2,

            /// <summary>
            /// The height has changed. Measuring is necessary.
            /// </summary>
            Height = 4,

            /// <summary>
            /// All the three properties has changed. Measuring is necessary.
            /// </summary>
            All = 7
        }
    }
}
