using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

#if CAT_USE_ANGLE
using CatUI.Data.Exceptions;
using OpenTK.Graphics.Egl;
#endif

namespace CatUI.Windowing.Desktop
{
    public partial class DesktopWindow
    {
        private const SKColorType COLOR_TYPE = SKColorType.Rgba8888;
        private const GRSurfaceOrigin SURFACE_ORIGIN = GRSurfaceOrigin.BottomLeft;

        private GRContext? _grContext;
        private GRGlFramebufferInfo _glInfo;
        private GRBackendRenderTarget? _renderTarget;
        private SKSize _lastSize;

        private int _framebufferBinding;
        private int _stencilBits;
        private int _samples;

        public void SetHwFramebufferData(int fbBinding, int stencilBits, int samples)
        {
            _framebufferBinding = fbBinding;
            _stencilBits = stencilBits;
            _samples = samples;
        }

#pragma warning disable CA1822 // Mark members as static
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void CreateHwSurface()
        {
#if CAT_USE_ANGLE
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

        private void RecreateSkiaDrawingObjects()
        {
            //create the contexts if not done already
            if (_grContext == null)
            {
                var glInterface = GRGlInterface.Create(name =>
                {
                    IntPtr fnPointer = GLFW.GetProcAddress(name);
                    return fnPointer;
                });
                _grContext = GRContext.CreateGl(glInterface);

                if (_grContext == null)
                {
                    throw new NullReferenceException(
                        "Graphics context is null. This is probably an internal graphics error.");
                }
            }

            SKSize newSize = new(_width, _height);

            //manage the drawing surface
            if (_renderTarget == null || _lastSize != newSize || !_renderTarget.IsValid)
            {
                _lastSize = newSize;

                int maxSamples = _grContext.GetMaxSurfaceSampleCount(COLOR_TYPE);
                if (_samples > maxSamples)
                {
                    _samples = maxSamples;
                }

                _glInfo = new GRGlFramebufferInfo((uint)_framebufferBinding, COLOR_TYPE.ToGlSizedFormat());

                //destroy the old surface
                Document.Renderer.Surface?.Dispose();
                Document.Renderer.SetPlatformManagedData(null, null);

                //re-create the render target
                _renderTarget?.Dispose();
                _renderTarget = new GRBackendRenderTarget((int)newSize.Width, (int)newSize.Height, _samples,
                    _stencilBits, _glInfo);
            }

            //create the surface
            if (Document.Renderer.Surface == null)
            {
                var surface = SKSurface.Create(_grContext, _renderTarget, SURFACE_ORIGIN, COLOR_TYPE);
                if (surface == null)
                {
                    throw new NullReferenceException(
                        "Drawing surface is null. This is probably an internal graphics error.");
                }

                SKCanvas canvas = surface.Canvas;
                if (canvas == null)
                {
                    throw new NullReferenceException("Canvas is null. This is probably an internal graphics error.");
                }

                Document.Renderer.SetPlatformManagedData(surface, canvas);
            }
        }
    }
}
