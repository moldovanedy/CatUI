using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CatUI.Elements
{
    /// <summary>
    /// Provides the functions that are called for styling. Remember that functions are called from the root element
    /// down to the actual element, so you can override what other higher elements did; however, you should try to
    /// limit the overriding as much as possible, as it might make the code more confusing.
    /// </summary>
    public class ThemeDefinition : INotifyPropertyChanged
    {
        /// <summary>
        /// <para>
        /// It is called:
        /// <list type="bullet">
        /// <item>
        /// on the element (and all its descendants) where the theme override was applied when that theme override
        /// was set or updated (either for a certain element type or for a style class)
        /// </item>
        /// <item>
        /// on an element and all its descendants when it enters the document
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The only argument is the element itself. This is used for the general styling, where most of the styling
        /// properties will be set.
        /// </para>
        /// </summary>
        public Action<Element>? OnThemeChanged
        {
            get => _onThemeChanged;
            set
            {
                _onThemeChanged = value;
                NotifyPropertyChanged();
            }
        }

        private Action<Element>? _onThemeChanged;

        /// <summary>
        /// It is called for the element that changed its state (NOT for descendants) whenever it changed its state.
        /// The first argument is the element itself, the second argument is its new state.
        /// </summary>
        public Action<Element, string?>? OnStateChanged
        {
            get => _onStateChanged;
            set
            {
                _onStateChanged = value;
                NotifyPropertyChanged();
            }
        }

        private Action<Element, string?>? _onStateChanged;

        /// <summary>
        /// It is called for the element that changed its state (NOT for descendants) whenever it modified its
        /// pseudo-classes (added or removed). The second argument is the pseudo-classes list, in order from the
        /// lowest priority to the highest one, so you should apply the changes from the start of the list to the end,
        /// like you normally would.
        /// </summary>
        public Action<Element, ReadOnlyCollection<string>>? OnPseudoClassesChanged
        {
            get => _onPseudoClassesChanged;
            set
            {
                _onPseudoClassesChanged = value;
                NotifyPropertyChanged();
            }
        }

        private Action<Element, ReadOnlyCollection<string>>? _onPseudoClassesChanged;

        internal Type? ElementType { get; set; }
        internal string? StyleClass { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ThemeDefinition() { }

        /// <summary>
        /// Creates a new theme definition with the specified callbacks.
        /// </summary>
        /// <param name="onThemeChanged">See <see cref="OnThemeChanged"/>.</param>
        /// <param name="onStateChanged"></param>
        /// <param name="onPseudoClassesChanged">See <see cref="OnPseudoClassesChanged"/>.</param>
        public ThemeDefinition(
            Action<Element>? onThemeChanged = null,
            Action<Element, string?>? onStateChanged = null,
            Action<Element, ReadOnlyCollection<string>>? onPseudoClassesChanged = null)
        {
            OnThemeChanged = onThemeChanged;
            OnStateChanged = onStateChanged;
            OnPseudoClassesChanged = onPseudoClassesChanged;
        }

        internal void InvokeOnThemeChanged(Element element)
        {
            OnThemeChanged?.Invoke(element);
        }

        internal void InvokeOnStateChanged(Element element, string? state)
        {
            OnStateChanged?.Invoke(element, state);
        }

        internal void InvokeOnPseudoClassesChanged(Element element)
        {
            List<string> reversedList = new(element.InternalPseudoClasses.Count);
            for (int i = element.InternalPseudoClasses.Count - 1; i >= 0; i--)
            {
                reversedList.Add(element.InternalPseudoClasses[i]);
            }

            OnPseudoClassesChanged?.Invoke(element, reversedList.AsReadOnly());
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
