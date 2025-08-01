using CatUI.Elements.Behaviors;

namespace CatUI.Elements.DocumentManagers
{
    public class FocusManager
    {
        public IFocusable? CurrentlyFocusedElement { get; private set; }

        /// <summary>
        /// This is only relevant when the focused element loses focus without another element entering focus.
        /// </summary>
        private IFocusable? _lastFocusedElement;

        private UiDocument? _document;

        public FocusManager(UiDocument document)
        {
            _document = document;
        }

        internal void UserWantsNextFocusable()
        {
            if (CurrentlyFocusedElement == null)
            {
                if (_lastFocusedElement == null)
                {
                    //TODO: focus a default element
                }
                else if (_lastFocusedElement.NextFocusableElement == null)
                {
                    //TODO: guess the next focusable element (common case)
                }
                else
                {
                    ElementWantsFocus(_lastFocusedElement.NextFocusableElement);
                }
            }
            else if (CurrentlyFocusedElement.NextFocusableElement == null)
            {
                //TODO: guess the next focusable element (common case)
            }
            else
            {
                ElementWantsFocus(CurrentlyFocusedElement.NextFocusableElement);
            }
        }

        internal void UserWantsPreviousFocusable()
        {
            if (CurrentlyFocusedElement == null)
            {
                if (_lastFocusedElement == null)
                {
                    //TODO: focus a default element
                }
                else if (_lastFocusedElement.PreviousFocusableElement == null)
                {
                    //TODO: guess the previous focusable element (common case)
                }
                else
                {
                    ElementWantsFocus(_lastFocusedElement.PreviousFocusableElement);
                }
            }
            else if (CurrentlyFocusedElement.PreviousFocusableElement == null)
            {
                //TODO: guess the previously focusable element from the focus graph (common case)
            }
            else
            {
                ElementWantsFocus(CurrentlyFocusedElement.PreviousFocusableElement);
            }
        }

        internal void ElementWantsFocus(IFocusable element)
        {
            CurrentlyFocusedElement?.OnFocusStateChanged(false);
            element.OnFocusStateChanged(true);
            CurrentlyFocusedElement = element;
            _lastFocusedElement = null;
        }

        internal void ElementReleasesFocus(IFocusable element)
        {
            if (CurrentlyFocusedElement != element)
            {
                return;
            }

            element.OnFocusStateChanged(false);
            CurrentlyFocusedElement = null;
            _lastFocusedElement = element;
        }
    }
}
