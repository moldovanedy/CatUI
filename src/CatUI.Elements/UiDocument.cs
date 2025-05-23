﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CatUI.Data;
using CatUI.Data.ElementData;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Exceptions;
using CatUI.RenderingEngine;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.Elements
{
    /// <summary>
    /// Represents the root of all elements. Every window has one document, and all elements attached to the document
    /// will participate in the application lifecycle.
    /// </summary>
    //this is for WndSet* methods
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)]
    public class UiDocument
    {
        /// <summary>
        /// Fired when the pointer (generally mouse cursor or finger) is moved, either by the user, by the platform
        /// or by your app. The position is always relative to the top-left corner of the client area of the window
        /// (this means that any kind of window decoration will NOT be taken into account).
        /// </summary>
        /// <remarks>
        /// This will only fire while the pointer is inside the window's client area. The coordinates are in dp, not pixels.
        /// </remarks>
        public event PointerMoveEventHandler? PointerMoveEvent;

        /// <summary>
        /// Fired when the pointer enters the window's client area. The coordinates are relative to the top-left
        /// corner of the window and might be negative, especially on fast pointer movements.
        /// </summary>
        public event PointerEnterEventHandler? PointerEnterEvent;

        /// <summary>
        /// Fired when the pointer leaves the window's client area. The coordinates are relative to the top-left
        /// corner of the window and might be negative, especially on fast pointer movements.
        /// </summary>
        public event PointerExitEventHandler? PointerExitEvent;

        /// <summary>
        /// Fired when the pointer is down (pressed) inside the window's client area (the platform protects events outside it).
        /// On mobile, every finger is a pointer, so this event will be fired for each finger. On desktop,
        /// only the primary mouse button click is considered for this kind of event. See remarks for more details.
        /// </summary>
        /// <remarks>
        /// For desktop, only the mouse's primary button is considered as the pointer. Other buttons won't fire this
        /// event. See <see cref="MouseButtonEvent"/> to get those mouse button clicks.
        /// </remarks>
        public event PointerDownEventHandler? PointerDownEvent;

        /// <summary>
        /// Fired when the pointer is up (released) inside the window's client area (the platform protects events outside it).
        /// On mobile, every finger is a pointer, so this event will be fired for each finger. On desktop,
        /// only the primary mouse button click is considered for this kind of event. See remarks for more details.
        /// </summary>
        /// <remarks>
        /// For desktop, only the mouse's primary button is considered as the pointer. Other buttons won't fire this
        /// event. See <see cref="MouseButtonEvent"/> to get those mouse button clicks.
        /// </remarks>
        public event PointerUpEventHandler? PointerUpEvent;

        /// <summary>
        /// Fired when one of the mouse buttons is either pressed or released (see
        /// <see cref="MouseButtonEventArgs.IsPressed"/> for that). To get the currently pressed mouse buttons, see
        /// <see cref="PressedMouseButtons"/>.
        /// </summary>
        public event MouseButtonEventHandler? MouseButtonEvent;

        /// <summary>
        /// Fired when the user uses the mouse wheel, generally for scrolling. This will also fire for touchpad scrolling,
        /// but beware that touchpad scrolling will invoke this event very frequently when scrolling, generally once
        /// per frame (around 60 times per second on most devices), so make sure you don't run expensive functions on
        /// this event.
        /// </summary>
        public event MouseWheelEventHandler? MouseWheelEvent;

        //GLFW already caches this state, but it's probably more efficient to get it from here instead of constantly
        //calling GLFW functions.

        /// <summary>
        /// Represents a bitmap of all the pressed buttons of the mouse. Do NOT convert directly to a
        /// <see cref="MouseButtonType"/> as it's a bitmap, it won't give correct results. Instead, to see for a particular
        /// button, just bitwise AND it with that button, if the result is not 0, it's pressed, otherwise it's released.
        /// </summary>
        /// <example>
        /// ((PressedMouseButtons &amp; MouseButtonType.Secondary) != 0) is true if the secondary mouse button is pressed,
        /// false otherwise.
        /// </example>
        public MouseButtonType PressedMouseButtons { get; private set; }

        /// <summary>
        /// Represents the root element of the document/window. All other elements are children of this element or one of its descendants.
        /// Setting this to another element will remove the previous one with all the children (invoking ExitDocument) and 
        /// attach this element to the document, along with its children (calling EnterDocument).
        /// </summary>
        public Element? Root
        {
            get => _root;
            set
            {
                if (_root != value)
                {
                    _root?.InvokeExitDocumentRecursive();
                }

                _root = value;
                if (_root == null)
                {
                    return;
                }

                _root.Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");
                _root.Document = this;
                _root.Bounds = new Rect(0, 0, ViewportSize.Width, ViewportSize.Height);
            }
        }

        private Element? _root;

        /// <summary>
        /// The viewport size in pixels. If you change this, the document will be resized and all elements will be
        /// redrawn.
        /// </summary>
        public Size ViewportSize
        {
            get => _viewportSize;
            private set
            {
                _viewportSize = value;
                Renderer.SetNewSize(new SKSize(value.Width, value.Height));
                Root?.MarkLayoutDirty();
            }
        }

        private Size _viewportSize = new();

        public Renderer Renderer { get; }

        public Color BackgroundColor
        {
            get => _background;
            set
            {
                _background = value;
                Renderer.SetBgColor(value);
            }
        }

        private Color _background = new(0xff_ff_ff);

        /// <summary>
        /// The factor used to convert between pixels and dp. Any change will trigger a <see cref="ViewportSize"/>
        /// change, which will in turn resize the document and perform layout recalculation on all elements.
        /// </summary>
        public float ContentScale
        {
            get => _contentScale;
            set
            {
                Size originalSize = new(ViewportSize.Width / _contentScale, ViewportSize.Height / _contentScale);
                _contentScale = value;
                ViewportSize = new Size(originalSize.Width * value, originalSize.Height * value);
            }
        }

        private float _contentScale = 1f;

        /// <summary>
        /// The root em font size. This is generally used only for fonts. Every dimension that uses em as a measuring
        /// unit will have the value multiplied with this value. Always consider this in dp (all calculations will take
        /// <see cref="ContentScale"/> into account). The default value is 16.
        /// </summary>
        public float RootEmSize
        {
            get => _rootEmSize;
            set
            {
                _rootEmSize = value;
                Root?.MarkLayoutDirty();
            }
        }

        private float _rootEmSize = 16f;

        #region App lifecycle

        /// <summary>
        /// Represents the current state of the application. When a state change event is fired (such as
        /// <see cref="OnAppActivate"/> or <see cref="OnAppHide"/>), this still holds the previous state (e.g. when OnActivate
        /// is fired, this is still <see cref="AppState.Inactive"/>).
        /// </summary>
        public AppState CurrentAppState { get; private set; } = AppState.Detached;

        /// <summary>
        /// Transition from AppState.Detached to AppState.Active.
        /// </summary>
        public event Action? OnAppStart;

        /// <summary>
        /// Transition from AppState.Active to AppState.Inactive.
        /// </summary>
        public event Action? OnAppDeactivate;

        /// <summary>
        /// Transition from AppState.Inactive to AppState.Active.
        /// </summary>
        public event Action? OnAppActivate;

        /// <summary>
        /// Transition from AppState.Inactive to AppState.Hidden.
        /// </summary>
        public event Action? OnAppHide;

        /// <summary>
        /// Transition from AppState.Hidden to AppState.Inactive.
        /// </summary>
        public event Action? OnAppShow;

        /// <summary>
        /// Transition from AppState.Hidden to AppState.Detached. Windowing implementations on desktop should also take
        /// into account <see cref="OnCloseRequested"/>.
        /// </summary>
        public event Action? OnAppStop;

        /// <summary>
        /// Any app state transition. Fired before any concrete events (such as <see cref="OnAppHide"/> or
        /// <see cref="OnAppActivate"/>). Contains the new app state.
        /// </summary>
        public event Action<AppState>? OnAppStateChange;

        /// <summary>
        /// Called when the user or the platform requested the application close. Returning true (the default behavior)
        /// will close the window immediately, while returning false will make the window continue running.
        /// This can be useful for implementing a prompt for the user (e.g. if they would like to save changes before
        /// the app is closed).
        /// </summary>
        /// <remarks>
        /// Although the window will be closed after this returns true, your app will still run until the end of the
        /// Main function. This is ignored on mobile platforms (Android and iOS) because they don't have a "close"
        /// button, and the app lifecycle is radically different from desktop and web.
        /// </remarks>
        public Func<bool> OnCloseRequested { get; set; } = () => true;

        #endregion

        private readonly Dictionary<string, Element> _elementCache = [];
        private readonly object _window;

        /// <summary>
        /// Creates a new document.
        /// </summary>
        /// <param name="isManagedByPlatform">
        /// True if the rendering is displayed using SkiaSharp.Views, false otherwise (for example, by setting up an
        /// OpenGL context for SkiaSharp to detect and use).
        /// </param>
        /// <param name="window">The IApplicationWindow instance that owns this document.</param>
        /// <param name="initialViewportSize"></param>
        /// <param name="initialContentScale"></param>
        public UiDocument(
            bool isManagedByPlatform,
            object window,
            Size initialViewportSize = default,
            float initialContentScale = 1f)
        {
            _window = window;
            Renderer = new Renderer(isManagedByPlatform);
            ContentScale = initialContentScale;
            ViewportSize = new Size(
                initialViewportSize.Width * initialContentScale,
                initialViewportSize.Height * initialContentScale);
            Renderer.SetBgColor(_background);
        }

        /// <summary>
        /// This will invoke draw for the root element and, consequently, to all eligible children (children that are
        /// visible and can be drawn). You should only call this from the windowing code, not from normal, UI code.
        /// For UI code, see <see cref="MarkVisualDirty"/>.
        /// </summary>
        /// <remarks>
        /// Because of hardware acceleration, this is very efficient, as partial redraws are really uncommon
        /// when drawing using GPU, so don't worry about potential performance issues when completely redrawing all
        /// the elements.
        /// </remarks>
        public void DrawAllElements()
        {
            Renderer.SaveCanvasState();
            Root?.InvokeDraw();
            Renderer.RestoreCanvasState(-1);
        }

        /// <summary>
        /// This will mark the document as "dirty", meaning elements should be redrawn in the next frame. Use this instead
        /// of <see cref="DrawAllElements"/> as much as possible.
        /// </summary>
        public void MarkVisualDirty()
        {
            Renderer.SetCanvasDirty();
        }

        public Element? GetElementById(string id)
        {
            return Root == null ? null : _elementCache.GetValueOrDefault(id);
        }

        /// <summary>
        /// Get the IApplicationWindow that owns this document.
        /// </summary>
        /// <typeparam name="T">The window type. Must be of type IApplicationWindow or derived.</typeparam>
        /// <returns>The window that owns this document.</returns>
        public T GetWindow<T>()
        {
            return (T)_window;
        }

        #region Artificial events

        /// <summary>
        /// Simulates a pointer move inside the document. This will always fire <see cref="PointerMoveEvent"/> on the
        /// document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulatePointerMove(PointerMoveEventArgs e)
        {
            PointerMoveEvent?.Invoke(this, e);

            Root?.CheckInvokePointerEnter(new PointerEnterEventArgs(e.Position, e.AbsolutePosition, e.IsPressed));
            Root?.CheckInvokePointerExit(new PointerExitEventArgs(e.Position, e.AbsolutePosition, e.IsPressed));
            Root?.CheckInvokePointerMove(e);
        }

        /// <summary>
        /// Simulates a pointer entering the document. This will always fire <see cref="PointerEnterEvent"/> on the
        /// document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulatePointerEnter(PointerEnterEventArgs e)
        {
            PointerEnterEvent?.Invoke(this, e);
            Root?.CheckInvokePointerEnter(e);
        }

        /// <summary>
        /// Simulates a pointer exiting the document. This will always fire <see cref="PointerExitEvent"/> on the
        /// document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulatePointerExit(PointerExitEventArgs e)
        {
            PointerExitEvent?.Invoke(this, e);
            Root?.CheckInvokePointerExit(e);
        }

        /// <summary>
        /// Simulates a pointer press inside the document. This will always fire <see cref="PointerDownEvent"/> on the
        /// document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulatePointerDown(PointerDownEventArgs e)
        {
            PointerDownEvent?.Invoke(this, e);
            Root?.CheckInvokePointerDown(e);
        }

        /// <summary>
        /// Simulates a pointer release inside the document. This will always fire <see cref="PointerUpEvent"/> on the
        /// document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulatePointerUp(PointerUpEventArgs e)
        {
            PointerUpEvent?.Invoke(this, e);
            Root?.CheckInvokePointerUp(e);
        }

        /// <summary>
        /// Simulates a mouse button down or up inside the document. This will always fire <see cref="MouseButtonEvent"/>
        /// on the document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulateMouseButton(MouseButtonEventArgs e)
        {
            int bitmap = (int)PressedMouseButtons;
            BinaryUtils.SetBit(ref bitmap, e.IsPressed, (int)e.ButtonType - 1);
            PressedMouseButtons = (MouseButtonType)bitmap;

            MouseButtonEvent?.Invoke(this, e);
            Root?.CheckInvokeMouseButton(e);
        }

        /// <summary>
        /// Simulates a mouse wheel interaction inside the document. This will always fire <see cref="MouseWheelEvent"/>
        /// on the document and will propagate it through the elements (where only eligible elements will react).
        /// Both <see cref="AbstractPointerEventArgs.Position"/> and <see cref="AbstractPointerEventArgs.AbsolutePosition"/>
        /// MUST refer to the absolute position here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For all events, the pointer coordinates are always in pixel coordinates instead of dp, unless otherwise
        /// noted. Keep this in mind when using it with other values in dp.
        /// </para>
        /// <para>
        /// This does not interact with the platform, so it's only possible to use it inside your application, not to
        /// interact with any other user applications. Any event is simply a simulation inside your app window.
        /// </para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        public void SimulateMouseWheel(MouseWheelEventArgs e)
        {
            MouseWheelEvent?.Invoke(this, e);
            Root?.CheckInvokeMouseWheel(e);
        }

        #endregion

        internal void AddToIdCache(Element element)
        {
            if (element.Id == null)
            {
                return;
            }

            if (!_elementCache.TryAdd(element.Id, element))
            {
                throw new DuplicateIdException($"The ID \"{element.Id}\" is already in use.");
            }
        }

        internal void RemoveFromIdCache(string oldId)
        {
            _elementCache.Remove(oldId);
        }

        //Private access for implementations of windowing. This is to avoid having public setters as it's not OK.
        //There might be better ways of doing this without reflection

        #region Set by window

        /// <summary>
        /// Will be used by window implementation to set the viewport's size. Do NOT modify its signature.
        /// </summary>
        /// <param name="viewportSize">The new viewport size.</param>
        internal void WndSetViewportSize(Size viewportSize)
        {
            ViewportSize = viewportSize;
        }

        /// <summary>
        /// Will be used by window implementation to set the app content scale. Do NOT modify its signature.
        /// </summary>
        /// <param name="scale">The new scale.</param>
        internal void WndSetContentScale(float scale)
        {
            ContentScale = scale;
        }

        /// <summary>
        /// Will be used by window implementation to set the current app state. Do NOT modify its signature.
        /// </summary>
        /// <param name="state">
        /// The new state. If it's the same as the <see cref="CurrentAppState"/> or describing an invalid transition,
        /// the method will return without changing or firing anything.
        /// </param>
        internal void WndSetAppState(AppState state)
        {
            if (CurrentAppState == state)
            {
                return;
            }

            switch (CurrentAppState)
            {
                case AppState.Detached:
                    {
                        if (state == AppState.Active)
                        {
                            OnAppStateChange?.Invoke(state);
                            OnAppStart?.Invoke();
                            break;
                        }

                        //invalid transition
                        return;
                    }
                case AppState.Active:
                    {
                        if (state == AppState.Inactive)
                        {
                            OnAppStateChange?.Invoke(state);
                            OnAppDeactivate?.Invoke();
                            break;
                        }

                        //invalid transition
                        return;
                    }
                case AppState.Inactive:
                    {
                        if (state == AppState.Active)
                        {
                            OnAppStateChange?.Invoke(state);
                            OnAppActivate?.Invoke();
                        }
                        else if (state == AppState.Hidden)
                        {
                            OnAppStateChange?.Invoke(state);
                            OnAppHide?.Invoke();
                        }
                        else
                        {
                            //invalid transition
                            return;
                        }

                        break;
                    }
                case AppState.Hidden:
                    {
                        if (state == AppState.Inactive)
                        {
                            OnAppStateChange?.Invoke(state);
                            OnAppShow?.Invoke();
                        }
                        else if (state == AppState.Detached)
                        {
                            OnAppStateChange?.Invoke(state);
                            OnAppStop?.Invoke();
                        }
                        else
                        {
                            //invalid transition
                            return;
                        }

                        break;
                    }
            }

            CurrentAppState = state;
        }

        #endregion


        /// <summary>
        /// The states your application can be in.
        /// </summary>
        public enum AppState
        {
            /// <summary>
            /// The starting state. The app is either starting now or detached from the system and about to be terminated.
            /// </summary>
            Detached = 0,

            /// <summary>
            /// The most important state: the app has focus, is visible to the user and can work normally.
            /// </summary>
            Active = 1,

            /// <summary>
            /// The app lost focus but is still visible to the user and can work normally.
            /// </summary>
            Inactive = 2,

            /// <summary>
            /// The app is either minimized on desktop or simply not visible on mobile (due to the user switching to
            /// another app). You should stop drawing UI in this state and possibly save any relevant data. On mobile,
            /// this is where the app has higher chances to be terminated by the system, so it's important to save any
            /// important data.
            /// </summary>
            Hidden = 3
        }
    }
}
