using System;
using System.Runtime.CompilerServices;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input;
using CatUI.Data.Events.Input.Keyboard;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements;
using CatUI.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using KeyModifiers = CatUI.Data.Enums.KeyModifiers;

namespace CatUI.Windowing.DesktopApp
{
    public unsafe partial class DesktopWindow
    {
        private void RegisterCallbacks()
        {
            #region Window managing

            _resizeCallback = (_, newWidth, newHeight) =>
            {
                newWidth = UtilNormalizeGlfwWm(newWidth);
                newHeight = UtilNormalizeGlfwWm(newHeight);

                ResizedEvent?.Invoke(
                    this,
                    new WindowResizedEventArgs(Width, Height, newWidth, newHeight)
                );
            };
            ResizedEvent += OnResize;
            GLFW.SetWindowSizeCallback(GlfwWindow, _resizeCallback);

            _contentScaleCallback = (_, xScale, _) =>
            {
                DocumentInvoke("WndSetContentScale", xScale);
                Document.Renderer.SetCanvasDirty();
            };
            GLFW.SetWindowContentScaleCallback(GlfwWindow, _contentScaleCallback);

            _iconifyCallback = (_, hasMinimizedNow) =>
            {
                OnMinimizeOrRestore(hasMinimizedNow);
            };
            GLFW.SetWindowIconifyCallback(GlfwWindow, _iconifyCallback);

            _maximizeCallback = (_, hasMaximizedNow) =>
            {
                OnMaximizeOrRestore(hasMaximizedNow);
            };
            GLFW.SetWindowMaximizeCallback(GlfwWindow, _maximizeCallback);

            _focusCallback = (_, isFocusingNow) =>
            {
                if (isFocusingNow)
                {
                    if (Document.CurrentAppState == UiDocument.AppState.Hidden)
                    {
                        DocumentInvoke("WndSetAppState", UiDocument.AppState.Inactive);
                    }

                    if (Document.CurrentAppState == UiDocument.AppState.Inactive)
                    {
                        DocumentInvoke("WndSetAppState", UiDocument.AppState.Active);
                    }
                }
                else
                {
                    DocumentInvoke("WndSetAppState", UiDocument.AppState.Inactive);
                }
            };
            GLFW.SetWindowFocusCallback(GlfwWindow, _focusCallback);

            _refreshCallback = _ =>
            {
                Monitor* monitor = GLFW.GetWindowMonitor(GlfwWindow);
                if (monitor == null)
                {
                    if (GLFW.GetWindowAttrib(GlfwWindow, WindowAttributeGetBool.Iconified))
                    {
                        LastSetWindowMode = WindowMode.Minimized;
                    }
                    else if (GLFW.GetWindowAttrib(GlfwWindow, WindowAttributeGetBool.Maximized))
                    {
                        LastSetWindowMode = WindowMode.Maximized;
                    }
                    else
                    {
                        LastSetWindowMode = WindowMode.Windowed;
                    }
                }
            };
            GLFW.SetWindowRefreshCallback(GlfwWindow, _refreshCallback);

            #endregion


            #region Cursor events

            _cursorMoveCallback = (_, posX, posY) =>
            {
                float positionX = (float)UtilDenormalizeGlfwWm(posX);
                float positionY = (float)UtilDenormalizeGlfwWm(posY);
                Point2D pos = new(positionX, positionY);
                bool pressed = (Document.PressedMouseButtons & MouseButtonType.Primary) != 0;

                InputPointer? pointer = Document.GetPointerByDeviceType(InputPointer.InputDeviceType.Mouse);
                if (pointer == null)
                {
                    return;
                }

                DocumentInvoke(
                    "WndAddOrUpdatePointer",
                    new InputPointer(pos, pressed, pointer.PointerId, InputPointer.InputDeviceType.Mouse));

                Document.SimulatePointerMove(
                    new PointerMoveEventArgs(
                        pos,
                        pos,
                        positionX - _lastMouseX,
                        positionY - _lastMouseY,
                        pressed,
                        0));

                _lastMouseX = positionX;
                _lastMouseY = positionY;
            };
            GLFW.SetCursorPosCallback(GlfwWindow, _cursorMoveCallback);

            _cursorEnterOrExitCallback = (_, entered) =>
            {
                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                x = UtilDenormalizeGlfwWm(x);
                y = UtilDenormalizeGlfwWm(y);

                Point2D pos = new((float)x, (float)y);
                bool pressed = (Document.PressedMouseButtons & MouseButtonType.Primary) != 0;

                if (entered)
                {
                    DocumentInvoke(
                        "WndAddOrUpdatePointer",
                        new InputPointer(pos, pressed, 0, InputPointer.InputDeviceType.Mouse));

                    Document.SimulatePointerEnter(
                        new PointerEnterEventArgs(pos, pos, pressed, 0));
                }
                else
                {
                    //the cursor position when exiting the window is very different from platform to platform,
                    // so we try to standardize it by setting the "exiting edge" to 1 unit outside the window area;
                    // this doesn't do the trick for Wayland, though
                    bool wasModified = false;
                    if (x <= 0 || Math.Abs(x) <= 0.5)
                    {
                        x = -1;
                        wasModified = true;
                    }

                    if (y <= 0 || Math.Abs(y) <= 0.5)
                    {
                        y = -1;
                        wasModified = true;
                    }

                    GLFW.GetWindowSize(GlfwWindow, out int windowWidth, out int windowHeight);

                    if (x >= windowWidth || Math.Abs(windowWidth - x) <= 0.5)
                    {
                        x = windowWidth + 1;
                        wasModified = true;
                    }

                    if (y >= windowHeight || Math.Abs(windowHeight - y) <= 0.5)
                    {
                        y = windowHeight + 1;
                        wasModified = true;
                    }

                    if (wasModified)
                    {
                        pos = new Point2D((float)x, (float)y);
                    }

                    InputPointer? pointer = Document.GetPointerByDeviceType(InputPointer.InputDeviceType.Mouse);
                    Document.SimulatePointerExit(
                        new PointerExitEventArgs(pos, pos, pressed, pointer?.PointerId ?? -1));

                    DocumentInvoke("WndRemovePointer", pointer?.PointerId ?? -1);
                }
            };
            GLFW.SetCursorEnterCallback(GlfwWindow, _cursorEnterOrExitCallback);

            _mouseButtonCallback = (_, glfwMouseBtn, action, _) =>
            {
                //there's a 1:1 correspondence between GLFW button index and our MouseButtonType
                var button = (MouseButtonType)(1 << (int)glfwMouseBtn);
                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                x = UtilDenormalizeGlfwWm(x);
                y = UtilDenormalizeGlfwWm(y);

                Point2D pos = new((float)x, (float)y);

                Document.SimulateMouseButton(
                    new MouseButtonEventArgs(
                        pos,
                        pos,
                        button,
                        action == InputAction.Press,
                        0));

                if (button != MouseButtonType.Primary)
                {
                    return;
                }

                if (action == InputAction.Press)
                {
                    Document.SimulatePointerDown(
                        new PointerDownEventArgs(pos, pos, 0));
                }
                else
                {
                    Document.SimulatePointerUp(
                        new PointerUpEventArgs(pos, pos, 0));
                }
            };
            GLFW.SetMouseButtonCallback(GlfwWindow, _mouseButtonCallback);

            _mouseScrollCallback = (_, deltaX, deltaY) =>
            {
                //TODO: use platform-specific APIs for better scrolling; this should only be the fallback if native
                // APIs fail for some reason, except on Wayland, where this interface works properly

                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                x = UtilDenormalizeGlfwWm(x);
                y = UtilDenormalizeGlfwWm(y);

                Point2D pos = new((float)x, (float)y);

                Document.SimulateMouseWheel(
                    new MouseWheelEventArgs(
                        pos,
                        pos,
                        //this isn't standardized, so we'll just use a generic 60 px scroll
                        (float)(deltaX == 0 ? deltaX : -deltaX) * 60,
                        (float)(deltaY == 0 ? deltaY : -deltaY) * 60,
                        (Document.PressedMouseButtons & MouseButtonType.Middle) != 0,
                        0));
            };
            GLFW.SetScrollCallback(GlfwWindow, _mouseScrollCallback);

            #endregion


            #region Keyboard events

            _keyCallback = (_, key, scancode, action, modifiers) =>
            {
                // string actionString =
                //     action switch
                //     {
                //         InputAction.Press => "Pressed",
                //         InputAction.Release => "Release",
                //         _ => "Repeat"
                //     };
                // CatLogger.LogDebug($"Key: {key}, {actionString}, Modifiers: {modifiers}");

                Document.SimulatePhysicalKeyEvent(
                    KeyEventDispatcher(key, scancode, action, modifiers));
            };
            GLFW.SetKeyCallback(GlfwWindow, _keyCallback);

            _charCallback = (_, codepoint) =>
            {
                Document.SimulateCharacterTyped(new CharTypedEventArgs((char)codepoint));
            };
            GLFW.SetCharCallback(GlfwWindow, _charCallback);

            #endregion
        }

        private void UnregisterCallbacks()
        {
            ResizedEvent = null;
            _resizeCallback = null;
            GLFW.SetWindowSizeCallback(GlfwWindow, null);

            _contentScaleCallback = null;
            GLFW.SetWindowContentScaleCallback(GlfwWindow, null);

            _iconifyCallback = null;
            GLFW.SetWindowIconifyCallback(GlfwWindow, null);

            _maximizeCallback = null;
            GLFW.SetWindowMaximizeCallback(GlfwWindow, null);

            _focusCallback = null;
            GLFW.SetWindowFocusCallback(GlfwWindow, null);

            _refreshCallback = null;
            GLFW.SetWindowRefreshCallback(GlfwWindow, null);

            _cursorMoveCallback = null;
            GLFW.SetCursorPosCallback(GlfwWindow, null);

            _cursorEnterOrExitCallback = null;
            GLFW.SetCursorEnterCallback(GlfwWindow, null);

            _mouseButtonCallback = null;
            GLFW.SetMouseButtonCallback(GlfwWindow, null);

            _mouseScrollCallback = null;
            GLFW.SetScrollCallback(GlfwWindow, null);

            _keyCallback = null;
            GLFW.SetKeyCallback(GlfwWindow, null);

            _charCallback = null;
            GLFW.SetCharModsCallback(GlfwWindow, null);
        }

        private KeyEventArgs KeyEventDispatcher(
            Keys key,
            int scancode,
            InputAction action,
            OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers modifiers)
        {
            PhysicalKey physicalKey = key == Keys.Unknown ? PhysicalKey.Unknown : (PhysicalKey)key;
            var keyAction = (KeyAction)action;
            var keyModifiers = KeyModifiers.None;

            //we want to override GLFW's behavior of putting the key itself in the modifiers on release,
            //but we need to make sure the other modifier key for the same function (e.g. Left Shift, Right Shift)
            //is not already pressed
            //if (keyAction == KeyAction.Released && physicalKey.IsModifierKey())
            if (physicalKey.IsModifierKey())
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (physicalKey)
                {
                    //if this key is released, but the equivalent on the other side is not, then we include
                    //the modifier, otherwise not; for single modifiers, we simply don't include the modifier (as it
                    //is this one and is being released)
                    case PhysicalKey.LeftShift
                        when GLFW.GetKey(GlfwWindow, Keys.RightShift) != InputAction.Release:
                    case PhysicalKey.RightShift
                        when GLFW.GetKey(GlfwWindow, Keys.LeftShift) != InputAction.Release:
                        keyModifiers = KeyModifiers.Shift;
                        break;
                    case PhysicalKey.LeftControl
                        when GLFW.GetKey(GlfwWindow, Keys.RightControl) != InputAction.Release:
                    case PhysicalKey.RightControl
                        when GLFW.GetKey(GlfwWindow, Keys.LeftControl) != InputAction.Release:
                        keyModifiers = KeyModifiers.Control;
                        break;
                    case PhysicalKey.LeftAlt
                        when GLFW.GetKey(GlfwWindow, Keys.RightAlt) != InputAction.Release:
                    case PhysicalKey.RightAlt
                        when GLFW.GetKey(GlfwWindow, Keys.LeftAlt) != InputAction.Release:
                        keyModifiers = KeyModifiers.Alt;
                        break;
                    case PhysicalKey.LeftSuper
                        when GLFW.GetKey(GlfwWindow, Keys.RightSuper) != InputAction.Release:
                    case PhysicalKey.RightSuper
                        when GLFW.GetKey(GlfwWindow, Keys.LeftSuper) != InputAction.Release:
                        keyModifiers = KeyModifiers.Super;
                        break;
                }
            }

            //if it is the same modifier as the one from GLFW, we skip it because we treated this case above 
            if (
                (modifiers & OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.Shift) != 0
             && physicalKey != PhysicalKey.LeftShift && physicalKey != PhysicalKey.RightShift)
            {
                keyModifiers |= KeyModifiers.Shift;
            }

            if (
                (modifiers & OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.Control) != 0
             && physicalKey != PhysicalKey.LeftControl && physicalKey != PhysicalKey.RightControl)
            {
                keyModifiers |= KeyModifiers.Control;
            }

            if (
                (modifiers & OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.Alt) != 0
             && physicalKey != PhysicalKey.LeftAlt && physicalKey != PhysicalKey.RightAlt)
            {
                keyModifiers |= KeyModifiers.Alt;
            }

            if (
                (modifiers & OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.Super) != 0
             && physicalKey != PhysicalKey.LeftSuper && physicalKey != PhysicalKey.RightSuper)
            {
                keyModifiers |= KeyModifiers.Super;
            }

            if (
                (modifiers & OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.CapsLock) != 0
             && physicalKey != PhysicalKey.CapsLock)
            {
                keyModifiers |= KeyModifiers.CapsLock;
            }

            if (
                (modifiers & OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.NumLock) != 0
             && physicalKey != PhysicalKey.NumLock)
            {
                keyModifiers |= KeyModifiers.NumLock;
            }

            Keys translatedGlfwKey = TranslateGlfwKey(key, scancode);
            PhysicalKey translatedKey =
                translatedGlfwKey == Keys.Unknown
                    ? PhysicalKey.Unknown
                    : (PhysicalKey)translatedGlfwKey;
            return new KeyEventArgs(translatedKey, physicalKey, keyModifiers, keyAction);
        }

        /// <summary>
        /// GLFW standardizes the keys by default, so it uses the US ANSI 104 keyboard layout. We try to translate
        /// these keys so that it uses the user's keyboard layout, very useful for shortcuts.
        /// </summary>
        /// <param name="glfwKey"></param>
        /// <param name="scancode"></param>
        /// <returns></returns>
        private static Keys TranslateGlfwKey(Keys glfwKey, int scancode)
        {
            if (glfwKey >= Keys.KeyPad0 && glfwKey <= Keys.KeyPadEqual)
            {
                return glfwKey;
            }

            string? keyName = GLFW.GetKeyName(glfwKey, scancode);
            // ReSharper disable once MergeIntoNegatedPattern
            if (keyName == null || keyName.Length != 1)
            {
                return glfwKey;
            }

            const string charNames = "`-=[]\\,;\'./";
            Keys[] charGlfwKeys =
            [
                Keys.GraveAccent, Keys.Minus, Keys.Equal, Keys.LeftBracket, Keys.RightBracket, Keys.Backslash,
                Keys.Comma, Keys.Semicolon, Keys.Apostrophe, Keys.Period, Keys.Slash
            ];

            char pressedChar = keyName[0];
            switch (pressedChar)
            {
                case >= '0' and <= '9':
                    return Keys.D0 + (pressedChar - '0');
                case >= 'A' and <= 'Z':
                    return Keys.A + (pressedChar - 'A');
                case >= 'a' and <= 'z':
                    return Keys.A + (pressedChar - 'a');
            }

            int charIdx = charNames.IndexOf(pressedChar);
            if (charIdx > 0 && charIdx < charNames.Length)
            {
                return charGlfwKeys[charIdx];
            }

            return glfwKey;
        }


        #region Callback functions

        private void OnResize(object sender, WindowResizedEventArgs e)
        {
            //set window size
            _width = e.NewWidth;
            _height = e.NewHeight;

            DocumentInvoke(
                "WndSetViewportSize",
                new Size((int)(_width * Document.ContentScale), (int)(_height * Document.ContentScale)));
            GraphicsBackend?.Resized(
                (int)(_width * Document.ContentScale), (int)(_height * Document.ContentScale));

            Document.Renderer.SetCanvasDirty();
            DoFrameActions();
        }

        private void OnMinimizeOrRestore(bool hasMinimizedNow)
        {
            CurrentWindowMode = hasMinimizedNow ? WindowMode.Minimized : LastSetWindowMode;

            if (hasMinimizedNow)
            {
                if (Document.CurrentAppState == UiDocument.AppState.Active)
                {
                    DocumentInvoke("WndSetAppState", UiDocument.AppState.Inactive);
                    DocumentInvoke("WndSetAppState", UiDocument.AppState.Hidden);
                }
                else if (Document.CurrentAppState == UiDocument.AppState.Inactive)
                {
                    DocumentInvoke("WndSetAppState", UiDocument.AppState.Hidden);
                }
            }
            else
            {
                DocumentInvoke("WndSetAppState", UiDocument.AppState.Inactive);
                DocumentInvoke("WndSetAppState", UiDocument.AppState.Active);
            }

            WindowModeChangedEvent?.Invoke(
                this,
                hasMinimizedNow
                    ? new WindowModeChangedEventArgs(WindowMode.Minimized, LastSetWindowMode)
                    : new WindowModeChangedEventArgs(LastSetWindowMode, WindowMode.Minimized));

            //This is a workaround for some window managers/display servers like KWin that will show the window framebuffer
            //as transparent after minimizing or restoring until a redrawing happens.

            // GL.GetInteger(GetPName.FramebufferBinding, out int frame);
            // GL.GetInteger(GetPName.StencilBits, out int stencil);
            // GL.GetInteger(GetPName.Samples, out int samples);
            // SetHwFramebufferData(frame, stencil, samples);

            Document.Renderer.SetCanvasDirty();
            DoFrameActions();
        }

        private void OnMaximizeOrRestore(bool hasMaximizedNow)
        {
            if (!_canInvokeMaximize)
            {
                return;
            }

            CurrentWindowMode = hasMaximizedNow ? WindowMode.Maximized : WindowMode.Windowed;

            WindowModeChangedEvent?.Invoke(
                this,
                hasMaximizedNow
                    ? new WindowModeChangedEventArgs(WindowMode.Maximized, WindowMode.Windowed)
                    : new WindowModeChangedEventArgs(WindowMode.Windowed, WindowMode.Maximized));
        }

        #endregion

        /// <summary>
        /// Normalizes the data from the GLFW window manager depending on the runtime platform. On some platforms, GLFW
        /// returns data as direct pixels, so it doesn't take into account scaling, while on others, the data is
        /// already taking scale into account. This makes all data take scale into account.
        /// </summary>
        /// <remarks>Use this when you always want normalized/scaled dimensions.</remarks>
        /// <param name="data">The input data to normalize.</param>
        /// <returns>The result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int UtilNormalizeGlfwWm(int data)
        {
            if (
                GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11
             || GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.Win32)
            {
                return (int)(data / Document.ContentScale);
            }

            return data;
        }

        /// <inheritdoc cref="UtilNormalizeGlfwWm(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal double UtilNormalizeGlfwWm(double data)
        {
            if (
                GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11
             || GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.Win32)
            {
                return data / Document.ContentScale;
            }

            return data;
        }

        /// <summary>
        /// De-normalizes the data from the GLFW window manager depending on the runtime platform. It's the exact
        /// opposite of <see cref="UtilNormalizeGlfwWm(int)"/>, as this converts anything by removing scaling, therefore
        /// making data indicate pixels instead of normalized coordinates.
        /// </summary>
        /// <remarks>Use this when you always want direct pixel dimensions.</remarks>
        /// <param name="data">The input data to de-normalize.</param>
        /// <returns>The result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int UtilDenormalizeGlfwWm(int data)
        {
            if (
                GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11
             || GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.Win32)
            {
                return data;
            }

            return (int)(data * Document.ContentScale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal double UtilDenormalizeGlfwWm(double data)
        {
            if (
                GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11
             || GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.Win32)
            {
                return data;
            }

            return data * Document.ContentScale;
        }
    }
}
