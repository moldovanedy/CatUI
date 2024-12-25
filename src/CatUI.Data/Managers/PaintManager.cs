using CatUI.Data.Enums;

using SkiaSharp;

namespace CatUI.Data.Managers
{
    public static class PaintManager
    {
        public const float DEFAULT_FONT_SIZE = 12;

        public static SKPaint DefaultPainter
        {
            get => new SKPaint()
            {
                Color = new SKColor(0x00_00_00_00),
                TextEncoding = SKTextEncoding.Utf8,
                TextSize = DEFAULT_FONT_SIZE,
                IsAntialias = true,
            };
        }

        public static SKPaint GetPaint(
            PaintMode paintMode = PaintMode.Fill,
            Color? paintColor = null,
            OutlineParams? outlineParams = null,
            float fontSize = 0,
            TextAlignmentType textAlignment = TextAlignmentType.Left)
        {
            SKPaint newPaint = DefaultPainter;
            ModifyPaint(
                paint: newPaint,
                paintMode: paintMode,
                paintColor: paintColor,
                outlineParams: outlineParams,
                fontSize: fontSize,
                textAlignment: textAlignment);
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
                switch (paintMode)
                {
                    default:
                    case PaintMode.Fill:
                        paint.Style = SKPaintStyle.Fill;
                        break;
                    case PaintMode.Outline:
                        paint.Style = SKPaintStyle.Stroke;
                        break;
                    case PaintMode.FillAndOutline:
                        paint.Style = SKPaintStyle.StrokeAndFill;
                        break;
                }
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
                paint.StrokeWidth = outlineParams.OutlineWidth;
                paint.StrokeCap = (SKStrokeCap)outlineParams.LineCap;
                paint.StrokeJoin = (SKStrokeJoin)outlineParams.LineJoin;
                paint.StrokeMiter = outlineParams.MiterLimit;
            }

            if (textAlignment != null && textAlignment != TextAlignmentType.Justify)
            {
                paint.TextAlign = (SKTextAlign)(textAlignment - 1);
            }
        }
    }
}
