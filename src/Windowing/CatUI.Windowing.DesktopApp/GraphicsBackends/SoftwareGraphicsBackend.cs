using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CatUI.Platform;
using CatUI.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("linux")]
public unsafe class SoftwareGraphicsBackend : IGraphicsBackend
{
    private Window* _glfwWindow;
    private void* _pixelDataPtr;
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
        if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);

            //1.0 will get the highest possible context, but still compatible with the old OpenGL 1.1 spec
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 1);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 0);

            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Any);
        }
        else
        {
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
        }
    }

    public void PostWindowCreation()
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            GLFW.MakeContextCurrent(_glfwWindow);
        }
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
        NativeMemory.Free(_pixelDataPtr);
        SKImageInfo info = new(_width, _height, SKColorType.Rgba8888, SKAlphaType.Premul);
        _pixelDataPtr = NativeMemory.Alloc((nuint)info.BytesSize);
        _bytesPerRow = info.RowBytes;

        var surface = SKSurface.Create(info, (nint)_pixelDataPtr, info.RowBytes);
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

    public void PresentFramebuffer()
    {
        GLFW.GetWindowSize(_glfwWindow, out int windowWidth, out int windowHeight);

        nint nativeWindowHandle = 0;
        if (OperatingSystem.IsWindows())
        {
            nativeWindowHandle = GLFW.GetWin32Window(_glfwWindow);
        }
        else if (OperatingSystem.IsMacOS())
        {
            nativeWindowHandle = GLFW.GetCocoaWindow(_glfwWindow);
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            nativeWindowHandle = (nint)_glfwWindow;
        }

        OS.SoftwareRenderer?.Draw(
            nativeWindowHandle,
            (nint)_pixelDataPtr,
            _width,
            _height,
            _bytesPerRow,
            windowWidth,
            windowHeight);
    }

    public void DestroyAndTerminate()
    {
        NativeMemory.Free(_pixelDataPtr);
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

        if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
        {
            GLFW.SwapInterval(_swapInterval);
        }
    }
}
