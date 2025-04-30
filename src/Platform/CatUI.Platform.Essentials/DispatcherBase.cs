using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatUI.Platform.Essentials
{
    /// <summary>
    /// The base for all dispatchers. All the dispatchers work the same, it's just that the actual implementation is
    /// platform-specific, and you need to use one of the already created dispatchers (or create your own if you
    /// want to provide support for a platform not officially supported by CatUI).
    /// </summary>
    public abstract class DispatcherBase
    {
        private readonly List<Action> _actions = new();

        /// <summary>
        /// The given action will be called on the UI thread regardless of what thread this method is called on
        /// so that all the operations can be executed in a thread-safe way, so you avoid any undefined behavior
        /// when you set properties of the elements. The actions set here will be called later (deferred), so there
        /// might be a delay of several milliseconds between when this method is called and when the action
        /// is actually invoked.
        /// </summary>
        /// <remarks>
        /// This should ALWAYS be called when you want to interact with the elements like setting properties
        /// on secondary threads or functions that are async and return a <see cref="Task"/> so they will be awaited.
        /// It is NOT necessary to use it when making element callbacks as async or for methods like
        /// Element.PointerEnter, Element.EnterDocument etc.
        /// </remarks>
        /// <param name="action">The action that you want to call on the UI thread.</param>
        public void InvokeOnUiThread(Action action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// Call this on the UI thread in the windowing code.
        /// </summary>
        protected void CallOnUIThread()
        {
            _actions.ForEach(a => a());
            _actions.Clear();
        }
    }
}
