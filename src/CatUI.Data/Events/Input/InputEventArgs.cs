using System;

namespace CatUI.Data.Events.Input
{
    public abstract class InputEventArgs : EventArgs
    {
        public bool IsPropagationStopped { get; private set; }

        public void StopPropagation()
        {
            IsPropagationStopped = true;
        }
    }
}
