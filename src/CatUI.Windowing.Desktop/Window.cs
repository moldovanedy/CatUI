using System;
using System.Collections.Generic;

using CatUI.Data;
using CatUI.Elements;

using OpenTK;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Windowing.Desktop
{
    public unsafe class Window
    {
        public nint NativeHandle
        {
            get
            {
                if (OperatingSystem.IsWindows())
                {
                    return GLFW.GetWin32Window(GlfwWindow);
                }
                else if (OperatingSystem.IsLinux())
                {
                    return (nint)(nuint)GLFW.GetX11Window(GlfwWindow);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    return GLFW.GetCocoaWindow(GlfwWindow);
                }
                else
                {
                    return 0;
                }
            }
        }
        public UIDocument Document { get; private set; } = new UIDocument();

        internal OpenTK.Windowing.GraphicsLibraryFramework.Window* GlfwWindow { get; private set; }

        private bool _shouldCloseWindow;
        private WindowFlags _flags = WindowFlags.Default;
        private WindowMode _startupMode = WindowMode.Windowed;
        private bool _isCreated;

#if USE_ANGLE
        private nint eglDisplay;
        private nint eglSurface;
        private nint eglContext;
#endif

        private GLFWCallbacks.WindowSizeCallback? _resizeCallback;
        private GLFWCallbacks.ErrorCallback? _errorCallback;

        #region Object creation
        public Window(
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
            SetInitialWidth(width);
            SetInitialHeight(height);
            SetInitialTitle(title);
            SetInitialMinWidth(minWidth);
            SetInitialMaxWidth(maxWidth);
            SetInitialMinHeight(minHeight);
            SetInitialMaxHeight(maxHeight);
            SetInitialWindowFlags(windowFlags);
            SetInitialStartupMode(startupMode);

            Create();
        }

        public Window SetInitialWidth(int width)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _width = width;
            return this;
        }

        public Window SetInitialHeight(int height)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _height = height;
            return this;
        }

        public Window SetInitialMinWidth(int minWidth)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _minWidth = minWidth;
            return this;
        }

        public Window SetInitialMinHeight(int minHeight)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _minHeight = minHeight;
            return this;
        }

        public Window SetInitialMaxWidth(int maxWidth)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _maxWidth = maxWidth;
            return this;
        }

        public Window SetInitialMaxHeight(int maxHeight)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _maxHeight = maxHeight;
            return this;
        }

        public Window SetInitialTitle(string title)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _title = title;
            return this;
        }

        public Window SetInitialWindowFlags(WindowFlags flags)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created");
            }

            _flags = flags;
            return this;
        }

        public Window SetInitialStartupMode(WindowMode startupMode)
        {
            if (_isCreated)
            {
                throw new Exception("Window is already created, use direct properties instead");
            }

            _startupMode = startupMode;
            return this;
        }

        public Window Create()
        {
            Init();

            GLFW.WindowHint(WindowHintBool.Resizable, (_flags & WindowFlags.Resizable) != 0);
            GLFW.WindowHint(WindowHintBool.Visible, (_flags & WindowFlags.Visible) != 0);
            GLFW.WindowHint(WindowHintBool.Decorated, (_flags & WindowFlags.Decorated) != 0);
            //GLFW_SCALE_TO_MONITOR is 0x0002200C
            GLFW.WindowHint((WindowHintBool)0x0002200C, (_flags & WindowFlags.DpiAware) != 0);
            GLFW.WindowHint(WindowHintBool.Focused, (_flags & WindowFlags.Focused) != 0);

            GLFW.WindowHint(WindowHintBool.Floating, (_flags & WindowFlags.AlwaysOnTop) != 0);
            GLFW.WindowHint(WindowHintBool.TransparentFramebuffer, (_flags & WindowFlags.TransparentFramebuffer) != 0);

            switch (_startupMode)
            {
                case WindowMode.Windowed:
                    GlfwWindow = GLFW.CreateWindow(_width, _height, _title, (Monitor*)0, (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0);
                    break;
                case WindowMode.Minimized:
                    GlfwWindow = GLFW.CreateWindow(_width, _height, _title, (Monitor*)0, (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0);
                    GLFW.IconifyWindow(GlfwWindow);
                    break;
                case WindowMode.Maximized:
                    GlfwWindow = GLFW.CreateWindow(_width, _height, _title, (Monitor*)0, (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0);
                    GLFW.MaximizeWindow(GlfwWindow);
                    break;
                case WindowMode.Fullscreen:
                    {
                        Monitor* monitor = GLFW.GetPrimaryMonitor();
                        VideoMode* videoMode = GLFW.GetVideoMode(monitor);
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
                            (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0);
                        break;
                    }
                case WindowMode.ExclusiveFullscreen:
                    {
                        Monitor* monitor = GLFW.GetPrimaryMonitor();
                        VideoMode* videoMode = GLFW.GetVideoMode(monitor);
                        _width = videoMode->Width;
                        _height = videoMode->Height;

                        GlfwWindow = GLFW.CreateWindow(_width, _height, _title, monitor, (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0);
                        break;
                    }
                default:
                    break;
            }

            GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, _minHeight, _maxWidth, _maxHeight);

            CreateSurface();
#if USE_ANGLE
            Egl.SwapInterval(eglDisplay, 1);
            GL.LoadBindings(new AngleBindingsContext());
#else
            GLFW.MakeContextCurrent(GlfwWindow);
            GLFW.SwapInterval(1);
            GL.LoadBindings(new GLFWBindingsContext());
#endif
            _resizeCallback = (glfwWindow, width, height) =>
            {
                Resized?.Invoke(width, height);
            };
            Resized += ResizeWindow;
            GLFW.SetWindowSizeCallback(GlfwWindow, _resizeCallback);

            _errorCallback = (errCode, message) =>
            {
                throw new GLFWException(message, errCode);
            };
            GLFW.SetErrorCallback(_errorCallback);

            Document = new UIDocument
            {
                ViewportSize = new Size(_width, _height)
            };
            FullyRedraw();

            _isCreated = true;
            return this;
        }

        ~Window()
        {
            Terminate();

            Resized -= ResizeWindow;
            GLFW.SetWindowSizeCallback(GlfwWindow, null);

            _resizeCallback = null;
            _errorCallback = null;
        }
        #endregion

        #region Properties
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                GLFW.SetWindowSize(GlfwWindow, value, _height);
                _width = value;
            }
        }
        private int _width = 50;

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                GLFW.SetWindowSize(GlfwWindow, _width, value);
                _height = value;
            }
        }
        private int _height = 50;

        public int MinWidth
        {
            get
            {
                return _minWidth;
            }
            set
            {
                GLFW.SetWindowSizeLimits(GlfwWindow, value, _minHeight, _maxWidth, _maxHeight);
                _minWidth = value;
            }
        }
        private int _minWidth;

        public int MinHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, value, _maxWidth, _maxHeight);
                _minHeight = value;
            }
        }
        private int _minHeight;

        public int MaxWidth
        {
            get
            {
                return _maxWidth;
            }
            set
            {
                GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, _minHeight, value, _maxHeight);
                _maxWidth = value;
            }
        }
        private int _maxWidth = ushort.MaxValue;

        public int MaxHeight
        {
            get
            {
                return _maxHeight;
            }
            set
            {
                GLFW.SetWindowSizeLimits(GlfwWindow, _minWidth, _minHeight, _maxHeight, value);
                _maxHeight = value;
            }
        }
        private int _maxHeight = ushort.MaxValue;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                GLFW.SetWindowTitle(GlfwWindow, _title);
            }
        }
        private string _title = string.Empty;

        /// <summary>
        /// This is the maximum number of frames per second the application is allowed to run. Lower values (like 30) 
        /// will make the application use less resources but will have some "lag", as the visuals will look more sluggish.
        /// Higher values will reduce visual "lag", but will utilize more CPU and GPU, thus potentially making the system slower.
        /// The default value is 60, which is suitable for most applications.
        /// </summary>
        public int MaxFPS { get; set; } = 60;
        #endregion

        #region Events
        public event Action<OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode, string>? ErrorOccurred;
        /// <summary>
        /// Fired when the user or the OS requested the application close. Returning true (the default behavior)
        /// will close the window immediately, while returning false will make the window continue running.
        /// This can be useful for implementing a prompt for the user (e.g. if they would like to save changes before the app is closed).
        /// </summary>
        /// <remarks>
        /// Although the window will be closed after this returns true, your app will still run until the end of the Main function.
        /// </remarks>
        public event Func<bool> CloseRequested = () =>
        {
            return true;
        };

        public event Action<int, int>? Resized;
        #endregion

        [Flags]
        public enum WindowFlags
        {
            Default = 0b11111,
            Resizable = 1,
            Visible = 2,
            Decorated = 4,
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
        private List<Action<double>> _animationFrameCallbacks = new List<Action<double>>();

        /// <summary>
        /// An event that is fired when the internal windowing system decides to redraw
        /// a part of the viewport or the whole viewport. Do NOT use this as a "game loop", as this won't be invoked
        /// on a regular basis, but rather only when it's necessary (something has changed visually).
        /// </summary>
        /// <remarks>
        /// If you want a reliable continuous loop for each frame, see <see cref="RequestAnimationFrame(Action{double})"/>.
        /// This event won't be invoked even when using <see cref="RequestAnimationFrame(Action{double})"/>
        /// unless something really has been drawn.
        /// </remarks>
        public event Action<double>? FrameUpdatedEvent;

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

            //don't use Glfw.WindowShouldClose because that is handled by CloseRequested
            while (!_shouldCloseWindow)
            {
                if (GLFW.WindowShouldClose(GlfwWindow))
                {
                    //if the handler returns true, close the window
                    if (CloseRequested.Invoke())
                    {
                        Terminate();
                        return;
                    }
                }

                DoFrameActions();
                GLFW.WaitEventsTimeout(0.02);
            }
        }

        public void Close()
        {
            _shouldCloseWindow = true;
            GLFW.PostEmptyEvent();
        }

        /// <summary>
        /// This function works similarly to the requestAnimationFrame web API. The given callback will be executed before starting
        /// to draw the next frame. Beware that this must perform a full redraw of the content even if nothing has changed, so
        /// beware of the eventual performance penalties.
        /// </summary>
        /// <remarks>
        /// This is limited to <see cref="MaxFPS"/>, meaning that if the drawing happens faster than the minimum time a frame must take,
        /// the main thread will sleep until that period is elapsed.
        /// </remarks>
        /// <param name="frameCallback">
        /// A function that receives the time between the last frame and the current frame 
        /// in seconds as a parameter.
        /// </param>
        public void RequestAnimationFrame(Action<double> frameCallback)
        {
            GLFW.PostEmptyEvent();
            _animationFrameCallbacks.Add(frameCallback);
        }

        private void Init()
        {
            //Wayland
            GLFW.InitHint((InitHintInt)0x00050003, 0x00060003);

            if (!GLFW.Init())
            {
                OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode errorCode = GLFW.GetError(out string description);
                throw new Exception($"Internal GLFW error ({errorCode}): {description}");
            }

            GLFW.SetErrorCallback((OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode code, string desc) =>
            {
                ErrorOccurred?.Invoke(code, desc);
            });
        }

        private void DoFrameActions()
        {
            double delta = GLFW.GetTime() - _lastTime;
            _lastTime = GLFW.GetTime();

            //bool hadFrameCallbacks = false;
            if (_animationFrameCallbacks.Count > 0)
            {
                //if a callback registers another callback, this will effectively become an infinite loop,
                //to prevent this, before executing all the callbacks, store their number
                //and only execute that number of callbacks
                int thisFrameCount = _animationFrameCallbacks.Count;

                for (int i = 0; i < thisFrameCount; i++)
                {
                    _animationFrameCallbacks[i].Invoke(delta);
                    //hadFrameCallbacks = true;
                }

                _animationFrameCallbacks.RemoveRange(0, thisFrameCount);
            }

            if (Document.Renderer.IsCanvasDirty)
            {
                FrameUpdatedEvent?.Invoke(delta);
                FullyRedraw();
                _lastTime = GLFW.GetTime();

#if USE_ANGLE
                Egl.SwapBuffers(eglDisplay, eglSurface);
#else
                GLFW.SwapBuffers(GlfwWindow);
#endif

                Document.Renderer.SkipCanvasPresentation();
                // Debug.WriteLine(delta);
            }

            // double minFrameTime = 1.0 / MaxFPS;
            // double thisFrameTime = GLFW.GetTime() - _lastTime;
            // if (thisFrameTime < minFrameTime)
            // {
            //     System.Threading.Thread.Sleep((int)((minFrameTime - thisFrameTime) * 1000));
            // }
        }

#pragma warning disable CA1822 // Mark members as static
        private void CreateSurface()
        {
#if USE_ANGLE
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            int[] platformAttributes =
            [
                Egl.PLATFORM_ANGLE_TYPE_ANGLE, Egl.PLATFORM_ANGLE_TYPE_D3D11_ANGLE,
                Egl.PLATFORM_ANGLE_MAX_VERSION_MAJOR_ANGLE, 1,
                Egl.PLATFORM_ANGLE_MAX_VERSION_MINOR_ANGLE, 1,
                Egl.NONE,
            ];
            eglDisplay = Egl.GetPlatformDisplay(Egl.PLATFORM_ANGLE_ANGLE, 0, platformAttributes);
            if (eglDisplay == 0)
            {
                return;
            }

            if (!Egl.Initialize(eglDisplay, out _, out _))
            {
                return;
            }

            int[] configAttributes =
            [
                Egl.SURFACE_TYPE, Egl.WINDOW_BIT,
                Egl.RENDERABLE_TYPE, Egl.OPENGL_ES2_BIT,
                Egl.NONE
            ];
            nint[] eglConfig = new nint[1];
            if (!Egl.ChooseConfig(eglDisplay, configAttributes, eglConfig, 1, out int numberOfConfigs) ||
                numberOfConfigs < 1)
            {
                return;
            }

            int[] contextAttributes =
            [
                Egl.CONTEXT_CLIENT_VERSION, 2,
                Egl.NONE
            ];
            eglContext = Egl.CreateContext(eglDisplay, eglConfig[0], 0, contextAttributes);
            if (eglContext == 0)
            {
                return;
            }

            eglSurface = Egl.CreateWindowSurface(eglDisplay, eglConfig[0], NativeHandle, 0);
            if (eglSurface == 0)
            {
                return;
            }

            if (!Egl.MakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext))
            {
                return;
            }
#else
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
#endif
            //GLFW.WindowHint(WindowHintContextApi.ContextCreationApi, ContextApi.NativeContextApi);
        }
#pragma warning restore CA1822 // Mark members as static

        private void Terminate()
        {
            //remove all the elements from the document
            Document.Root = null;
            GLFW.SetErrorCallback(null);

#if USE_ANGLE
            Egl.DestroySurface(eglDisplay, eglSurface);
            Egl.DestroyContext(eglDisplay, eglContext);
            Egl.Terminate(eglDisplay);
#endif

            if (GlfwWindow != (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0)
            {
                GLFW.DestroyWindow(GlfwWindow);
                GlfwWindow = (OpenTK.Windowing.GraphicsLibraryFramework.Window*)0;
            }
            //GLFW.Terminate();
        }

        private void ResizeWindow(int width, int height)
        {
            //GL.Viewport(0, 0, width, height);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.GetInteger(GetPName.FramebufferBinding, out int frame);
            GL.GetInteger(GetPName.StencilBits, out int stencil);
            GL.GetInteger(GetPName.Samples, out int samples);
            Document.Renderer.SetFramebufferData(frame, stencil, samples);

            Document.ViewportSize = new Size(width, height);
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
