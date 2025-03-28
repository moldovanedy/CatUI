using System;
using System.Collections.Generic;
using System.Reflection;
using CatUI.Data;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements;
using CatUI.Windowing.Common;
using OpenTK;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Windowing.Desktop
{
    /// <summary>
    /// Represents a window on a desktop platform. On desktop, your app can create multiple windows and can generally
    /// set their size, position on the display etc.
    /// </summary>
    public unsafe class DesktopWindow : IApplicationWindow
    {
        /// <summary>
        /// Represents the pointer to the platform's window representation. You can use this to implement platform-specific
        /// functionality which almost always require a window handle. If you use this, you are responsible for the
        /// functionality you use it for; misusing this might cause random crashes. 
        /// </summary>
        /// <remarks>
        /// On Windows, it returns the Win32 HWND. On macOS, it will return the Cocoa window handle. On Linux,
        /// it returns the X11 or Wayland window pointer, depending on which platform it runs on.
        /// </remarks>
        public nint NativeHandle
        {
            get
            {
                if (GlfwWindow == null)
                {
                    return 0;
                }

                if (OperatingSystem.IsWindows())
                {
                    return GLFW.GetWin32Window(GlfwWindow);
                }

                if (OperatingSystem.IsLinux())
                {
                    if (GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11)
                    {
                        return (nint)(nuint)GLFW.GetX11Window(GlfwWindow);
                    }

                    return GLFW.GetWaylandWindow(GlfwWindow);
                }

                if (OperatingSystem.IsMacOS())
                {
                    return GLFW.GetCocoaWindow(GlfwWindow);
                }

                return 0;
            }
        }

        /// <summary>
        /// Represents the window's document. All elements that will appear on this window must be part of this document.
        /// </summary>
        public UiDocument Document { get; }

        /// <summary>
        /// Represents the pointer to the GLFW window representation. Use this if you want to implement something using
        /// GLFW (this is GLFWWindow*). This is only usable inside unsafe code.
        /// </summary>
        internal Window* GlfwWindow { get; private set; }

        private bool _shouldCloseWindow;
        private readonly WindowFlags _flags;

#if USE_ANGLE
        private nint _eglDisplay;
        private nint _eglSurface;
        private nint _eglContext;
#endif

        private GLFWCallbacks.WindowSizeCallback? _resizeCallback;
        private GLFWCallbacks.WindowIconifyCallback? _iconifyCallback;
        private GLFWCallbacks.WindowMaximizeCallback? _maximizeCallback;
        private GLFWCallbacks.WindowRefreshCallback? _refreshCallback;

        private GLFWCallbacks.CursorPosCallback? _cursorMoveCallback;
        private GLFWCallbacks.CursorEnterCallback? _cursorEnterOrExitCallback;
        private GLFWCallbacks.MouseButtonCallback? _mouseButtonCallback;
        private GLFWCallbacks.ScrollCallback? _mouseScrollCallback;

        private float _lastMouseX;
        private float _lastMouseY;
        private bool _canInvokeMaximize = true;

        #region Properties

        /// <summary>
        /// Represents the window's width in desktop coordinates (meaning it is scaled with the platform's display scale,
        /// unless you didn't set <see cref="WindowFlags.DpiAware"/>, in which case all coordinates are physical pixels).
        /// Setting this will resize the window to that value, but will be restricted to <see cref="MaxWidth"/> and
        /// <see cref="MinWidth"/>. If you want the physical pixel width, see <see cref="FramebufferWidth"/>.
        /// </summary>
        /// <remarks>
        /// When the <see cref="CurrentWindowMode"/> is <see cref="WindowMode.ExclusiveFullscreen"/>, this is not reliable
        /// (if the window was in other mode before, this will be the width from that mode). When it's
        /// <see cref="WindowMode.Fullscreen"/>, it is the display's width. This does NOT take into account the window
        /// decorations.
        /// </remarks>
        public int Width
        {
            get => _width;
            set
            {
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowSize(GlfwWindow, value, _height);
                }

                _width = value;
            }
        }

        private int _width;

        /// <summary>
        /// Represents the window's height in desktop coordinates (meaning it is scaled with the platform's display scale,
        /// unless you didn't set <see cref="WindowFlags.DpiAware"/>, in which case all coordinates are physical pixels).
        /// Setting this will resize the window to that value, but will be restricted to <see cref="MaxHeight"/> and
        /// <see cref="MinHeight"/>. If you want the physical pixel height, see <see cref="FramebufferHeight"/>.
        /// </summary>
        /// <remarks>
        /// When the <see cref="CurrentWindowMode"/> is <see cref="WindowMode.ExclusiveFullscreen"/>, this is not reliable
        /// (if the window was in other mode before, this will be the height from that mode). When it's
        /// <see cref="WindowMode.Fullscreen"/>, it is the display's height. This does NOT take into account the window
        /// decorations.
        /// </remarks>
        public int Height
        {
            get => _height;
            set
            {
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowSize(GlfwWindow, _width, value);
                }

                _height = value;
            }
        }

        private int _height;

        /// <summary>
        /// Represents the physical pixel width, meaning that this value is not scaled with the platform's display
        /// scaling. This does NOT take into account the window decorations.
        /// </summary>
        public int FramebufferWidth
        {
            get
            {
                if (GlfwWindow == null)
                {
                    return 0;
                }

                GLFW.GetFramebufferSize(GlfwWindow, out int width, out _);
                return width;
            }
        }

        /// <summary>
        /// Represents the physical pixel height, meaning that this value is not scaled with the platform's display
        /// scaling. This does NOT take into account the window decorations.
        /// </summary>
        public int FramebufferHeight
        {
            get
            {
                if (GlfwWindow == null)
                {
                    return 0;
                }

                GLFW.GetFramebufferSize(GlfwWindow, out _, out int height);
                return height;
            }
        }

        /// <summary>
        /// If true, it means that the <see cref="WindowFlags.DpiAware"/> was set.
        /// </summary>
        public bool IsDpiAware => (_flags & WindowFlags.DpiAware) != 0;


        /// <summary>
        /// It represents the minimum width that the window can have, either resized by the user or by your code.
        /// Like <see cref="Width"/> it is scaled by the platform if <see cref="WindowFlags.DpiAware"/> is set.
        /// If the current width is smaller than the given value, the window will be resized. This does NOT take into
        /// account the window decorations.
        /// </summary>
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowSizeLimits(GlfwWindow, value, _minHeight, _maxWidth, _maxHeight);
                }

                _minWidth = value;
            }
        }

        private int _minWidth;

        /// <summary>
        /// It represents the minimum height that the window can have, either resized by the user or by your code.
        /// Like <see cref="Height"/> it is scaled by the platform if <see cref="WindowFlags.DpiAware"/> is set.
        /// If the current height is smaller than the given value, the window will be resized. This does NOT take into
        /// account the window decorations.
        /// </summary>
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, value, _maxWidth, _maxHeight);
                }

                _minHeight = value;
            }
        }

        private int _minHeight;

        /// <summary>
        /// It represents the maximum width that the window can have, either resized by the user or by your code.
        /// Like <see cref="Width"/> it is scaled by the platform if <see cref="WindowFlags.DpiAware"/> is set.
        /// If the current width is larger than the given value, the window will be resized. This does NOT take into
        /// account the window decorations.
        /// </summary>
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, _minHeight, value, _maxHeight);
                }

                _maxWidth = value;
            }
        }

        private int _maxWidth;

        /// <summary>
        /// It represents the maximum height that the window can have, either resized by the user or by your code.
        /// Like <see cref="Height"/> it is scaled by the platform if <see cref="WindowFlags.DpiAware"/> is set.
        /// If the current height is larger than the given value, the window will be resized. This does NOT take into
        /// account the window decorations.
        /// </summary>
        public int MaxHeight
        {
            get => _maxHeight;
            set
            {
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, _minHeight, _maxHeight, value);
                }

                _maxHeight = value;
            }
        }

        private int _maxHeight;

        /// <summary>
        /// Represents the window title.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (GlfwWindow != null)
                {
                    GLFW.SetWindowTitle(GlfwWindow, _title);
                }
            }
        }

        private string _title;

        /// <summary>
        /// Represents the current window mode. To set it, see <see cref="SetWindowMode"/>.
        /// </summary>
        public WindowMode CurrentWindowMode { get; private set; }

        /// <summary>
        /// Represents the window mode last set by you in <see cref="SetWindowMode"/> or at window creation.
        /// </summary>
        public WindowMode LastSetWindowMode { get; private set; }

        /// <summary>
        /// <para>
        /// This is the maximum number of frames per second the application is allowed to run. Lower values (like 30) 
        /// will make the application use less resources but will have some "lag", as the visuals will look more sluggish.
        /// Higher values will reduce visual "lag", but will utilize more CPU and GPU, thus potentially making the system slower.
        /// The default value is -1, which means this is ignored, and the <see cref="SwapInterval"/> will be respected.
        /// </para>
        /// <para>
        /// Unless you have a good reason, you should always use <see cref="SwapInterval"/> instead of this, because this
        /// property achieves the lower FPS by introducing artificial delays (with Thread.Sleep) which will block the
        /// main thread for some time. <see cref="SwapInterval"/> uses the OS facilities to reduce the FPS, which is much better.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is NOT guaranteed that this FPS will be reached, as this depends on both the client's hardware and
        /// on your code for creating the UI.
        /// </para>
        /// <para>
        /// At any given moment, only one of <see cref="MaxFps"/> and <see cref="SwapInterval"/> will be respected,
        /// this will be respected if and only if <see cref="SwapInterval"/> is set to -1.
        /// </para>
        /// </remarks>
        public int MaxFps { get; set; } = -1;

        /// <summary>
        /// <para>
        /// It specifies the number of display vertical "blanks" to wait for until rendering the next frame.
        /// The default value is 1 and is essentially V-Sync enabled, which is the best setting for most apps.
        /// A value of 0 means that the app will try to render the frames as fast as possible (when there are changes, of course),
        /// which might cause "screen tearing".
        /// </para>
        /// <para>
        /// It is highly recommended to use this instead of <see cref="MaxFps"/> because it uses the OS mechanisms for
        /// achieving a lower frame rate (to reduce power consumption). A value over 1 effectively means the display
        /// frame rate divided by this value (so for a 60 Hz display a value of 2 means 30 FPS, 4 means 15 FPS etc.).
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is NOT guaranteed that this setting will be respected, because it is entirely dependent on the
        /// client's hardware and drivers for the GPU.
        /// </para>
        /// <para>
        /// At any given moment, only one of <see cref="MaxFps"/> and <see cref="SwapInterval"/> will be respected,
        /// this has precedence over <see cref="MaxFps"/> and will be respected regardless of <see cref="MaxFps"/>
        /// unless this is -1.
        /// </para>
        /// </remarks>
        public int SwapInterval
        {
            get => _swapInterval;
            set
            {
                _swapInterval = value;
                GLFW.SwapInterval(_swapInterval);
            }
        }

        private int _swapInterval = 1;

        #endregion

        #region Events

        /// <summary>
        /// Fired when an internal GLFW error occured. GLFW is the windowing library that CatUI uses on desktop to
        /// create windows, manage input and handle hardware graphics context (OpenGL).
        /// </summary>
        public event Action<OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode, string>? ErrorOccurred;

        /// <summary>
        /// This function will be called when the user, the platform or your code tries to close the window. You can
        /// set this to any function that returns a boolean: if it returns true, the window will be closed and its
        /// resources freed, if it returns false the close request is ignored.
        /// </summary>
        /// <remarks>
        /// This is useful for prompting the user to do some action (e.g. save the file or the modified settings).
        /// Beware that the user or the platform might still be able to close the window forcefully without this function
        /// being called.
        /// </remarks>
        public Func<bool> OnCloseRequested { get; set; } = () => true;

        /// <summary>
        /// Fired when the window is resized, either by the user, by the platform or by your code.
        /// </summary>
        public event WindowResizedEventHandler? ResizedEvent;

        /// <summary>
        /// Fired when the window mode (i.e. windowed, maximized, minimized, or full-screen) is changed either by the user,
        /// by the platform or by your code. To enter full-screen (simple or exclusive), only your code can trigger this
        /// event by calling <see cref="SetWindowMode"/>.
        /// </summary>
        public event WindowModeChangedEventHandler? WindowModeChangedEvent;

        #endregion

        /// <summary>
        /// Represents the window startup flags. These are a bitmap that must be set in the constructor, before the call
        /// to <see cref="DesktopWindow.Open"/> and can not be modified afterward, however some properties (like
        /// the window visibility controlled by <see cref="Visible"/> here) can still be controlled by code.
        /// </summary>
        [Flags]
        public enum WindowFlags
        {
            /// <summary>
            /// Represents the default value for the flags (<see cref="Resizable"/>, <see cref="Visible"/>,
            /// <see cref="Decorated"/>, <see cref="DpiAware"/> and <see cref="Focused"/>).
            /// </summary>
            Default = 0b11111,

            /// <summary>
            /// Indicates that the window is resizable by the user. Even if this is unset, you can generally set the
            /// size from code using <see cref="DesktopWindow.Width"/> and <see cref="DesktopWindow.Height"/>.
            /// </summary>
            Resizable = 1,

            /// <summary>
            /// Sets the window to be visible. Otherwise, it will be hidden.
            /// </summary>
            Visible = 2,

            /// <summary>
            /// Makes the window have the platform's decorations like borders, the title bar and the control buttons. 
            /// </summary>
            Decorated = 4,

            /// <summary>
            /// Indicates that the window's scale will be automatically controlled by CatUI and will correspond to
            /// the platform preferences. It is highly recommended to set this flag. It is enabled by default.
            /// </summary>
            DpiAware = 8,

            /// <summary>
            /// Focuses the window so that the user can interact with it directly. 
            /// </summary>
            Focused = 16,

            /// <summary>
            /// Makes the window float above other windows even if it's not focused or that other windows are maximized.
            /// This should generally be a setting controlled by the user, as enabling this without the user to be able
            /// to disable this behavior might result in a bad UX (note that some window managers might be able to
            /// override this behavior). 
            /// </summary>
            AlwaysOnTop = 32,

            /// <summary>
            /// Makes the window have a transparent background so it can allow the user to see behind this window.
            /// This might be useful for implementing widget-like apps (like a clock) or splash screens.
            /// </summary>
            TransparentFramebuffer = 64
        }

        public enum WindowMode
        {
            /// <summary>
            /// The window occupies a certain position and is visible.
            /// </summary>
            Windowed = 0,

            /// <summary>
            /// The window is minimized in the platform's taskbar. It is not visible.
            /// </summary>
            Minimized = 1,

            /// <summary>
            /// The window is maximized; it occupies the entire screen, except window decorations and optionally the
            /// taskbar (depends on the platform and the user settings).
            /// </summary>
            Maximized = 2,

            /// <summary>
            /// The window is in full-screen, a.k.a. borderless full-screen or windowed full-screen. This is similar
            /// to <see cref="Maximized"/>, but the window occupies the entire screen, no decorations or taskbar,
            /// but the user can easily switch to other apps because the display's video mode is unaffected. This is
            /// recommended if you want full-screen.
            /// </summary>
            Fullscreen = 3,

            /// <summary>
            /// The window is in full-screen, but it alters the display's video mode, makes switching to other apps
            /// a bit harder and, in some rare situations, can cause GPU crashes more easily when switching. If you
            /// want full-screen, consider using <see cref="Fullscreen"/> instead, as it's more stable and easier.
            /// If the window loses focus in this mode, it is automatically minimized.
            /// </summary>
            ExclusiveFullscreen = 4
        }

        private double _lastTime;
        private readonly List<Action<double>> _animationFrameCallbacks = new();

        /// <summary>
        /// Fired when the window is "dirty" and it needs a repaint, either partially or fully. This is fired before the
        /// redraw.
        /// </summary>
        public event Action<double>? FrameUpdatedEvent;

        #region Object creation

        public DesktopWindow(
            int width = 800,
            int height = 600,
            string title = "",
            int minWidth = 50,
            int maxWidth = ushort.MaxValue,
            int minHeight = 50,
            int maxHeight = ushort.MaxValue,
            WindowFlags windowFlags = WindowFlags.Default,
            WindowMode startupMode = WindowMode.Windowed)
        {
            _width = width;
            _height = height;
            _title = title;
            _minWidth = minWidth;
            _maxWidth = maxWidth;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
            _flags = windowFlags;
            CurrentWindowMode = startupMode;
            LastSetWindowMode = CurrentWindowMode;

            if (!GLFW.Init())
            {
                OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode errorCode = GLFW.GetError(out string description);
                throw new Exception($"Internal GLFW error ({errorCode}): {description}");
            }

            //TODO: this is wrong, as this callback is per application, not per window; fix this
            GLFW.SetErrorCallback((code, desc) =>
            {
                ErrorOccurred?.Invoke(code, desc);
            });

            GLFW.WindowHint(WindowHintBool.Resizable, (_flags & WindowFlags.Resizable) != 0);
            GLFW.WindowHint(WindowHintBool.Visible, (_flags & WindowFlags.Visible) != 0);
            GLFW.WindowHint(WindowHintBool.Decorated, (_flags & WindowFlags.Decorated) != 0);
            GLFW.WindowHint(WindowHintBool.ScaleToMonitor, (_flags & WindowFlags.DpiAware) != 0);
            GLFW.WindowHint(WindowHintBool.Focused, (_flags & WindowFlags.Focused) != 0);

            GLFW.WindowHint(WindowHintBool.Floating, (_flags & WindowFlags.AlwaysOnTop) != 0);
            GLFW.WindowHint(WindowHintBool.TransparentFramebuffer, (_flags & WindowFlags.TransparentFramebuffer) != 0);

            Document = new UiDocument(new Size(_width, _height));
        }

        ~DesktopWindow()
        {
            Terminate();

            if (GlfwWindow != null)
            {
                UnregisterCallbacks();
            }

            //GLFW.SetErrorCallback(null);
        }

        #endregion

        /// <summary>
        /// Runs through the whole application lifetime. When this function returns,
        /// the window is destroyed and any subsequent calls or properties setting on this window object will fail.
        /// This will block the main thread when waiting for the next frame.
        /// </summary>
        /// <remarks>
        /// If an unhandled exception is thrown, the method will return, but without closing the window, so you can 
        /// call this in a try/catch inside a loop because running this function multiple times is ok unless the window was terminated.
        /// </remarks>
        public void Run()
        {
            //this.Document.Renderer.SetBgColor(Document.BackgroundColor);
            Document.Renderer.SetCanvasDirty();

            GLFW.GetMonitorContentScale(GLFW.GetPrimaryMonitor(), out float xScale, out float _);
            DocumentInvoke("WndSetContentScale", xScale);

            //don't use Glfw.WindowShouldClose because that is handled by OnCloseRequested
            while (!_shouldCloseWindow)
            {
                if (GlfwWindow == null)
                {
                    throw new NullReferenceException("Window pointer was null. Did you open the window first?");
                }

                if (GLFW.WindowShouldClose(GlfwWindow))
                {
                    //if the handler returns true, close the window
                    if (OnCloseRequested.Invoke())
                    {
                        Terminate();
                        return;
                    }
                }

                DoFrameActions();
                GLFW.WaitEventsTimeout(0.02);
                _canInvokeMaximize = true;
            }
        }

        /// <summary>
        /// Creates the window and opens it. You must open the window before interacting with it, like calling
        /// <see cref="Run"/>.
        /// </summary>
        /// <exception cref="InternalPlatformException">Thrown when GLFW couldn't create or show the window.</exception>
        public void Open()
        {
            //request OpenGL 3.3 core
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

            switch (CurrentWindowMode)
            {
                default:
                case WindowMode.Windowed:
                    GlfwWindow = GLFW.CreateWindow(_width, _height, _title, (Monitor*)0, (Window*)0);
                    break;
                case WindowMode.Minimized:
                    GlfwWindow = GLFW.CreateWindow(_width, _height, _title, (Monitor*)0, (Window*)0);
                    GLFW.IconifyWindow(GlfwWindow);
                    break;
                case WindowMode.Maximized:
                    GlfwWindow = GLFW.CreateWindow(_width, _height, _title, (Monitor*)0, (Window*)0);
                    GLFW.MaximizeWindow(GlfwWindow);
                    break;
                case WindowMode.Fullscreen:
                    {
                        _canInvokeMaximize = false;
                        Monitor* monitor = GLFW.GetPrimaryMonitor();
                        if (monitor == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get primary display");
                        }

                        VideoMode* videoMode = GLFW.GetVideoMode(monitor);
                        if (videoMode == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get video mode");
                        }

                        GLFW.WindowHint(WindowHintInt.RedBits, videoMode->RedBits);
                        GLFW.WindowHint(WindowHintInt.GreenBits, videoMode->GreenBits);
                        GLFW.WindowHint(WindowHintInt.BlueBits, videoMode->BlueBits);
                        GLFW.WindowHint(WindowHintInt.RefreshRate, videoMode->RefreshRate);

                        _width = videoMode->Width;
                        _height = videoMode->Height;
                        GlfwWindow = GLFW.CreateWindow(
                            videoMode->Width,
                            videoMode->Height,
                            _title,
                            monitor,
                            (Window*)0);
                        break;
                    }
                case WindowMode.ExclusiveFullscreen:
                    {
                        _canInvokeMaximize = false;
                        Monitor* monitor = GLFW.GetPrimaryMonitor();
                        if (monitor == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get primary display");
                        }

                        VideoMode* videoMode = GLFW.GetVideoMode(monitor);
                        if (videoMode == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get video mode");
                        }

                        _width = videoMode->Width;
                        _height = videoMode->Height;

                        GlfwWindow = GLFW.CreateWindow(_width, _height, _title, monitor, (Window*)0);
                        break;
                    }
            }

            if (GlfwWindow == null)
            {
                throw new InternalPlatformException("GLFW: Could not create window");
            }

            GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, _minHeight, _maxWidth, _maxHeight);
            CreateSurface();

#if USE_ANGLE
            Egl.SwapInterval(_eglDisplay, SwapInterval);
            GL.LoadBindings(new AngleBindingsContext());
#else
            GLFW.MakeContextCurrent(GlfwWindow);
            GLFW.SwapInterval(SwapInterval);
            GL.LoadBindings(new GLFWBindingsContext());
#endif

            RegisterCallbacks();
            FullyRedraw();
        }

        /// <summary>
        /// Closes the window, but if <see cref="OnCloseRequested"/> is overriden, it will be called, so the window might
        /// not actually close directly.
        /// </summary>
        public void Close()
        {
            _shouldCloseWindow = true;
            GLFW.PostEmptyEvent();
        }

        /// <summary>
        /// Sets the window mode. Any value will trigger <see cref="WindowModeChangedEvent"/>. You can minimize, maximize,
        /// make full-screen or make windowed this window. Regardless of the mode, the window will only be manipulated
        /// on the current display.
        /// </summary>
        /// <param name="mode">The new mode.</param>
        /// <param name="exclusiveFullscreenModeOptions">
        /// If mode is <see cref="WindowMode.ExclusiveFullscreen"/>, this specifies the display resolution and refresh rate;
        /// if this is left null, the display's preferred values are respected. This is ignored for other types of window mode.
        /// </param>
        /// <exception cref="InternalPlatformException">
        /// Thrown if GLFW (the internal windowing library) could not get the required data from the platform.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if mode is some invalid value that is not any of the enum values.
        /// </exception>
        public void SetWindowMode(
            WindowMode mode,
            ExclusiveFullscreenModeOptions? exclusiveFullscreenModeOptions = null)
        {
            _canInvokeMaximize = mode != WindowMode.Fullscreen && mode != WindowMode.ExclusiveFullscreen;

            switch (mode)
            {
                case WindowMode.Windowed:
                    if (CurrentWindowMode == WindowMode.Windowed)
                    {
                        break;
                    }

                    GLFW.RestoreWindow(GlfwWindow);
                    break;
                case WindowMode.Minimized:
                    if (CurrentWindowMode == WindowMode.Minimized)
                    {
                        break;
                    }

                    GLFW.IconifyWindow(GlfwWindow);
                    break;
                case WindowMode.Maximized:
                    if (CurrentWindowMode == WindowMode.Maximized)
                    {
                        break;
                    }

                    if (CurrentWindowMode == WindowMode.Fullscreen ||
                        CurrentWindowMode == WindowMode.ExclusiveFullscreen)
                    {
                        GLFW.SetWindowMonitor(GlfwWindow, null, 0, 0, _width, _height, 0);
                    }

                    GLFW.MaximizeWindow(GlfwWindow);
                    break;
                case WindowMode.Fullscreen:
                    {
                        if (CurrentWindowMode == WindowMode.Fullscreen)
                        {
                            break;
                        }

                        Monitor* monitor = GLFW.GetWindowMonitor(GlfwWindow);
                        if (monitor == null)
                        {
                            monitor = GLFW.GetPrimaryMonitor();

                            if (monitor == null)
                            {
                                throw new InternalPlatformException("GLFW: Could not get the window's display");
                            }
                        }

                        VideoMode* videoMode = GLFW.GetVideoMode(monitor);
                        if (videoMode == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get video mode");
                        }

                        GLFW.WindowHint(WindowHintInt.RedBits, videoMode->RedBits);
                        GLFW.WindowHint(WindowHintInt.GreenBits, videoMode->GreenBits);
                        GLFW.WindowHint(WindowHintInt.BlueBits, videoMode->BlueBits);
                        GLFW.WindowHint(WindowHintInt.RefreshRate, videoMode->RefreshRate);

                        GLFW.SetWindowMonitor(
                            GlfwWindow, null,
                            0, 0,
                            videoMode->Width, videoMode->Height,
                            0);

                        WindowModeChangedEvent?.Invoke(
                            this,
                            new WindowModeChangedEventArgs(mode, CurrentWindowMode));
                        break;
                    }
                case WindowMode.ExclusiveFullscreen:
                    {
                        if (CurrentWindowMode == WindowMode.ExclusiveFullscreen)
                        {
                            break;
                        }

                        Monitor* monitor = GLFW.GetWindowMonitor(GlfwWindow);
                        if (monitor == null)
                        {
                            monitor = GLFW.GetPrimaryMonitor();

                            if (monitor == null)
                            {
                                throw new InternalPlatformException("GLFW: Could not get window's display");
                            }
                        }

                        VideoMode* videoMode = GLFW.GetVideoMode(monitor);
                        if (videoMode == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get video mode");
                        }

                        _width = videoMode->Width;
                        _height = videoMode->Height;

                        GLFW.SetWindowMonitor(
                            GlfwWindow,
                            monitor,
                            0, 0,
                            exclusiveFullscreenModeOptions?.ResolutionWidth ?? videoMode->Width,
                            exclusiveFullscreenModeOptions?.ResolutionHeight ?? videoMode->Height,
                            exclusiveFullscreenModeOptions?.RefreshRate ?? videoMode->RefreshRate);

                        WindowModeChangedEvent?.Invoke(
                            this,
                            new WindowModeChangedEventArgs(mode, CurrentWindowMode));
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid window mode");
            }

            CurrentWindowMode = mode;
            LastSetWindowMode = mode;
        }

        /// <inheritdoc cref="IApplicationWindow.RequestAnimationFrame"/>
        /// <remarks>
        /// This is limited to <see cref="MaxFps"/>, meaning that if the drawing happens faster than the minimum time
        /// a frame must take, the main thread will sleep until that period is elapsed.
        /// </remarks>
        public void RequestAnimationFrame(Action<double> frameCallback)
        {
            GLFW.PostEmptyEvent();
            _animationFrameCallbacks.Add(frameCallback);
        }

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

                DocumentInvoke(
                    "WndCallPointerMove",
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
                    DocumentInvoke(
                        "WndCallPointerEnter",
                        new PointerEnterEventArgs(pos, pos, pressed));
                }
                else
                {
                    DocumentInvoke(
                        "WndCallPointerExit",
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

                DocumentInvoke(
                    "WndCallMouseButton",
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
                    DocumentInvoke(
                        "WndCallPointerDown",
                        new PointerDownEventArgs(pos, pos));
                }
                else
                {
                    DocumentInvoke(
                        "WndCallPointerUp",
                        new PointerUpEventArgs(pos, pos));
                }
            };
            GLFW.SetMouseButtonCallback(GlfwWindow, _mouseButtonCallback);

            _mouseScrollCallback = (_, deltaX, deltaY) =>
            {
                //TODO: both mouse and touchpad generate only -1 and 1 (at least on Linux), so we need to somehow
                // detect if it's a mouse or touchpad and scale the mouse accordingly

                GLFW.GetCursorPos(GlfwWindow, out double x, out double y);
                Point2D pos = new((float)x, (float)y);

                DocumentInvoke(
                    "WndCallMouseWheel",
                    new MouseWheelEventArgs(
                        pos,
                        pos,
                        (float)(deltaX == 0 ? deltaX : -deltaX),
                        (float)(deltaY == 0 ? deltaY : -deltaY),
                        (Document.PressedMouseButtons & MouseButtonType.Middle) != 0));
            };
            GLFW.SetScrollCallback(GlfwWindow, _mouseScrollCallback);
        }

        private void UnregisterCallbacks()
        {
            ResizedEvent = null;
            _resizeCallback = null;
            GLFW.SetWindowSizeCallback(GlfwWindow, null);

            _iconifyCallback = null;
            GLFW.SetWindowIconifyCallback(GlfwWindow, null);

            _maximizeCallback = null;
            GLFW.SetWindowMaximizeCallback(GlfwWindow, null);

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

        private void DoFrameActions()
        {
            double delta = GLFW.GetTime() - _lastTime;
            _lastTime = GLFW.GetTime();

            bool hadFrameCallbacks = false;
            if (_animationFrameCallbacks.Count > 0)
            {
                //if a callback registers another callback, this will effectively become an infinite loop,
                //to prevent this, before executing all the callbacks, store their number
                //and only execute that number of callbacks
                int thisFrameCount = _animationFrameCallbacks.Count;

                for (int i = 0; i < thisFrameCount; i++)
                {
                    _animationFrameCallbacks[i].Invoke(delta);
                    hadFrameCallbacks = true;
                }

                _animationFrameCallbacks.RemoveRange(0, thisFrameCount);
            }

            if (CatApplication.Instance.AppInitializer != null &&
                CatApplication.Instance.Dispatcher is DesktopDispatcher dispatcher)
            {
                dispatcher.CallActions();
            }

            if (hadFrameCallbacks || Document.Renderer.IsCanvasDirty)
            {
                if (Document.Renderer.IsCanvasDirty)
                {
                    FrameUpdatedEvent?.Invoke(delta);
                    FullyRedraw();
                }

                _lastTime = GLFW.GetTime();

#if USE_ANGLE
                Egl.SwapBuffers(_eglDisplay, _eglSurface);
#else
                GLFW.SwapBuffers(GlfwWindow);
#endif

                if (Document.Renderer.IsCanvasDirty)
                {
                    Document.Renderer.SkipCanvasPresentation();
                }

                //Debug.WriteLine(delta);
            }

            if (SwapInterval != -1 || MaxFps != -1)
            {
                return;
            }

            double minFrameTime = 1.0 / MaxFps;
            double thisFrameTime = GLFW.GetTime() - _lastTime;
            if (thisFrameTime < minFrameTime)
            {
                System.Threading.Thread.Sleep((int)((minFrameTime - thisFrameTime) * 1000));
            }
        }

#pragma warning disable CA1822 // Mark members as static
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void CreateSurface()
        {
#if USE_ANGLE
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            int[] platformAttributes =
            {
                Egl.PLATFORM_ANGLE_TYPE_ANGLE, Egl.PLATFORM_ANGLE_TYPE_D3D11_ANGLE,
                Egl.PLATFORM_ANGLE_MAX_VERSION_MAJOR_ANGLE, 1, Egl.PLATFORM_ANGLE_MAX_VERSION_MINOR_ANGLE, 1,
                Egl.NONE
            };
            _eglDisplay = Egl.GetPlatformDisplay(Egl.PLATFORM_ANGLE_ANGLE, (nint)0, platformAttributes);
            if (_eglDisplay == 0)
            {
                throw new InternalPlatformException("EGL: Could not get platform display");
            }

            if (!Egl.Initialize(_eglDisplay, out _, out _))
            {
                throw new InternalPlatformException("EGL: Could not initialize EGL");
            }

            int[] configAttributes =
            {
                Egl.SURFACE_TYPE, Egl.WINDOW_BIT, Egl.RENDERABLE_TYPE, Egl.OPENGL_ES2_BIT, Egl.NONE
            };
            nint[] eglConfig = new nint[1];
            if (!Egl.ChooseConfig(_eglDisplay, configAttributes, eglConfig, 1, out int numberOfConfigs) ||
                numberOfConfigs < 1)
            {
                throw new InternalPlatformException("EGL: Could not get configuration");
            }

            int[] contextAttributes = { Egl.CONTEXT_CLIENT_VERSION, 2, Egl.NONE };
            _eglContext = Egl.CreateContext(_eglDisplay, eglConfig[0], (nint)0, contextAttributes);
            if (_eglContext == 0)
            {
                throw new InternalPlatformException("EGL: Could not create context");
            }

            _eglSurface = Egl.CreateWindowSurface(_eglDisplay, eglConfig[0], NativeHandle, (nint)0);
            if (_eglSurface == 0)
            {
                throw new InternalPlatformException("EGL: Could not create surface");
            }

            if (!Egl.MakeCurrent(_eglDisplay, _eglSurface, _eglSurface, _eglContext))
            {
                throw new InternalPlatformException("EGL: Could not make surface current");
            }

#else
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
#endif
        }
#pragma warning restore CA1822 // Mark members as static

        private void Terminate()
        {
            //remove all the elements from the document
            Document.Root = null;

#if USE_ANGLE
            Egl.DestroySurface(_eglDisplay, _eglSurface);
            Egl.DestroyContext(_eglDisplay, _eglContext);
            Egl.Terminate(_eglDisplay);
#endif

            if (GlfwWindow != null)
            {
                GLFW.DestroyWindow(GlfwWindow);
                GlfwWindow = (Window*)0;
            }
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

            WindowModeChangedEvent?.Invoke(
                this,
                hasMinimizedNow
                    ? new WindowModeChangedEventArgs(WindowMode.Minimized, LastSetWindowMode)
                    : new WindowModeChangedEventArgs(LastSetWindowMode, WindowMode.Minimized));

            //This is a workaround for some window managers/display servers like KWin that will show the window framebuffer
            //as transparent after minimizing or restoring until a redraw happens.

            //TODO: instead of a full redraw, only redraw parts that are different from the back buffer \
            //TODO (): (this can only be implemented once partial redraws are implemented)
            GL.GetInteger(GetPName.FramebufferBinding, out int frame);
            GL.GetInteger(GetPName.StencilBits, out int stencil);
            GL.GetInteger(GetPName.Samples, out int samples);
            Document.Renderer.SetFramebufferData(frame, stencil, samples);

            Document.Renderer.SetCanvasDirty();
            DoFrameActions();

// #if USE_ANGLE
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

        private void FullyRedraw()
        {
            Document.Renderer.ResetAndClear();
            Document.DrawAllElements();
            Document.Renderer.Flush();
        }

        /// <summary>
        /// Dangerously calls non-internal instance methods from <see cref="Document"/>. These are necessary to make
        /// sure we don't have public access to those setters, only implementations of <see cref="IApplicationWindow"/>
        /// should be allowed to modify those.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="args">The arguments to give.</param>
        private void DocumentInvoke(string methodName, params object[] args)
        {
            MethodInfo? func = Document.GetType().GetMethod(
                methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (func != null)
            {
                func.Invoke(Document, args);
            }
        }

        // ReSharper disable once UnusedType.Local
        private sealed class AngleBindingsContext : IBindingsContext
        {
            public nint GetProcAddress(string function)
            {
                return Egl.GetProcAddress(function);
            }
        }
    }

    public readonly struct ExclusiveFullscreenModeOptions
    {
        public int ResolutionWidth { get; }
        public int ResolutionHeight { get; }
        public int RefreshRate { get; }

        public ExclusiveFullscreenModeOptions(int resolutionWidth, int resolutionHeight, int refreshRate)
        {
            ResolutionWidth = resolutionWidth;
            ResolutionHeight = resolutionHeight;
            RefreshRate = refreshRate;
        }
    }
}
