using SkiaSharp;

namespace CatUI.RenderingEngine;

/// <summary>
/// A helper used in the Canvas element to facilitate low-level drawing using either helper functions or direct
/// SkiaSharp functions.
/// </summary>
public class CanvasPen
{
    /// <summary>
    /// A reference to the renderer. This contains the actual drawing helpers. The measuring units are absolute,
    /// so always take into account the canvas' bounds when drawing (otherwise the drawing will get clipped).
    /// </summary>
    public Renderer CanvasRenderer { get; }

    /// <summary>
    /// If you set this to true during drawing, it will force the canvas to invoke the drawing function again the next
    /// frame. This will be reset to false before each draw function invocation.
    /// </summary>
    public bool IsDirty { get; set; }

    /// <summary>
    /// The same as <see cref="Renderer.Canvas"/> from <see cref="CanvasRenderer"/>.
    /// </summary>
    public SKCanvas? SkiaCanvas => CanvasRenderer.Canvas;

    public CanvasPen(Renderer renderer)
    {
        CanvasRenderer = renderer;
    }
}
