using CatUI.Data.Assets;
using CatUI.Data.Enums;
using SkiaSharp;

namespace CatUI.Data.Managers
{
    public static class PaintManager
    {
        public const float DEFAULT_FONT_SIZE = 16;

        /// <summary>
        /// Returns a new SKPaint that has a completely transparent color, uses the <see cref="DEFAULT_FONT_SIZE"/>,
        /// uses antialiasing and subpixel rendering.
        /// </summary>
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
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            FontAsset? font = null)
        {
            SKPaint newPaint = DefaultPainter;
            ModifyPaint(
                newPaint,
                paintMode,
                paintColor,
                outlineParams,
                fontSize,
                textAlignment,
                font);
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
        /// <param name="fontSize"></param>
        /// <param name="textAlignment"></param>
        /// <param name="font"></param>
        public static void ModifyPaint(
            SKPaint paint,
            PaintMode? paintMode = PaintMode.Fill,
            Color? paintColor = null,
            OutlineParams? outlineParams = null,
            float fontSize = 0,
            TextAlignmentType? textAlignment = TextAlignmentType.Left,
            FontAsset? font = null)
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

            if (font != null)
            {
                paint.Typeface = font.SkiaFont;
            }
        }
    }
}
