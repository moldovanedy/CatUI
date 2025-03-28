using CatUI.Data.Enums;
using SkiaSharp;

namespace CatUI.Data.Managers
{
    public static class PaintManager
    {
        public const float DEFAULT_FONT_SIZE = 12;

        public static SKPaint DefaultPainter =>
            new()
            {
                Color = new SKColor(0x00_00_00_00),
                TextEncoding = SKTextEncoding.Utf8,
                TextSize = DEFAULT_FONT_SIZE,
                IsAntialias = true,
                SubpixelText = true
            };

        public static SKPaint GetPaint(
            PaintMode paintMode = PaintMode.Fill,
            Color? paintColor = null,
            OutlineParams? outlineParams = null,
            float fontSize = 0,
            TextAlignmentType textAlignment = TextAlignmentType.Left)
        {
            SKPaint newPaint = DefaultPainter;
            ModifyPaint(
                newPaint,
                paintMode,
                paintColor,
                outlineParams,
                fontSize,
                textAlignment);
            return newPaint;
        }

        public static void ModifyPaint(
            SKPaint paint,
            PaintMode? paintMode = PaintMode.Fill,
            Color? paintColor = null,
            OutlineParams? outlineParams = null,
            float fontSize = 0,
            TextAlignmentType? textAlignment = TextAlignmentType.Left)
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

            if (fontSize != 0)
            {
                paint.TextSize = fontSize;
            }

            if (outlineParams != null)
            {
                paint.StrokeWidth = outlineParams.Value.OutlineWidth;
                paint.StrokeCap = (SKStrokeCap)outlineParams.Value.LineCap;
                paint.StrokeJoin = (SKStrokeJoin)outlineParams.Value.LineJoin;
                paint.StrokeMiter = outlineParams.Value.MiterLimit;
            }

            if (textAlignment != null && textAlignment != TextAlignmentType.Justify)
            {
                paint.TextAlign = (SKTextAlign)(textAlignment - 1);
            }
        }
    }
}
