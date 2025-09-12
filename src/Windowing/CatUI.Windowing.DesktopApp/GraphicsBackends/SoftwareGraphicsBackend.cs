using System;
using System.Runtime.InteropServices;
using CatUI.Platform;
using CatUI.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends
{
    public unsafe class SoftwareGraphicsBackend : IGraphicsBackend
    {
        private Window* _glfwWindow;
        private nint _pixelDataPtr;
        private int _swapInterval;
        
        private SKSize _lastSize;
        private int _width;
        private int _height;
        private int _bytesPerRow;
        
        internal void SetGlfwWindowPointer(Window* windowPtr)
        {
            _glfwWindow = windowPtr;
        }
        
        public void PrepareWindowCreation()
        {
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
        }

        public void PostWindowCreation()
        {
            //no-op
        }

        public SKSurface RecreateSurface(SKSurface previousSurface)
        {
            SKSize newSize = new(_width, _height);
            if (_lastSize == newSize)
            {
                return
                    previousSurface
                 ?? throw new NullReferenceException(
                        "Created surface is null. This is probably an internal graphics error.");
            }

            _lastSize = newSize;
            Marshal.FreeCoTaskMem(_pixelDataPtr);
            SKImageInfo info = new(_width, _height, SKColorType.Rgba8888, SKAlphaType.Premul);
            _pixelDataPtr = Marshal.AllocCoTaskMem(info.BytesSize);
            _bytesPerRow = info.RowBytes;
            
            var surface = SKSurface.Create(info, _pixelDataPtr, info.RowBytes);
            if (surface == null)
            {
                throw new NullReferenceException(
                    "Drawing surface is null. This is probably an internal graphics error.");
            }
            
            if (surface.Canvas == null)
            {
                throw new NullReferenceException("Canvas is null. This is probably an internal graphics error.");
            }
            
            return surface;
        }

        public void SwapBuffers()
        {
            GLFW.GetWindowSize(_glfwWindow, out int windowWidth, out int windowHeight);
            OS.SoftwareRenderer?.Draw(
                GLFW.GetCocoaWindow(_glfwWindow),
                _pixelDataPtr,
                _width,
                _height,
                _bytesPerRow,
                windowWidth, 
                windowHeight);
        }

        public void DestroyAndTerminate()
        {
            Marshal.FreeCoTaskMem(_pixelDataPtr);
        }

        public void Resized(int width, int height)
        {
            _width = width;
            _height = height;
            OS.SoftwareRenderer?.Resized(width, height);
        }

        public void SwapIntervalChanged(int swapInterval)
        {
            _swapInterval = swapInterval;
        }
    }
}
