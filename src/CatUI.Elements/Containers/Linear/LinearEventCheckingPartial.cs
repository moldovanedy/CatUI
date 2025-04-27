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
            if (!IsInsideViewport() || Document == null)
            {
                return;
            }

            int? restoreCount = DrawingSetClip();

            FireDrawEvent();
            if (!Visible || Children.Count == 0)
            {
                return;
            }

            //TODO: start this at the first visible element, not at the first element
            float lastPositionOnAxis =
                ContainerOrientation == Orientation.Horizontal
                    ? Children[0].Bounds.X
                    : Children[0].Bounds.Y;
            float posLimit =
                ContainerOrientation == Orientation.Horizontal
                    ? Document.ViewportSize.Width
                    : Document.ViewportSize.Height;

            int i = 0;
            while (i < Children.Count && lastPositionOnAxis <= posLimit)
            {
                Children[i].InvokeDraw();
                lastPositionOnAxis =
                    ContainerOrientation == Orientation.Horizontal
                        ? Children[i].Bounds.X
                        : Children[i].Bounds.Y;
                i++;
            }

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
            float requiredDelta = (ContainerOrientation == Orientation.Horizontal
                ? e.AbsolutePosition.X - _lastPointerPosition.X
                : e.AbsolutePosition.Y - _lastPointerPosition.Y) * 1.5f;
            _lastPointerPosition = e.AbsolutePosition;

            float currentDelta = 0;
            int currentIndex = _lastCheckedElementIndex + 1;
            float lastRelevantPosition =
                ContainerOrientation == Orientation.Horizontal
                    ? Children[_lastCheckedElementIndex].Bounds.X
                    : Children[_lastCheckedElementIndex].Bounds.Y;

            while (currentDelta < requiredDelta && currentIndex < Children.Count)
            {
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
                ContainerOrientation == Orientation.Horizontal
                    ? Children[_lastCheckedElementIndex].Bounds.X
                    : Children[_lastCheckedElementIndex].Bounds.Y;

            while (currentDelta > requiredDelta && currentIndex >= 0)
            {
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

            //we first check the last element, as it is possible to fire to the same one, eliminating search
            int idx = _lastCheckedElementIndex;
            int left = 0;
            int right = Children.Count - 1;
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

                idx = (left + right) / 2;
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
    }
}
