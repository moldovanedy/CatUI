using CatUI.Data;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements;
using OpenTK.Graphics.OpenGL;
using CatUI.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Windowing.Desktop
{
    public unsafe partial class DesktopWindow
    {
        private void RegisterCallbacks()
        {
            _resizeCallback = (_, newWidth, newHeight) =>
            {
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

            _cursorMoveCallback = (_, posX, posY) =>
            {
                float positionX = (float)posX;
                float positionY = (float)posY;
                Point2D pos = new(positionX, positionY);
                bool pressed = (Document.PressedMouseButtons & MouseButtonType.Primary) != 0;

                Document.SimulatePointerMove(
                    new PointerMoveEventArgs(
                        pos,
                        pos,
                        positionX - _lastMouseX,
                        positionY - _lastMouseY,
                        pressed));

                _lastMouseX = positionX;
                _lastMouseY = positionY;
            };
            GLFW.SetCursorPosCallback(GlfwWindow, _cursorMoveCallback);

            _cursorEnterOrExitCallback = (_, entered) =>
            {
                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                Point2D pos = new((float)x, (float)y);
                bool pressed = (Document.PressedMouseButtons & MouseButtonType.Primary) != 0;

                if (entered)
                {
                    Document.SimulatePointerEnter(
                        new PointerEnterEventArgs(pos, pos, pressed));
                }
                else
                {
                    Document.SimulatePointerExit(
                        new PointerExitEventArgs(pos, pos, pressed));
                }
            };
            GLFW.SetCursorEnterCallback(GlfwWindow, _cursorEnterOrExitCallback);

            _mouseButtonCallback = (_, glfwMouseBtn, action, _) =>
            {
                //there's a 1:1 correspondence between GLFW button index and our MouseButtonType
                var button = (MouseButtonType)(1 << (int)glfwMouseBtn);
                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                Point2D pos = new((float)x, (float)y);

                Document.SimulateMouseButton(
                    new MouseButtonEventArgs(
                        pos,
                        pos,
                        button,
                        action == InputAction.Press));

                if (button != MouseButtonType.Primary)
                {
                    return;
                }

                if (action == InputAction.Press)
                {
                    Document.SimulatePointerDown(
                        new PointerDownEventArgs(pos, pos));
                }
                else
                {
                    Document.SimulatePointerUp(
                        new PointerUpEventArgs(pos, pos));
                }
            };
            GLFW.SetMouseButtonCallback(GlfwWindow, _mouseButtonCallback);

            _mouseScrollCallback = (_, deltaX, deltaY) =>
            {
                //TODO: both mouse and touchpad generate only -1 and 1 (at least on Linux), so we need to somehow
                // detect if it's a mouse or touchpad and scale the mouse accordingly or use platform-specific APIs

                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                Point2D pos = new((float)x, (float)y);

                Document.SimulateMouseWheel(
                    new MouseWheelEventArgs(
                        pos,
                        pos,
                        (float)(deltaX == 0 ? deltaX : -deltaX) * 10,
                        (float)(deltaY == 0 ? deltaY : -deltaY) * 10,
                        (Document.PressedMouseButtons & MouseButtonType.Middle) != 0));
            };
            GLFW.SetScrollCallback(GlfwWindow, _mouseScrollCallback);
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
        }


        #region Callback functions

        private void OnResize(object sender, WindowResizedEventArgs e)
        {
            //GL.Viewport(0, 0, width, height);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            _width = e.NewWidth;
            _height = e.NewHeight;

            GL.GetInteger(GetPName.FramebufferBinding, out int frame);
            GL.GetInteger(GetPName.StencilBits, out int stencil);
            GL.GetInteger(GetPName.Samples, out int samples);
            Document.Renderer.SetFramebufferData(frame, stencil, samples);

            DocumentInvoke("WndSetViewportSize", new Size(e.NewWidth, e.NewHeight));
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

            GL.GetInteger(GetPName.FramebufferBinding, out int frame);
            GL.GetInteger(GetPName.StencilBits, out int stencil);
            GL.GetInteger(GetPName.Samples, out int samples);
            Document.Renderer.SetFramebufferData(frame, stencil, samples);

            Document.Renderer.SetCanvasDirty();
            DoFrameActions();

// #if CAT_USE_ANGLE
//             Egl.SwapBuffers(_eglDisplay, _eglSurface);
// #else
//             GLFW.SwapBuffers(GlfwWindow);
// #endif
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
    }
}
