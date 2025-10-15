using System;
using CatUI.Data;
using SkiaSharp;

namespace CatUI.RenderingEngine;

public partial class Renderer
{
    /// <summary>
    /// True when you can safely use <see cref="Surface"/> and <see cref="Canvas"/>, false otherwise.
    /// </summary>
    public bool CanDraw { get; private set; }

    /// <summary>
    /// Represents the SkiaSharp surface where the drawing happens. Its canvas is stored in <see cref="Canvas"/>.
    /// This is only safe to use when the drawing actually happens, that is when <see cref="CanDraw"/> is true.
    /// </summary>
    public SKSurface? Surface { get; private set; }

    /// <summary>
    /// Represents the canvas where all content is drawn. Use this for pretty much any drawing.
    /// This is only safe to use when the drawing actually happens, that is when <see cref="CanDraw"/> is true.
    /// </summary>
    public SKCanvas? Canvas { get; private set; }

    public bool IsCanvasDirty { get; private set; }


    private Color _bgColor;


    /// <summary>
    /// You must only call this inside internal window managers (e.g. DesktopWindow, AndroidWindow) and never
    /// from UI code. This is called before any drawing can happen.
    /// </summary>
    public void BeginDraw()
    {
        CanDraw = true;
    }

    /// <summary>
    /// You must only call this inside internal window managers (e.g. DesktopWindow, AndroidWindow) and never
    /// from UI code. This is called after all drawing operations are finished.
    /// </summary>
    public void EndDraw()
    {
        CanDraw = false;
    }

    /// <summary>
    /// This method should be called always before <see cref="ResetAndClear"/>, this is generally
    /// called whenever SkiaSharp.Views paint event is fired or when the surface needs to be recreated
    /// (when the hardware drawing is done manually, like on desktop).
    /// </summary>
    /// <remarks>
    /// You can still call this when the rendering is not managed by SkiaSharp.Views, but then you are completely
    /// responsible for the drawing, as <see cref="ResetAndClear"/> will still consider it has control over the
    /// context, so it will still use the framebuffer data you give.
    /// </remarks>
    /// <param name="surface"></param>
    /// <param name="canvas"></param>
    public void SetPlatformManagedData(SKSurface? surface, SKCanvas? canvas)
    {
        Surface = surface;
        Canvas = canvas;
    }

    public void SetBgColor(Color backgroundColor)
    {
        _bgColor = backgroundColor;
    }

    /// <summary>
    /// Will clear the viewport with the viewport's background color.
    /// </summary>
    public void ResetAndClear()
    {
        ArgumentNullException.ThrowIfNull(Canvas);
        using (new SKAutoCanvasRestore(Canvas, true))
        {
            Canvas.Clear(_bgColor);
        }
    }


    /// <summary>
    /// Flushes the SkiaSharp contents to the screen.
    /// </summary>
    public void Flush()
    {
        Canvas?.Flush();
    }

    public void SetCanvasDirty()
    {
        IsCanvasDirty = true;
    }

    /// <summary>
    /// Will make the canvas appear "clean" by setting <see cref="IsCanvasDirty"/> to false.
    /// Should only be called by the internal windowing system when the updated interface is presented
    /// or in special circumstances when you simply don't want to present the updated interface.
    /// </summary>
    /// <remarks>
    /// Will not stop the redrawing internally, so there aren't any performance benefits from calling this.
    /// </remarks>
    public void SkipCanvasPresentation()
    {
        IsCanvasDirty = false;
    }

    #region Layered drawing

    /// <summary>
    /// Analog to <see cref="SKCanvas.Save"/>. Pushes the current canvas state on a stack.
    /// </summary>
    /// <returns>The value that should be given to <see cref="RestoreCanvasState(int)"/> to return to this state.</returns>
    /// <exception cref="NullReferenceException">If <see cref="Canvas"/> is null.</exception>
    public int SaveCanvasState()
    {
        if (Canvas == null)
        {
            throw new NullReferenceException("Canvas was null.");
        }

        return Canvas.Save();
    }

    /// <summary>
    /// Analog to <see cref="SKCanvas.RestoreToCount"/>. Restores the canvas state to the given state on the stack.
    /// Use <see cref="RestoreCanvasState()"/> to only go back one state.
    /// </summary>
    /// <param name="initialState">The state to return to. It's the value returned by <see cref="SaveCanvasState"/>.</param>
    /// <exception cref="NullReferenceException">If <see cref="Canvas"/> is null.</exception>
    public void RestoreCanvasState(int initialState)
    {
        if (Canvas == null)
        {
            throw new NullReferenceException("Canvas was null.");
        }

        Canvas.RestoreToCount(initialState);
    }

    /// <summary>
    /// Analog to <see cref="SKCanvas.Restore"/>. Restores the previous canvas state (the one before the call to
    /// <see cref="SaveCanvasState"/>).
    /// </summary>
    /// <exception cref="NullReferenceException">If <see cref="Canvas"/> is null.</exception>
    public void RestoreCanvasState()
    {
        if (Canvas == null)
        {
            throw new NullReferenceException("Canvas was null.");
        }

        Canvas.Restore();
    }

    #endregion

    #region Clipping

    /// <summary>
    /// Sets the clip region as the given rect. The coordinates are absolute pixel coordinates.
    /// </summary>
    /// <param name="clipRect"></param>
    public void SetClipRect(Rect clipRect)
    {
        Canvas?.ClipRect(clipRect, SKClipOperation.Intersect, true);
    }

    /// <summary>
    /// Sets the clip region as the given path. It is generally as fast as <see cref="SetClipRect"/> for simple
    /// shapes (circle, ellipse, rect), but gets significantly slower for complex paths.
    /// The coordinates are absolute pixel coordinates.
    /// </summary>
    /// <param name="clipPath">The Skia clip path.</param>
    public void SetClipPath(SKPath clipPath)
    {
        Canvas?.ClipPath(clipPath, SKClipOperation.Intersect, true);
    }

    #endregion
}
