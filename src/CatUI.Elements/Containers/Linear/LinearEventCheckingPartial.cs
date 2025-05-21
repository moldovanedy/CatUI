using System;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input.Pointer;

namespace CatUI.Elements.Containers.Linear
{
    public abstract partial class LinearContainerBase
    {
        private int _lastCheckedElementIndex;
        private Point2D _lastPointerPosition = Point2D.Zero;

        protected internal override void InvokeDraw()
        {
            if (!IsInsideViewport() || Document == null || !IsCurrentlyEnabled)
            {
                return;
            }

            int? restoreCount = DrawingSetClip();

            FireDrawEvent();
            if (!IsCurrentlyVisible || Children.Count == 0)
            {
                goto Restore;
            }

            //get the first enabled element
            //TODO: start this at the first visible element, not at the first element
            int index = GetIndexOfNextEnabledElement(0, 0, Children.Count - 1);

            float lastPositionOnAxis =
                ContainerOrientation == Orientation.Horizontal
                    ? Children[index].Bounds.X
                    : Children[index].Bounds.Y;
            float posLimit =
                ContainerOrientation == Orientation.Horizontal
                    ? Document.ViewportSize.Width
                    : Document.ViewportSize.Height;

            int i = 0;
            while (i < Children.Count && lastPositionOnAxis <= posLimit)
            {
                Children[i].InvokeDraw();

                if (Children[i].IsCurrentlyEnabled)
                {
                    lastPositionOnAxis =
                        ContainerOrientation == Orientation.Horizontal
                            ? Children[i].Bounds.X
                            : Children[i].Bounds.Y;
                }

                i++;
            }

        Restore:
            if ((ClipType & ClipApplicability.Drawing) != 0 && restoreCount != null)
            {
                Document.Renderer.RestoreCanvasState(restoreCount.Value);
            }
        }

        protected internal override void CheckInvokePointerEnter(PointerEnterEventArgs e)
        {
            if (!IsPointerInside(e))
            {
                return;
            }

            if (Children.Count > 0)
            {
                _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
                Children[_lastCheckedElementIndex].CheckInvokePointerEnter(e);
            }

            if (WasPointerInside)
            {
                return;
            }

            WasPointerInside = true;

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new PointerEnterEventArgs(
                new Point2D(e.AbsolutePosition.X - Bounds.X, e.AbsolutePosition.Y - Bounds.Y),
                e.AbsolutePosition,
                e.IsPressed);
            FirePointerEnter(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected internal override void CheckInvokePointerExit(PointerExitEventArgs e)
        {
            if (Children.Count == 0)
            {
                goto NoChildCheck;
            }

            _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
            Children[_lastCheckedElementIndex].CheckInvokePointerExit(e);

            //also check in adjacent elements because we need it for input cancelling and especially for firing
            //PointerExit correctly

            //the distance that the pointer travelled plus 50% to ensure we don't miss elements
            float requiredDelta = Math.Abs(ContainerOrientation == Orientation.Horizontal
                ? e.AbsolutePosition.X - _lastPointerPosition.X
                : e.AbsolutePosition.Y - _lastPointerPosition.Y) * 1.5f;
            _lastPointerPosition = e.AbsolutePosition;

            float currentDelta = 0;
            int currentIndex = _lastCheckedElementIndex + 1;
            float lastRelevantPosition =
                ContainerOrientation == Orientation.Horizontal ? e.AbsolutePosition.X : e.AbsolutePosition.Y;

            while (currentDelta < requiredDelta && currentIndex < Children.Count)
            {
                currentIndex = GetIndexOfNextEnabledElement(currentIndex, currentIndex, Children.Count - 1);
                //it means there are no more enabled elements in this direction
                if (!Children[currentIndex].IsCurrentlyEnabled)
                {
                    break;
                }

                Element child = Children[currentIndex];
                child.CheckInvokePointerExit(e);

                float thisRelevantPosition =
                    ContainerOrientation == Orientation.Horizontal ? child.Bounds.X : child.Bounds.Y;
                currentDelta += thisRelevantPosition - lastRelevantPosition;
                lastRelevantPosition = thisRelevantPosition;

                currentIndex++;
            }

            requiredDelta = -requiredDelta;
            currentDelta = 0;
            currentIndex = _lastCheckedElementIndex - 1;
            lastRelevantPosition =
                ContainerOrientation == Orientation.Horizontal ? e.AbsolutePosition.X : e.AbsolutePosition.Y;

            while (currentDelta > requiredDelta && currentIndex >= 0)
            {
                currentIndex = GetIndexOfNextEnabledElement(currentIndex, 0, currentIndex);
                //it means there are no more enabled elements in this direction
                if (!Children[currentIndex].IsCurrentlyEnabled)
                {
                    break;
                }

                Element child = Children[currentIndex];
                child.CheckInvokePointerExit(e);

                float thisRelevantPosition =
                    ContainerOrientation == Orientation.Horizontal ? child.Bounds.X : child.Bounds.Y;
                currentDelta += thisRelevantPosition - lastRelevantPosition;
                lastRelevantPosition = thisRelevantPosition;

                currentIndex--;
            }

        NoChildCheck:
            if (Document == null || IsPointerInside(e) || !WasPointerInside)
            {
                return;
            }

            WasPointerInside = false;

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new PointerExitEventArgs(
                new Point2D(e.AbsolutePosition.X - Bounds.X, e.AbsolutePosition.Y - Bounds.Y),
                e.AbsolutePosition,
                e.IsPressed);
            FirePointerExit(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }

            FirePointerExitCancelEvents(e);
        }

        protected internal override void CheckInvokePointerMove(PointerMoveEventArgs e)
        {
            if (!WasPointerInside || !IsPointerInside(e))
            {
                return;
            }

            if (Children.Count > 0)
            {
                _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
                Children[_lastCheckedElementIndex].CheckInvokePointerMove(e);
            }

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new PointerMoveEventArgs(
                new Point2D(e.AbsolutePosition.X - Bounds.X, e.AbsolutePosition.Y - Bounds.Y),
                e.AbsolutePosition,
                e.DeltaX,
                e.DeltaY,
                e.IsPressed);
            FirePointerMove(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected internal override void CheckInvokePointerDown(PointerDownEventArgs e)
        {
            if (!WasPointerInside || !IsPointerInside(e))
            {
                return;
            }

            if (Children.Count > 0)
            {
                _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
                Children[_lastCheckedElementIndex].CheckInvokePointerDown(e);
            }

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new PointerDownEventArgs(
                new Point2D(e.AbsolutePosition.X - Bounds.X, e.AbsolutePosition.Y - Bounds.Y),
                e.AbsolutePosition);
            FirePointerDown(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected internal override void CheckInvokePointerUp(PointerUpEventArgs e)
        {
            if (Children.Count > 0)
            {
                //TODO: cancelling does not work on the opposite axis of the container
                _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
                Children[_lastCheckedElementIndex].CheckInvokePointerUp(e);
            }

            Rect bounds = Bounds;
            if (CanBypassPointerChecks)
            {
                CanBypassPointerChecks = false;
            }
            else
            {
                if (!WasPointerInside || !IsPointerInside(e))
                {
                    return;
                }
            }

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new PointerUpEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.WasCancelled);
            FirePointerUp(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected internal override void CheckInvokeMouseButton(MouseButtonEventArgs e)
        {
            if (Children.Count > 0)
            {
                _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
                Children[_lastCheckedElementIndex].CheckInvokeMouseButton(e);
            }

            Rect bounds = Bounds;
            if (CanBypassPointerChecks)
            {
                CanBypassPointerChecks = false;
            }
            else
            {
                if (!WasPointerInside || !IsPointerInside(e))
                {
                    return;
                }
            }

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new MouseButtonEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.ButtonType,
                e.IsPressed,
                e.WasCancelled);
            FireMouseButton(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected internal override void CheckInvokeMouseWheel(MouseWheelEventArgs e)
        {
            if (!WasPointerInside || !IsPointerInside(e))
            {
                return;
            }

            if (Children.Count > 0)
            {
                _lastCheckedElementIndex = GetEligibleElementIndex(e.AbsolutePosition);
                Children[_lastCheckedElementIndex].CheckInvokeMouseWheel(e);
            }

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new MouseWheelEventArgs(
                new Point2D(e.AbsolutePosition.X - Bounds.X, e.AbsolutePosition.Y - Bounds.Y),
                e.AbsolutePosition,
                e.DeltaX,
                e.DeltaY,
                e.IsPressed);
            FireMouseWheel(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }


        private int GetEligibleElementIndex(Point2D pointToCheck)
        {
            if (Children.Count == 0)
            {
                return -1;
            }

            if (_lastCheckedElementIndex >= Children.Count)
            {
                _lastCheckedElementIndex = Children.Count / 2;
            }

            //we first check the last checked element, as it is possible to fire to the same one, eliminating search
            int idx = _lastCheckedElementIndex;
            int left = 0;
            int right = Children.Count - 1;

            idx = GetIndexOfNextEnabledElement(idx, left, right);
            Rect bounds = Children[idx].Bounds;

            //binary search
            while (left < right && !ShouldStop(ref bounds, pointToCheck, out bool pointIsAfterBounds))
            {
                //if we're here, there are at least 2 children, so it's impossible to get out of the array bounds 
                if (pointIsAfterBounds)
                {
                    left = idx + 1;
                }
                else
                {
                    right = idx - 1;
                }

                idx = GetIndexOfNextEnabledElement((left + right) / 2, left, right);
                bounds = Children[idx].Bounds;
            }

            return idx;
        }

        private bool ShouldStop(ref Rect bounds, Point2D pointToCheck, out bool pointIsAfterBounds)
        {
            bool stop;
            pointIsAfterBounds = false;

            if (ContainerOrientation == Orientation.Horizontal)
            {
                stop = bounds.X <= pointToCheck.X && bounds.X + bounds.Width >= pointToCheck.X;
                if (!stop)
                {
                    pointIsAfterBounds = bounds.X + bounds.Width < pointToCheck.X;
                }
            }
            else
            {
                stop = bounds.Y <= pointToCheck.Y && bounds.Y + bounds.Height >= pointToCheck.Y;
                if (!stop)
                {
                    pointIsAfterBounds = bounds.Y + bounds.Height < pointToCheck.Y;
                }
            }

            return stop;
        }

        private int GetIndexOfNextEnabledElement(int indexOfCurrentElement, int left, int right)
        {
            int initialIndex = indexOfCurrentElement;
            if (Children[indexOfCurrentElement].IsCurrentlyEnabled)
            {
                return indexOfCurrentElement;
            }

            //go to the next enabled element
            while (indexOfCurrentElement < right && !Children[indexOfCurrentElement].IsCurrentlyEnabled)
            {
                indexOfCurrentElement++;
            }

            //if nothing was enabled to the right, go to the left now
            if (!Children[indexOfCurrentElement].IsCurrentlyEnabled)
            {
                indexOfCurrentElement = initialIndex;
                while (indexOfCurrentElement > left && !Children[indexOfCurrentElement].IsCurrentlyEnabled)
                {
                    indexOfCurrentElement--;
                }
            }

            return indexOfCurrentElement;
        }
    }
}
