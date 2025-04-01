using System;

namespace CatUI.Data.Events.Navigation
{
    public abstract class AbstractNavigationEventArgs : EventArgs
    {
        public string OldPath { get; protected set; } = string.Empty;
    }
}
