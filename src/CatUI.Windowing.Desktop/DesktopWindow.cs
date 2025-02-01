using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Elements;
using CatUI.Windowing.Common;
using OpenTK;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Windowing.Desktop
{
    public unsafe class DesktopWindow : IApplicationWindow
    {
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
                    if (GLFW.GetPlatform() == Platform.X11)
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

        public UiDocument Document { get; }

        internal Window* GlfwWindow { get; private set; }

        private bool _shouldCloseWindow;
        private readonly WindowFlags _flags;
        private readonly WindowMode _startupMode;

#if USE_ANGLE
        private nint _eglDisplay;
        private nint _eglSurface;
        private nint _eglContext;
#endif

        private GLFWCallbacks.WindowSizeCallback? _resizeCallback;

        #region Properties

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

        public bool IsDpiAware => (_flags & WindowFlags.DpiAware) != 0;


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

        public Func<bool> OnCloseRequested { get; set; } = () => true;

        public event ResizedEventHandler? ResizedEvent;

        #endregion

        /// <summary>
        /// Represents the window startup flags. These are a bitmap that must be set in the constructor, before the call
        /// to <see cref="DesktopWindow.Open"/> and can not be modified afterward.
        /// </summary>
        [Flags]
        public enum WindowFlags
        {
            /// <summary>
            /// Represents the default value for the flags (<see cref="Resizable"/>, <see cref="Visible"/>,
            /// <see cref="Decorated"/>, <see cref="DpiAware"/> and <see cref="Focused"/>).
            /// </summary>
            Default = 0b11111,
            Resizable = 1,
            Visible = 2,
            Decorated = 4,

            /// <summary>
            /// Indicates that the window's scale will be automatically controlled by CatUI and will correspond to
            /// the platform preferences. It is highly recommended to set this flag. It is enabled by default.
            /// </summary>
            DpiAware = 8,
            Focused = 16,

            AlwaysOnTop = 32,
            TransparentFramebuffer = 64
        }

        public enum WindowMode
        {
            Windowed = 0,
            Minimized = 1,
            Maximized = 2,
            Fullscreen = 3,
            ExclusiveFullscreen = 4
        }

        private double _lastTime;
        private readonly List<Action<double>> _animationFrameCallbacks = new();

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
            _startupMode = startupMode;

            if (!GLFW.Init())
            {
                OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode errorCode = GLFW.GetError(out string description);
                throw new Exception($"Internal GLFW error ({errorCode}): {description}");
            }

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

            Document = new UiDocument { ViewportSize = new Size(_width, _height) };
        }

        ~DesktopWindow()
        {
            Terminate();

            if (GlfwWindow != null)
            {
                GLFW.SetWindowSizeCallback(GlfwWindow, null);
            }

            ResizedEvent -= ResizeWindow;
            GLFW.SetErrorCallback(null);

            _resizeCallback = null;
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
            Document.ContentScale = xScale;

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
            }
        }

        public void Open()
        {
            switch (_startupMode)
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
                        Monitor* monitor = GLFW.GetPrimaryMonitor();
                        if (monitor == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get primary monitor");
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
                            GLFW.GetPrimaryMonitor(),
                            (Window*)0);
                        break;
                    }
                case WindowMode.ExclusiveFullscreen:
                    {
                        Monitor* monitor = GLFW.GetPrimaryMonitor();
                        if (monitor == null)
                        {
                            throw new InternalPlatformException("GLFW: Could not get primary monitor");
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

            _resizeCallback = (_, newWidth, newHeight) =>
            {
                ResizedEvent?.Invoke(
                    this,
                    new ResizedEventArgs(Width, Height, newWidth, newHeight)
                );
            };
            ResizedEvent += ResizeWindow;
            GLFW.SetWindowSizeCallback(GlfwWindow, _resizeCallback);

            FullyRedraw();
        }

        public void Close()
        {
            _shouldCloseWindow = true;
            GLFW.PostEmptyEvent();
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
            GLFW.SetErrorCallback(null);

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

        private void ResizeWindow(object sender, ResizedEventArgs e)
        {
            //GL.Viewport(0, 0, width, height);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.GetInteger(GetPName.FramebufferBinding, out int frame);
            GL.GetInteger(GetPName.StencilBits, out int stencil);
            GL.GetInteger(GetPName.Samples, out int samples);
            Document.Renderer.SetFramebufferData(frame, stencil, samples);

            Document.ViewportSize = new Size(e.NewWidth, e.NewHeight);
            Document.Renderer.SetCanvasDirty();
            DoFrameActions();
        }

        private void FullyRedraw()
        {
            Document.Renderer.ResetAndClear();
            Document.DrawAllElements();
            Document.Renderer.Flush();
        }

        private sealed class AngleBindingsContext : IBindingsContext
        {
            public nint GetProcAddress(string function)
            {
                return Egl.GetProcAddress(function);
            }
        }
    }
}
