using System;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input;
using CatUI.Data.Events.Input.Keyboard;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.DocumentManagers;

namespace CatUI.Elements
{
    public partial class Element
    {
        #region Events

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) enters the <see cref="Bounds"/> of the element.
        /// </summary>
        public event PointerEnterEventHandler? PointerEnterEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) exits the <see cref="Bounds"/> of the element.
        /// </summary>
        public event PointerExitEventHandler? PointerExitEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) moves inside <see cref="Bounds"/> of the element.
        /// This will be fired a lot of times, so ensure your logic is not computationally heavy.
        /// </summary>
        public event PointerMoveEventHandler? PointerMoveEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) is pressed inside the <see cref="Bounds"/> of the element.
        /// </summary>
        public event PointerDownEventHandler? PointerDownEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) is released inside the <see cref="Bounds"/> of the element.
        /// </summary>
        public event PointerUpEventHandler? PointerUpEvent;

        /// <summary>
        /// Fired when the mouse (or touchpad) is pressed or released (see <see cref="AbstractPointerEventArgs.IsPressed"/>
        /// for that) inside the <see cref="Bounds"/> of the element.
        /// </summary>
        public event MouseButtonEventHandler? MouseButtonEvent;

        /// <summary>
        /// Fired when the mouse wheel (or touchpad scroll gesture) is moved inside the <see cref="Bounds"/> of the element.
        /// When the user uses the touchpad, this will be fired a lot of times, so ensure your logic is not
        /// computationally heavy.
        /// </summary>
        /// <remarks>
        /// Even if rare on mice, most touchpads support horizontal scrolling as well, so you generally have to consider
        /// that as well for scrolling (it's <see cref="MouseWheelEventArgs.DeltaX"/>).
        /// </remarks>
        public event MouseWheelEventHandler? MouseWheelEvent;

        /// <summary>
        /// Fired when a key on a keyboard (virtual or physical) was pressed, released, or held down for some time
        /// (this will be a repeating event) while the window has focus. Unlike <see cref="CharTypedEvent"/> this
        /// actually uses physical keys on the keyboard rather than text. However, for text input, you should
        /// always use <see cref="CharTypedEvent"/>.
        /// </summary>
        public event KeyEventHandler? KeyEvent;

        /// <summary>
        /// Fired when the user types in a character. This is different from <see cref="KeyEvent"/>, as this is only
        /// for text input.
        /// </summary>
        /// <remarks>
        /// On some keyboards, you need multiple keystrokes to generate a single character, so this is preferred over
        /// the key events for text input, but not for keyboard shortcuts.
        /// </remarks>
        public event CharTypedEventHandler? CharTypedEvent;

        #endregion

        public PointerEnterEventHandler? OnPointerEnter
        {
            get => _onPointerEnter;
            set
            {
                PointerEnterEvent -= _onPointerEnter;
                _onPointerEnter = value;
                PointerEnterEvent += _onPointerEnter;
            }
        }

        private PointerEnterEventHandler? _onPointerEnter;

        public PointerExitEventHandler? OnPointerExit
        {
            get => _onPointerExit;
            set
            {
                PointerExitEvent -= _onPointerExit;
                _onPointerExit = value;
                PointerExitEvent += _onPointerExit;
            }
        }

        private PointerExitEventHandler? _onPointerExit;

        public PointerMoveEventHandler? OnPointerMove
        {
            get => _onPointerMove;
            set
            {
                PointerMoveEvent -= _onPointerMove;
                _onPointerMove = value;
                PointerMoveEvent += _onPointerMove;
            }
        }

        private PointerMoveEventHandler? _onPointerMove;

        public PointerDownEventHandler? OnPointerDown
        {
            get => _onPointerDown;
            set
            {
                PointerDownEvent -= _onPointerDown;
                _onPointerDown = value;
                PointerDownEvent += _onPointerDown;
            }
        }

        private PointerDownEventHandler? _onPointerDown;

        public PointerUpEventHandler? OnPointerUp
        {
            get => _onPointerUp;
            set
            {
                PointerUpEvent -= _onPointerUp;
                _onPointerUp = value;
                PointerUpEvent += _onPointerUp;
            }
        }

        private PointerUpEventHandler? _onPointerUp;

        public MouseButtonEventHandler? OnMouseButton
        {
            get => _onMouseButton;
            set
            {
                MouseButtonEvent -= _onMouseButton;
                _onMouseButton = value;
                MouseButtonEvent += _onMouseButton;
            }
        }

        private MouseButtonEventHandler? _onMouseButton;

        public MouseWheelEventHandler? OnMouseWheel
        {
            get => _onMouseWheel;
            set
            {
                MouseWheelEvent -= _onMouseWheel;
                _onMouseWheel = value;
                MouseWheelEvent += _onMouseWheel;
            }
        }

        private MouseWheelEventHandler? _onMouseWheel;

        public KeyEventHandler? OnKeyEvent
        {
            get => _onKeyEvent;
            set
            {
                KeyEvent -= _onKeyEvent;
                _onKeyEvent = value;
                KeyEvent += _onKeyEvent;
            }
        }

        private KeyEventHandler? _onKeyEvent;

        public CharTypedEventHandler? OnCharTyped
        {
            get => _onCharTyped;
            set
            {
                CharTypedEvent -= _onCharTyped;
                _onCharTyped = value;
                CharTypedEvent += _onCharTyped;
            }
        }

        private CharTypedEventHandler? _onCharTyped;

        /// <summary>
        /// If true, will bypass the checks on pointer exiting so it can fire events needed when an event is cancelled.
        /// You should generally only use this inside overrides of <see cref="CheckInvokePointerUp"/> and
        /// <see cref="CheckInvokeMouseButton"/>. When overriding any of the CheckInvoke* methods, please look closely
        /// at the initial implementation in ElementEventCheckingPartial.cs, as you are completely responsible for doing proper
        /// checks and setting this variable to the correct value each time.
        /// </summary>
        /// <remarks>
        /// This is generally only set by <see cref="FirePointerExitCancelEvents"/> (called on <see cref="CheckInvokePointerExit"/>)
        /// and overrides of <see cref="CheckInvokePointerUp"/> and <see cref="CheckInvokeMouseButton"/> must set this to
        /// false if it's true, otherwise (when it's false) it should perform the regular checks. 
        /// </remarks>
        /// <example>
        /// See the implementation of <see cref="CheckInvokePointerUp"/> inside the ElementEventCheckingPartial.cs for an example.
        /// </example>
        protected bool CanBypassPointerChecks { get; set; }

        /// <summary>
        /// If true, it means that the pointer was inside the element bounds on the last check. When overriding any of the
        /// CheckInvoke* methods, please look closely at the initial implementation in ElementEventCheckingPartial.cs, as you
        /// are completely responsible for doing proper checks and setting this variable to the correct value each time.
        /// </summary>
        protected bool WasPointerInside { get; set; }


        #region Public API

        protected virtual void PointerEnter(object sender, PointerEnterEventArgs e) { }
        protected virtual void PointerExit(object sender, PointerExitEventArgs e) { }
        protected virtual void PointerMove(object sender, PointerMoveEventArgs e) { }
        protected virtual void PointerDown(object sender, PointerDownEventArgs e) { }
        protected virtual void PointerUp(object sender, PointerUpEventArgs e) { }
        protected virtual void MouseButton(object sender, MouseButtonEventArgs e) { }
        protected virtual void MouseWheel(object sender, MouseWheelEventArgs e) { }

        #region Pointer capture

        public bool HasPointerCapture { get; private set; }

        /// <summary>
        /// Invoked when the pointer capture is lost (either by manually calling <see cref="ReleasePointerCapture"/> or
        /// when the pointer is up). The first argument is the pointer ID, the second parameter is a boolean that is
        /// true if the pointer capture was released because the pointer is up, false if the pointer capture was
        /// manually released by calling <see cref="ReleasePointerCapture"/>.
        /// </summary>
        public Action<int, bool>? OnPointerCaptureRelease;

        /// <summary>
        /// Makes the element receive pointer move events even when the pointer is outside the bounds of this element.
        /// This only works until the pointer is either released (pointer up) by the user/system or by calling
        /// <see cref="ReleasePointerCapture"/>.
        /// </summary>
        public void SetPointerCapture()
        {
            HasPointerCapture = true;
            Document?.ElementSetPointerCapture(this, 0);
        }

        /// <summary>
        /// Releases the pointer capture set by <see cref="SetPointerCapture"/>, while also sending pointer up events to
        /// the elements. This also invokes <see cref="OnPointerCaptureRelease"/>.
        /// </summary>
        public void ReleasePointerCapture()
        {
            if (!HasPointerCapture)
            {
                return;
            }

            HasPointerCapture = false;
            Document?.ElementReleasePointerCapture(this, 0);
            OnPointerCaptureRelease?.Invoke(0, false);

            SendPointerUpOnRelease();
        }

        /// <summary>
        /// Will release the pointer capture, as well as sending pointer up events
        /// </summary>
        internal void InternallyReleasePointerCapture()
        {
            if (!HasPointerCapture)
            {
                return;
            }

            HasPointerCapture = false;
            Document?.ElementReleasePointerCapture(this, 0);
            OnPointerCaptureRelease?.Invoke(0, true);

            SendPointerUpOnRelease();
        }

        private void SendPointerUpOnRelease()
        {
            InputPointer? pointer = Document?.GetActivePointerFromId(0);
            Point2D absolutePos = pointer?.AbsolutePosition ?? Point2D.Zero;
            FirePointerUp(
                new PointerUpEventArgs(
                    new Point2D(absolutePos.X - Bounds.X, absolutePos.Y - Bounds.Y),
                    absolutePos,
                    0));
        }

        #endregion //Pointer capture

        #endregion

        //TODO: check if we can put pointer enter and pointer exit inside a single method
        //

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerEnterEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner) and whether the pointer is pressed or not.
        /// </param>
        protected internal virtual void CheckInvokePointerEnter(PointerEnterEventArgs e)
        {
            if (!IsCurrentlyEnabled || !IsPointerInside(e))
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokePointerEnter(e);
                i--;
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
                e.IsPressed,
                e.PointerId);
            FirePointerEnter(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected void FirePointerEnter(PointerEnterEventArgs elementArgs)
        {
            PointerEnterEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerExitEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner) and whether the pointer is pressed or not.
        /// </param>
        protected internal virtual void CheckInvokePointerExit(PointerExitEventArgs e)
        {
            if (!IsCurrentlyEnabled)
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokePointerExit(e);
                i--;
            }

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
                e.IsPressed,
                e.PointerId);
            FirePointerExit(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }

            FirePointerExitCancelEvents(e);
        }

        protected void FirePointerExit(PointerExitEventArgs elementArgs)
        {
            PointerExitEvent?.Invoke(this, elementArgs);
        }

        protected void FirePointerExitCancelEvents(PointerExitEventArgs e)
        {
            //TODO: this is firing the pointer up events in between the pointer exit events... we probably need to fix that
            //

            if (e.IsPressed)
            {
                CanBypassPointerChecks = true;
                CheckInvokePointerUp(
                    new PointerUpEventArgs(e.Position, e.AbsolutePosition, e.PointerId, true));
                CanBypassPointerChecks = false;
            }

            //8 is MouseButtonType count
            for (int btn = 0; btn < 8; btn++)
            {
                if (((int)(Document?.PressedMouseButtons ?? 0) & (1 << btn)) == 0)
                {
                    continue;
                }

                try
                {
                    CanBypassPointerChecks = true;
                    CheckInvokeMouseButton(
                        new MouseButtonEventArgs(
                            e.Position,
                            e.AbsolutePosition,
                            (MouseButtonType)(1 << btn),
                            e.IsPressed,
                            e.PointerId,
                            true));
                }
                finally
                {
                    CanBypassPointerChecks = false;
                }
            }
        }

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerMoveEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner) and whether the pointer is pressed or not.
        /// </param>
        protected internal virtual void CheckInvokePointerMove(PointerMoveEventArgs e)
        {
            if (!IsCurrentlyEnabled || !WasPointerInside || !IsPointerInside(e))
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokePointerMove(e);
                i--;
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
                e.IsPressed,
                e.PointerId);
            FirePointerMove(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected internal void FirePointerMove(PointerMoveEventArgs elementArgs)
        {
            PointerMoveEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerDownEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner).
        /// </param>
        protected internal virtual void CheckInvokePointerDown(PointerDownEventArgs e)
        {
            if (!IsCurrentlyEnabled || !WasPointerInside || !IsPointerInside(e))
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokePointerDown(e);
                i--;
            }

            if (e.IsPropagationStopped)
            {
                return;
            }

            var elementArgs = new PointerDownEventArgs(
                new Point2D(e.AbsolutePosition.X - Bounds.X, e.AbsolutePosition.Y - Bounds.Y),
                e.AbsolutePosition,
                e.PointerId);
            FirePointerDown(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected void FirePointerDown(PointerDownEventArgs elementArgs)
        {
            PointerDownEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element (with the exception from
        /// <see cref="CheckInvokePointerExit"/>, as it's important to invoke this on exit). It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerUpEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner).
        /// </param>
        protected internal virtual void CheckInvokePointerUp(PointerUpEventArgs e)
        {
            if (!IsCurrentlyEnabled || HasPointerCapture)
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokePointerUp(e);
                i--;
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
                e.PointerId,
                e.WasCancelled);
            FirePointerUp(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected void FirePointerUp(PointerUpEventArgs elementArgs)
        {
            PointerUpEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element (with the exception from
        /// <see cref="CheckInvokePointerExit"/>, as it's important to invoke this on exit). It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerUpEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner).
        /// </param>
        protected internal virtual void CheckInvokeMouseButton(MouseButtonEventArgs e)
        {
            if (!IsCurrentlyEnabled)
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokeMouseButton(e);
                i--;
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
                e.PointerId,
                e.WasCancelled);
            FireMouseButton(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected void FireMouseButton(MouseButtonEventArgs elementArgs)
        {
            MouseButtonEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="MouseWheelEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner).
        /// </param>
        protected internal virtual void CheckInvokeMouseWheel(MouseWheelEventArgs e)
        {
            if (!IsCurrentlyEnabled || !WasPointerInside || !IsPointerInside(e))
            {
                return;
            }

            int i = Children.Count - 1;
            while (i >= 0)
            {
                Children[i].CheckInvokeMouseWheel(e);
                i--;
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
                e.IsPressed,
                e.PointerId);
            FireMouseWheel(elementArgs);

            if (elementArgs.IsPropagationStopped)
            {
                e.StopPropagation();
            }
        }

        protected void FireMouseWheel(MouseWheelEventArgs elementArgs)
        {
            MouseWheelEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// Note that this will always use <see cref="FocusManager.CurrentlyFocusedElement"/> as the target, not
        /// the current element. If there is no focused element, this will return immediately, having no effect.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void CheckInvokeKeyEvent(KeyEventArgs e)
        {
            var currentElement = Document?.FocusManager.CurrentlyFocusedElement as Element;
            while (currentElement != null)
            {
                var elementArgs = new KeyEventArgs(e);
                currentElement.FireKeyEvent(elementArgs);

                if (elementArgs.IsPropagationStopped)
                {
                    return;
                }

                currentElement = currentElement.GetParent();
            }
        }

        protected void FireKeyEvent(KeyEventArgs elementArgs)
        {
            KeyEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// Note that this will always use <see cref="FocusManager.CurrentlyFocusedElement"/> as the target, not
        /// the current element. If there is no focused element, this will return immediately, having no effect.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void CheckInvokeCharTyped(CharTypedEventArgs e)
        {
            var currentElement = Document?.FocusManager.CurrentlyFocusedElement as Element;
            while (currentElement != null)
            {
                var elementArgs = new CharTypedEventArgs(e);
                currentElement.FireCharTyped(elementArgs);

                if (elementArgs.IsPropagationStopped)
                {
                    return;
                }

                currentElement = currentElement.GetParent();
            }
        }

        protected void FireCharTyped(CharTypedEventArgs elementArgs)
        {
            CharTypedEvent?.Invoke(this, elementArgs);
        }


        /// <summary>
        /// Checks if the pointer is inside the element or not using either the <see cref="ClipPath"/> or the
        /// <see cref="Bounds"/>, depending on whether <see cref="ClipType"/> flags has <see cref="ClipApplicability.HitTesting"/>
        /// enabled or not and the <see cref="ClipPath"/> is not null. Only the absolute position is used.
        /// </summary>
        /// <param name="e">The pointer data that contains the pointer position.</param>
        /// <returns>
        /// True if the given pointer data indicates that the pointer is inside the element's bounds, false otherwise.
        /// </returns>
        protected bool IsPointerInside(AbstractPointerEventArgs e)
        {
            Rect bounds = Bounds;
            if (ClipPath != null && (ClipType & ClipApplicability.HitTesting) != 0)
            {
                return
                    Document != null &&
                    ClipPath.IsPointInside(e.AbsolutePosition, bounds, Document.ContentScale, Document.ViewportSize);
            }

            return Rect.IsPointInside(ref bounds, e.AbsolutePosition);
        }
    }
}
