using System;
using System.IO;
using System.Runtime.InteropServices;
using CatUI.Windowing.Common;
using SkiaSharp;

namespace HeadlessExample;

public unsafe class PngFileGraphicsBackend : IGraphicsBackend
{
    private void* _pixelDataPtr;
    private SKSurface? _surface;
    private int _snapshotIndex;

    private SKSize _lastSize;
    private int _width;
    private int _height;

    public void PrepareWindowCreation()
    {
        //no-op
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
        NativeMemory.Free(_pixelDataPtr);
        SKImageInfo info = new(_width, _height, SKColorType.Rgba8888, SKAlphaType.Premul);
        _pixelDataPtr = NativeMemory.Alloc((nuint)info.BytesSize);

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

        _surface = surface;
        return surface;
    }

    public void PresentFramebuffer()
    {
        if (_surface == null)
        {
            return;
        }

        SKImage snap = _surface.Snapshot();

        using SKData? rawData = snap.Encode();
        using FileStream stream = File.OpenWrite($"snap{_snapshotIndex}.png");
        _snapshotIndex++;
        rawData.SaveTo(stream);
    }

    public void DestroyAndTerminate()
    {
        NativeMemory.Free(_pixelDataPtr);
        _surface?.Dispose();
        _surface = null;
    }

    public void Resized(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void SwapIntervalChanged(int swapInterval)
    {
        //no-op
    }
}
