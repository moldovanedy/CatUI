using System;
using CatUI.Data.Exceptions;
using CatUI.Windowing.Common;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.ES20;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

namespace CatUI.Windowing.Desktop.GraphicsBackends
{
    internal class AngleGraphicsBackend : IGraphicsBackend
    {
        private nint _nativeWindowHandle;
        private int _swapInterval;

        private nint _eglDisplay;
        private nint _eglSurface;
        private nint _eglContext;

        private GRContext? _grContext;
        private GRGlFramebufferInfo _glInfo;
        private GRBackendRenderTarget? _renderTarget;
        private SKSize _lastSize;

        private int _framebufferBinding;
        private int _stencilBits;
        private int _samples;

        private int _width;
        private int _height;

        internal void SetWindowPointer(nint windowPtr)
        {
            _nativeWindowHandle = windowPtr;
        }

        public void PrepareWindowCreation()
        {
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
        }

        public void PostWindowCreation()
        {
            //for DX11: Egl.PLATFORM_ANGLE_TYPE_D3D11_ANGLE
            int[] platformAttributes =
            [
                Egl.PLATFORM_ANGLE_TYPE_ANGLE, Egl.PLATFORM_ANGLE_TYPE_OPENGL_ANGLE,
                Egl.PLATFORM_ANGLE_MAX_VERSION_MAJOR_ANGLE, 1, Egl.PLATFORM_ANGLE_MAX_VERSION_MINOR_ANGLE, 1,
                Egl.NONE
            ];

            _eglDisplay = Egl.GetPlatformDisplay(Egl.PLATFORM_ANGLE_ANGLE, 0, platformAttributes);
            if (_eglDisplay == 0)
            {
                throw new InternalPlatformException("EGL: Could not get platform display");
            }

            if (!Egl.Initialize(_eglDisplay, out _, out _))
            {
                throw new InternalPlatformException("EGL: Could not initialize EGL");
            }

            int[] configAttributes =
            [
                Egl.SURFACE_TYPE, Egl.WINDOW_BIT, Egl.RENDERABLE_TYPE, Egl.OPENGL_ES2_BIT, Egl.NONE
            ];

            nint[] eglConfig = new nint[1];
            if (!Egl.ChooseConfig(_eglDisplay, configAttributes, eglConfig, 1, out int numberOfConfigs) ||
                numberOfConfigs < 1)
            {
                throw new InternalPlatformException("EGL: Could not get configuration");
            }

            int[] contextAttributes = { Egl.CONTEXT_CLIENT_VERSION, 2, Egl.NONE };
            _eglContext = Egl.CreateContext(_eglDisplay, eglConfig[0], 0, contextAttributes);
            if (_eglContext == 0)
            {
                throw new InternalPlatformException("EGL: Could not create context");
            }

            _eglSurface = Egl.CreateWindowSurface(_eglDisplay, eglConfig[0], _nativeWindowHandle, 0);
            if (_eglSurface == 0)
            {
                throw new InternalPlatformException("EGL: Could not create surface");
            }

            if (!Egl.MakeCurrent(_eglDisplay, _eglSurface, _eglSurface, _eglContext))
            {
                throw new InternalPlatformException("EGL: Could not make surface current");
            }
        }

        public SKSurface RecreateSurface(SKSurface previousSurface)
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
                previousSurface.Dispose();
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
                if (canvas == null)
                {
                    throw new NullReferenceException("Canvas is null. This is probably an internal graphics error.");
                }

                return surface;
            }

            return previousSurface;
        }

        public void SwapBuffers()
        {
            Egl.SwapBuffers(_eglDisplay, _eglSurface);
        }

        public void DestroyAndTerminate()
        {
            Egl.DestroySurface(_eglDisplay, _eglSurface);
            Egl.DestroyContext(_eglDisplay, _eglContext);
            Egl.Terminate(_eglDisplay);
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
            Egl.SwapInterval(_eglDisplay, _swapInterval);
        }
    }
}
