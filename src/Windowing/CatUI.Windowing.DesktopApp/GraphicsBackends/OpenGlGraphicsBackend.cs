using System;
using CatUI.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends;

internal sealed unsafe class OpenGlGraphicsBackend : IGraphicsBackend
{
    private readonly int _requestedMajorVersion;
    private readonly int _requestedMinorVersion;

    private Window* _glfwWindow;
    private int _swapInterval;

    private GRContext? _grContext;
    private GRGlFramebufferInfo _glInfo;
    private GRBackendRenderTarget? _renderTarget;
    private SKSize _lastSize;

    private int _framebufferBinding;
    private int _stencilBits;
    private int _samples;

    private int _width;
    private int _height;

    /// <summary>
    /// Version 0.0 will try to get the highest version possible. Other values specify the minimum OpenGL version.
    /// Version 2.0 should be the absolute minimum for SkiaSharp to work, but 3.3 is supported by most devices
    /// nowadays.
    /// </summary>
    /// <remarks>
    /// Versions lower than 3.2 are for fallback use only! Features might not work with a lower version, or the
    /// window creation will fail altogether (especially on macOS).
    /// </remarks>
    /// <param name="requestedMajorVersion">
    /// The requested OpenGL major version. Note that the actual version might be higher than the requested one,
    /// but not lower.
    /// </param>
    /// <param name="requestedMinorVersion">
    /// The requested OpenGL minor version. Note that the actual version might be higher than the requested one,
    /// but not lower.
    /// </param>
    internal OpenGlGraphicsBackend(int requestedMajorVersion = 0, int requestedMinorVersion = 0)
    {
        _requestedMajorVersion = requestedMajorVersion;
        _requestedMinorVersion = requestedMinorVersion;
    }

    internal void SetGlfwWindowPointer(Window* windowPtr)
    {
        _glfwWindow = windowPtr;
    }

    public void PrepareWindowCreation()
    {
        GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);

        GLFW.WindowHint(WindowHintInt.ContextVersionMajor,
            _requestedMajorVersion == 0 ? 1 : _requestedMajorVersion);
        GLFW.WindowHint(WindowHintInt.ContextVersionMinor, _requestedMinorVersion);

        if (_requestedMajorVersion >= 3 && _requestedMinorVersion >= 2)
        {
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
        }
        else
        {
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Any);
        }
    }

    public void PostWindowCreation()
    {
        GLFW.MakeContextCurrent(_glfwWindow);
        GL.LoadBindings(new GLFWBindingsContext());
    }

    public SKSurface RecreateSurface(SKSurface? previousSurface)
    {
        //create the contexts if not done already
        if (_grContext == null)
        {
            GRGlInterface glInterface;
            if (GLFW.GetPlatform() == OpenTK.Windowing.GraphicsLibraryFramework.Platform.X11)
            {
                glInterface = GRGlInterface.Create();
            }
            else
            {
                glInterface = GRGlInterface.Create(name =>
                {
                    // ReSharper disable once ConvertClosureToMethodGroup
                    return GLFW.GetProcAddress(name);
                });
            }

            _grContext = GRContext.CreateGl(glInterface);
            if (_grContext == null)
            {
                throw new NullReferenceException(
                    "Graphics context is null. This is probably an internal graphics error.");
            }
        }

        SKSize newSize = new(_width, _height);
        bool isSurfaceDestroyed = false;

        //manage the drawing surface
        if (_renderTarget == null || _lastSize != newSize || !_renderTarget.IsValid)
        {
            _lastSize = newSize;

            int maxSamples = _grContext.GetMaxSurfaceSampleCount(IGraphicsBackend.COLOR_TYPE);
            if (_samples > maxSamples)
            {
                _samples = maxSamples;
            }

            _glInfo = new GRGlFramebufferInfo(
                (uint)_framebufferBinding,
                IGraphicsBackend.COLOR_TYPE.ToGlSizedFormat());

            //destroy the old surface
            previousSurface?.Dispose();
            isSurfaceDestroyed = true;

            //re-create the render target
            _renderTarget?.Dispose();
            _renderTarget = new GRBackendRenderTarget(
                (int)newSize.Width,
                (int)newSize.Height,
                _samples,
                _stencilBits,
                _glInfo);
        }

        //create the surface
        if (isSurfaceDestroyed)
        {
            var surface = SKSurface.Create(
                _grContext,
                _renderTarget,
                IGraphicsBackend.SURFACE_ORIGIN,
                IGraphicsBackend.COLOR_TYPE);
            if (surface == null)
            {
                throw new NullReferenceException(
                    "Drawing surface is null. This is probably an internal graphics error.");
            }

            SKCanvas canvas = surface.Canvas;
            return canvas == null
                ? throw new NullReferenceException("Canvas is null. This is probably an internal graphics error.")
                : surface;
        }

        return
            previousSurface
         ?? throw new NullReferenceException(
                "Created surface is null. This is probably an internal graphics error.");
    }


    public void PresentFramebuffer()
    {
        GLFW.SwapBuffers(_glfwWindow);
    }

    public void DestroyAndTerminate()
    {
        //no-op
    }

    public void Resized(int width, int height)
    {
        _width = width;
        _height = height;

        GL.GetInteger(GetPName.FramebufferBinding, out int frame);
        GL.GetInteger(GetPName.StencilBits, out int stencil);
        GL.GetInteger(GetPName.Samples, out int samples);

        _framebufferBinding = frame;
        _stencilBits = stencil;
        _samples = samples;
    }

    public void SwapIntervalChanged(int swapInterval)
    {
        _swapInterval = swapInterval;
        GLFW.SwapInterval(_swapInterval);
    }
}
