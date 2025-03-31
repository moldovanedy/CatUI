using CatUI.Data;
using CatUI.Data.Events.Input.Pointer;

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

        private bool _isPointerInside;
        private bool _canBypassPointerChecks;


        #region Public API

        protected virtual void PointerEnter(object sender, PointerEnterEventArgs e) { }
        protected virtual void PointerExit(object sender, PointerExitEventArgs e) { }
        protected virtual void PointerMove(object sender, PointerMoveEventArgs e) { }
        protected virtual void PointerDown(object sender, PointerDownEventArgs e) { }
        protected virtual void PointerUp(object sender, PointerUpEventArgs e) { }
        protected virtual void MouseButton(object sender, MouseButtonEventArgs e) { }
        protected virtual void MouseWheel(object sender, MouseWheelEventArgs e) { }

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
        internal virtual void CheckInvokePointerEnter(PointerEnterEventArgs e)
        {
            Rect bounds = Bounds;
            if (!Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.CheckInvokePointerEnter(e);
            }

            if (_isPointerInside)
            {
                return;
            }

            _isPointerInside = true;
            var elementArgs = new PointerEnterEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.IsPressed);
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
        internal virtual void CheckInvokePointerExit(PointerExitEventArgs e)
        {
            foreach (Element child in Children)
            {
                child.CheckInvokePointerExit(e);
            }

            Rect bounds = Bounds;
            if (Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            if (!_isPointerInside)
            {
                return;
            }

            _isPointerInside = false;
            var elementArgs = new PointerExitEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.IsPressed);
            PointerExitEvent?.Invoke(this, elementArgs);

            //TODO: this is firing the pointer up events in between the pointer exit events... we probably need to fix that
            //

            //we also need to check if we need to fire pointer up or mouse buttons up events (generally if the user
            //drags the cursor or finger outside the element while pressed)
            //it's ok to only call these functions here instead of the root because at most this element is affected,
            //if there are other elements higher up the tree affected, this pointer exit invocation will solve those as well
            if (e.IsPressed)
            {
                _canBypassPointerChecks = true;
                CheckInvokePointerUp(new PointerUpEventArgs(e.Position, e.AbsolutePosition, true));
                _canBypassPointerChecks = false;
            }

            //8 is MouseButtonType count
            for (int i = 0; i < 8; i++)
            {
                if (((int)(Document?.PressedMouseButtons ?? 0) & (1 << i)) == 0)
                {
                    continue;
                }

                try
                {
                    _canBypassPointerChecks = true;
                    CheckInvokeMouseButton(
                        new MouseButtonEventArgs(
                            e.Position,
                            e.AbsolutePosition,
                            (MouseButtonType)(1 << i),
                            e.IsPressed,
                            true));
                }
                finally
                {
                    _canBypassPointerChecks = false;
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
        internal virtual void CheckInvokePointerMove(PointerMoveEventArgs e)
        {
            if (!_isPointerInside)
            {
                return;
            }

            Rect bounds = Bounds;
            if (!Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.CheckInvokePointerMove(e);
            }

            var elementArgs = new PointerMoveEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.DeltaX,
                e.DeltaY,
                e.IsPressed);
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
        internal virtual void CheckInvokePointerDown(PointerDownEventArgs e)
        {
            if (!_isPointerInside)
            {
                return;
            }

            Rect bounds = Bounds;
            if (!Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.CheckInvokePointerDown(e);
            }

            var elementArgs = new PointerDownEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition);
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
        internal virtual void CheckInvokePointerUp(PointerUpEventArgs e)
        {
            foreach (Element child in Children)
            {
                child.CheckInvokePointerUp(e);
            }

            Rect bounds = Bounds;
            if (_canBypassPointerChecks)
            {
                _canBypassPointerChecks = false;
            }
            else
            {
                if (!_isPointerInside)
                {
                    return;
                }

                if (!Rect.IsPointInside(ref bounds, e.Position))
                {
                    return;
                }
            }

            var elementArgs = new PointerUpEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.WasCancelled);
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
        internal virtual void CheckInvokeMouseButton(MouseButtonEventArgs e)
        {
            //TODO: check if on desktop we can invoke pointer down and pointer up from here, saving some performance
            // if it's worth it

            foreach (Element child in Children)
            {
                child.CheckInvokeMouseButton(e);
            }

            Rect bounds = Bounds;
            if (_canBypassPointerChecks)
            {
                _canBypassPointerChecks = false;
            }
            else
            {
                if (!_isPointerInside)
                {
                    return;
                }

                if (!Rect.IsPointInside(ref bounds, e.Position))
                {
                    return;
                }
            }

            var elementArgs = new MouseButtonEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.ButtonType,
                e.IsPressed,
                e.WasCancelled);
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
        internal virtual void CheckInvokeMouseWheel(MouseWheelEventArgs e)
        {
            if (!_isPointerInside)
            {
                return;
            }

            Rect bounds = Bounds;
            if (!Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.CheckInvokeMouseWheel(e);
            }

            var elementArgs = new MouseWheelEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.DeltaX,
                e.DeltaY,
                e.IsPressed);
            MouseWheelEvent?.Invoke(this, elementArgs);
        }
    }
}
