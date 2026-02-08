using System;
using CatUI.Data;
using CatUI.Elements.Behaviors;

namespace CatUI.Elements.DocumentManagers;

public class FocusManager
{
    public IFocusable? CurrentlyFocusedElement { get; private set; }

    /// <summary>
    /// This is only relevant when the focused element loses focus without another element entering focus.
    /// </summary>
    private IFocusable? _lastFocusedElement;

    private readonly UiDocument _document;

    public FocusManager(UiDocument document)
    {
        _document = document;
    }

    internal void UserWantsNextFocusable()
    {
        IFocusable? elementToFocus;
        if (CurrentlyFocusedElement == null)
        {
            //focus the first available element
            if (_lastFocusedElement == null)
            {
                elementToFocus = GetAppropriateFocusable(null);
            }
            //guess the next focusable element (common case)
            else if (_lastFocusedElement.NextFocusableElement == null)
            {
                elementToFocus =
                    GetAppropriateFocusable(_lastFocusedElement as Element)
                    //this generally means we no longer have any elements, so we start all over again from the
                    //first one
                 ?? GetAppropriateFocusable(null, true);
            }
            else
            {
                elementToFocus = _lastFocusedElement.NextFocusableElement;
            }
        }
        //guess the next focusable element (common case)
        else if (CurrentlyFocusedElement.NextFocusableElement == null)
        {
            elementToFocus =
                GetAppropriateFocusable(CurrentlyFocusedElement as Element)
                //this generally means we no longer have any elements, so we start all over again from the first one
             ?? GetAppropriateFocusable(null, true);
        }
        else
        {
            elementToFocus = CurrentlyFocusedElement.NextFocusableElement;
        }

        if (elementToFocus != null && elementToFocus.IsFocusEnabled)
        {
            ElementWantsFocus(elementToFocus);
        }
    }

    internal void UserWantsPreviousFocusable()
    {
        IFocusable? elementToFocus;
        if (CurrentlyFocusedElement == null)
        {
            //focus the first available element
            if (_lastFocusedElement == null)
            {
                elementToFocus = GetAppropriateFocusable(null, true);
            }
            //guess the previous focusable element (common case)
            else if (_lastFocusedElement.PreviousFocusableElement == null)
            {
                elementToFocus = GetAppropriateFocusable(_lastFocusedElement as Element, true);
            }
            else
            {
                elementToFocus = _lastFocusedElement.PreviousFocusableElement;
            }
        }
        //guess the previously focusable element from the focus graph (common case)
        else if (CurrentlyFocusedElement.PreviousFocusableElement == null)
        {
            elementToFocus = GetAppropriateFocusable(CurrentlyFocusedElement as Element, true);
        }
        else
        {
            elementToFocus = CurrentlyFocusedElement.PreviousFocusableElement;
        }

        if (elementToFocus != null && elementToFocus.IsFocusEnabled)
        {
            ElementWantsFocus(elementToFocus);
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


    private IFocusable? GetAppropriateFocusable(Element? previousFocused, bool wantsPreviousFocusable = false)
    {
        if (_document.Root == null)
        {
            return null;
        }

        if (previousFocused == null && _document.Root is IFocusable focusableRoot)
        {
            return focusableRoot;
        }

        previousFocused ??= _document.Root;
        //first, search inside the element
        IFocusable? result = GetFocusableForParent(previousFocused, wantsPreviousFocusable: wantsPreviousFocusable);
        if (result != null)
        {
            return result;
        }

        //then, search in siblings, then "uncles", etc. :)
        Element? currentlySearched = previousFocused.GetParent();
        while (currentlySearched != null)
        {
            result = GetFocusableForParent(
                currentlySearched,
                previousFocused,
                wantsPreviousFocusable);
            if (result != null)
            {
                return result;
            }

            previousFocused = currentlySearched;
            currentlySearched = currentlySearched.GetParent();
        }

        return null;
    }

    private static IFocusable? GetFocusableForParent(
        Element parent,
        Element? previousFocused = null,
        bool wantsPreviousFocusable = false)
    {
        if (!parent.IsCurrentlyEnabled)
        {
            return null;
        }

        //TODO: adjust for RTL layouts by inverting X axis
        Element? previousBestFit = previousFocused;
        if (parent.Children.Count == 0)
        {
            return null;
        }

        //this is to ensure we avoid infinite loops (shouldn't happen, but still): if the counter exceeds the
        //number of children of this parent, we just return null
        int watchDogCounter = 0;

        while (true)
        {
            //we start with a "dummy" element, the worst element
            Element bestFit = new()
            {
                Bounds = new Rect(
                    wantsPreviousFocusable ? float.MinValue : float.MaxValue,
                    wantsPreviousFocusable ? float.MinValue : float.MaxValue,
                    10,
                    10)
            };

            //TODO: try to find more fits in this loop to avoid O(n^2)
            foreach (Element child in parent.Children)
            {
                if (!child.IsCurrentlyEnabled || !child.IsCurrentlyVisible)
                {
                    continue;
                }

                //first, we exclude elements that are worse than the previous best fit / the previously focused element
                if (child == previousBestFit
                 || (wantsPreviousFocusable
                        ? child.Bounds.Y > previousBestFit?.Bounds.Y
                        : child.Bounds.Y < previousBestFit?.Bounds.Y))
                {
                    continue;
                }

                //we put X here because Y has a higher precedence
                if (wantsPreviousFocusable
                        ? child.Bounds.X > previousBestFit?.Bounds.X
                        : child.Bounds.X < previousBestFit?.Bounds.X)
                {
                    continue;
                }

                //if Y is better, immediately replace
                if (wantsPreviousFocusable
                        ? child.Bounds.Y >= bestFit.Bounds.Y
                        : child.Bounds.Y < bestFit.Bounds.Y)
                {
                    bestFit = child;
                }
                //if Y is equal, check X
                else if (
                    wantsPreviousFocusable
                        ? Math.Abs(child.Bounds.Y - bestFit.Bounds.Y) >= 0.1
                        : Math.Abs(child.Bounds.Y - bestFit.Bounds.Y) < 0.1)
                {
                    //if X is better, replace
                    if (wantsPreviousFocusable
                            ? child.Bounds.X >= bestFit.Bounds.X
                            : child.Bounds.X < bestFit.Bounds.X)
                    {
                        bestFit = child;
                    }
                    //if X is equal, only replace it if the best element is already searched
                    else if (
                        (wantsPreviousFocusable
                            ? Math.Abs(child.Bounds.X - bestFit.Bounds.X) >= 0.1
                            : Math.Abs(child.Bounds.X - bestFit.Bounds.X) < 0.1)
                     && bestFit == previousBestFit)
                    {
                        bestFit = child;
                    }
                }
            }

            //if we didn't find anything better than the previous best fit (which was already consumed) OR
            // the element is the "dummy" element (it has no document), it means we don't have any more good elements
            // and the search is over
            if (bestFit == previousBestFit || bestFit.Document == null)
            {
                return null;
            }

            if (bestFit is IFocusable focusable)
            {
                return focusable;
            }

            IFocusable? descendantFocusable = GetFocusableForParent(bestFit);
            if (descendantFocusable != null)
            {
                return descendantFocusable;
            }

            previousBestFit = bestFit;
            watchDogCounter++;

            if (watchDogCounter > parent.Children.Count)
            {
                return null;
            }
        }
    }
}
