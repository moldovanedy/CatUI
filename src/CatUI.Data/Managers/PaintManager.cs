using CatUI.Data.Enums;
using SkiaSharp;

namespace CatUI.Data.Managers;

public static class PaintManager
{
    public const float DEFAULT_FONT_SIZE = 16;

    /// <summary>
    /// Returns a new SKPaint that has a completely transparent color, uses the <see cref="DEFAULT_FONT_SIZE"/>,
    /// uses antialiasing and subpixel rendering.
    /// </summary>
    public static SKPaint DefaultPainter =>
        new() { Color = new SKColor(0x00_00_00_00), IsAntialias = true };

    public static SKPaint GetPaint(
        PaintMode paintMode = PaintMode.Fill,
        Color? paintColor = null,
        OutlineParams? outlineParams = null)
    {
        SKPaint newPaint = DefaultPainter;
        ModifyPaint(
            newPaint,
            paintMode,
            paintColor,
            outlineParams);
        return newPaint;
    }

    /// <summary>
    /// Modifies the given paint by setting the given properties to it. The modifications are additive: if the paint
    /// already has a property set (either by a previous call to this or a direct assignment), unless that parameter
    /// does not have the default value, it won't affect that property.
    /// </summary>
    /// <param name="paint"></param>
    /// <param name="paintMode"></param>
    /// <param name="paintColor"></param>
    /// <param name="outlineParams"></param>
    public static void ModifyPaint(
        SKPaint paint,
        PaintMode? paintMode = PaintMode.Fill,
        Color? paintColor = null,
        OutlineParams? outlineParams = null)
    {
        if (paintMode != null)
        {
            paint.Style = paintMode switch
            {
                PaintMode.Outline => SKPaintStyle.Stroke,
                PaintMode.FillAndOutline => SKPaintStyle.StrokeAndFill,
                _ => SKPaintStyle.Fill
            };
        }

        if (paintColor != null)
        {
            paint.Color = (SKColor)paintColor;
        }

        if (outlineParams != null)
        {
            paint.StrokeWidth = outlineParams.Value.OutlineWidth;
            paint.StrokeCap = (SKStrokeCap)outlineParams.Value.LineCap;
            paint.StrokeJoin = (SKStrokeJoin)outlineParams.Value.LineJoin;
            paint.StrokeMiter = outlineParams.Value.MiterLimit;
        }
    }
}
