using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.RenderingEngine.GraphicsCaching;
using SkiaSharp;

namespace CatUI.RenderingEngine
{
    public class Renderer
    {
        public SKSurface? Surface { get; private set; }
        public SKCanvas? Canvas { get; private set; }
        public GRContext? Context { get; private set; }

        public bool IsCanvasDirty { get; private set; }

        private const SKColorType COLOR_TYPE = SKColorType.Rgba8888;
        private const GRSurfaceOrigin SURFACE_ORIGIN = GRSurfaceOrigin.BottomLeft;

        private GRGlFramebufferInfo _glInfo;
        private GRBackendRenderTarget? _renderTarget;
        private SKSize _lastSize;
        private SKSize _newSize;

        private Color _bgColor;
        private int _framebufferBinding = 0;
        private int _stencilBits = 0;
        private int _samples = 0;
        private float _scale = 1;

        public void SetContentScale(float scale)
        {
            _scale = scale;
        }

        public void SetFramebufferData(int fbBinding, int stencilBits, int samples)
        {
            _framebufferBinding = fbBinding;
            _stencilBits = stencilBits;
            _samples = samples;
        }

        public void SetNewSize(SKSize size)
        {
            _newSize = size;
        }

        public void SetBgColor(Color backgroundColor)
        {
            _bgColor = backgroundColor;
        }

        /// <summary>
        /// Will clear the viewport with the viewport's background color,
        /// but will also take care of recreating the surface if needed (for example on a window resize).
        /// </summary>
        public void ResetAndClear()
        {
            //create the contexts if not done already
            if (Context == null)
            {
                GRGlInterface glInterface = GRGlInterface.Create();
                Context = GRContext.CreateGl(glInterface);
            }

            //manage the drawing surface
            if (_renderTarget == null || _lastSize != _newSize || !_renderTarget.IsValid)
            {
                _lastSize = _newSize;

                int maxSamples = Context.GetMaxSurfaceSampleCount(COLOR_TYPE);
                if (_samples > maxSamples)
                    _samples = maxSamples;
                _glInfo = new GRGlFramebufferInfo((uint)_framebufferBinding, COLOR_TYPE.ToGlSizedFormat());

                // destroy the old surface
                Surface?.Dispose();
                Surface = null;
                Canvas = null;

                // re-create the render target
                _renderTarget?.Dispose();
                _renderTarget = new GRBackendRenderTarget((int)_newSize.Width, (int)_newSize.Height, _samples, _stencilBits, _glInfo);
            }

            //create the surface
            if (Surface == null)
            {
                Surface = SKSurface.Create(Context, _renderTarget, SURFACE_ORIGIN, COLOR_TYPE);
                Canvas = Surface.Canvas;
            }

            ArgumentNullException.ThrowIfNull(Canvas);
            using (new SKAutoCanvasRestore(Canvas, true))
            {
                Canvas.Clear(_bgColor);
            }
        }

        /// <summary>
        /// Flushes the SkiaSharp contents to GL.
        /// </summary>
        public void Flush()
        {
            Canvas?.Flush();
            Context?.Flush();
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

        public int CalculateDimension(Dimension dimension)
        {
            switch (dimension.MeasuringUnit)
            {
                default:
                case Unit.Dp:
                    return (int)(dimension.Value * _scale);
                case Unit.Pixels:
                    return (int)dimension.Value;
                case Unit.Percent:
                    return
                        (int)(dimension.Value *
                        (Canvas?.DeviceClipBounds.Size.Width ?? 0) /
                        100f);
                case Unit.ViewportWidth:
                    return
                        (int)(dimension.Value *
                        (Canvas?.DeviceClipBounds.Size.Width ?? 0) /
                        100f);
                case Unit.ViewportHeight:
                    return
                        (int)(dimension.Value *
                        (Canvas?.DeviceClipBounds.Size.Height ?? 0) /
                        100f);
            }
        }

        #region Drawing
        /// <summary>
        /// Draws a rect directly on the canvas. The corners values are interpreted as pixels regardless of the measuring unit.
        /// Leaving corners to default will draw a sharp rectangle, with no rounded corners.
        /// </summary>
        /// <remarks>
        /// This method can only draw a filled rectangle, for only drawing the outline (aka stroke), use <see cref="DrawRectOutline(Rect, IBrush, CornerInset)"/>
        /// </remarks>
        /// <param name="rect">The direct pixel measurements of the rect.</param>
        /// <param name="fillBrush">The brush to use to paint the rect.</param>
        /// <param name="roundedCorners">
        /// Optionally provide the details for rounded corners. The corners values are interpreted as pixels regardless of the measuring unit.
        /// </param>
        public void DrawRect(Rect rect, IBrush fillBrush, CornerInset roundedCorners = default)
        {
            SKPaint paint;
            if (fillBrush is ColorBrush colorBrush)
            {
                if (!PaintCache.TryGetPaint(colorBrush.Color, PaintMode.Fill, 0f, out paint!))
                {
                    paint = fillBrush.ToSkiaPaint();
                    PaintCache.CacheNewPaint(paint);
                }
            }
            else
            {
                paint = fillBrush.ToSkiaPaint();
            }

            if (roundedCorners.HasNonTrivialValues)
            {
                SKRoundRect roundRect = new SKRoundRect();
                SKPoint[] radii = SetupRectCorners(roundedCorners);
                roundRect.SetRectRadii(rect, radii);
                this.Canvas?.DrawRoundRect(roundRect, paint);
            }
            else
            {
                this.Canvas?.DrawRect(rect, paint);
            }
        }

        /// <summary>
        /// Draws a rect outline (stroke) directly on the canvas. The corners values are interpreted as pixels regardless of the measuring unit.
        /// Leaving corners to default will draw a sharp rectangle, with no rounded corners.
        /// </summary>
        /// <remarks>
        /// This method can only draw a stroked rectangle, for drawing the rectangle as filled, use <see cref="DrawRect(Rect, IBrush, CornerInset)"/>
        /// </remarks>
        /// <param name="rect">The direct pixel measurements of the rect.</param>
        /// <param name="strokeBrush">The brush to use to paint the rect.</param>
        /// <param name="roundedCorners">
        /// Optionally provide the details for rounded corners. The corners values are interpreted as pixels regardless of the measuring unit.
        /// </param>
        /// <param name="outlineWidth">The width of the outline in pixels.</param>
        public void DrawRectOutline(Rect rect, IBrush strokeBrush, float outlineWidth, CornerInset roundedCorners = default)
        {
            SKPaint paint;
            if (strokeBrush is ColorBrush colorBrush)
            {
                if (!PaintCache.TryGetPaint(colorBrush.Color, PaintMode.Stroke, outlineWidth, out paint!))
                {
                    paint = strokeBrush.ToSkiaPaint();
                    PaintCache.CacheNewPaint(paint);
                }
            }
            else
            {
                paint = strokeBrush.ToSkiaPaint();
            }

            if (roundedCorners.HasNonTrivialValues)
            {
                SKRoundRect roundRect = new SKRoundRect();
                SKPoint[] radii = SetupRectCorners(roundedCorners);
                roundRect.SetRectRadii(rect, radii);

                this.Canvas?.DrawRoundRect(roundRect, paint);
            }
            else
            {
                this.Canvas?.DrawRect(rect, paint);
            }
        }

        public void DrawEllipse(Point2D center, float rx, float ry, IBrush fillBrush)
        {
            SKPaint paint;
            if (fillBrush is ColorBrush colorBrush)
            {
                if (!PaintCache.TryGetPaint(colorBrush.Color, PaintMode.Fill, 0f, out paint!))
                {
                    paint = fillBrush.ToSkiaPaint();
                    PaintCache.CacheNewPaint(paint);
                }
            }
            else
            {
                paint = fillBrush.ToSkiaPaint();
            }

            this.Canvas?.DrawOval(center.X, center.Y, rx, ry, paint);
        }

        public void DrawEllipseOutline(Point2D center, float rx, float ry, IBrush fillBrush, float outlineWidth)
        {
            SKPaint paint;
            if (fillBrush is ColorBrush colorBrush)
            {
                if (!PaintCache.TryGetPaint(colorBrush.Color, PaintMode.Stroke, outlineWidth, out paint!))
                {
                    paint = fillBrush.ToSkiaPaint();
                    PaintCache.CacheNewPaint(paint);
                }
            }
            else
            {
                paint = fillBrush.ToSkiaPaint();
            }

            this.Canvas?.DrawOval(center.X, center.Y, rx, ry, paint);
        }

        /// <summary>
        /// Draws the specified text until the text will exceed the width of the element size.
        /// This means that this method MIGHT NOT render the whole text, but will return the number of characters rendered.
        /// </summary>
        /// <param name="text">The text to render. This method MIGHT NOT render the whole text.</param>
        /// <param name="topLeftPoint">The top-left point of the text that needs to be drawn.</param>
        /// <param name="fontSize">The font size of the text.</param>
        /// <param name="elementSize">
        /// The size of the element that contains the text.
        /// Use a very large width in order to guarantee the rendering of the whole text (this will use word wrap).
        /// This method does NOT account for vertical size, so vertical overflow is possible.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The text alignment to use. <see cref="HorizontalAlignmentType.Stretch"/> won't have any effect
        /// and will work as <see cref="HorizontalAlignmentType.Left"/>.
        /// </param>
        /// <param name="breakMode">Specifies the text break mode. See <see cref="TextBreakMode"/> for more information.</param>
        /// <param name="hyphenCharacter">Specifies the character used as a hyphen if necessary. For no hyphens, set this to the null character.</param>
        /// <returns>The number of characters drawn.</returns>
        /// <exception cref="ArgumentException">Thrown if the text contains an invalid newline (\r instead of \n or \r\n).</exception>
        public int DrawTextRow(
            string text,
            Point2D topLeftPoint,
            Dimension fontSize,
            Size elementSize,
            Color color,
            HorizontalAlignmentType horizontalAlignment = HorizontalAlignmentType.Left,
            TextBreakMode breakMode = TextBreakMode.SoftBreak,
            char hyphenCharacter = '-')
        {
            if (!PaintCache.TryGetFontPaint(
                    CalculateDimension(fontSize),
                    color,
                    PaintMode.Fill,
                    horizontalAlignment,
                    0f,
                    out SKPaint? painter))
            {
                painter = PaintCache.DefaultPainter.Clone();
                if (horizontalAlignment != HorizontalAlignmentType.Stretch)
                {
                    painter.TextAlign = (SKTextAlign)(horizontalAlignment - 1);
                }
                painter.TextSize = CalculateDimension(fontSize);
                painter.Color = color;

                PaintCache.CacheNewPaint(painter);
            }


            float drawPointX = topLeftPoint.X;
            if (horizontalAlignment == HorizontalAlignmentType.Center)
            {
                drawPointX += elementSize.Width / 2;
            }
            else if (horizontalAlignment == HorizontalAlignmentType.Right)
            {
                drawPointX += elementSize.Width;
            }
            SKPoint drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);

            StringBuilder sb = new StringBuilder();
            List<int> shyPositions = new List<int>();

            bool hasNewLine = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' && i == text.Length - 1)
                {
                    throw new ArgumentException("Invalid text: found CR (\\r) without LF (\\n)", text);
                }

                if (text[i] == '\n' || (text[i] == '\r' && text[i + 1] == '\n'))
                {
                    hasNewLine = true;
                    break;
                }
                else if (text[i] == '\u00ad')
                {
                    shyPositions.Add(sb.Length);
                    continue;
                }

                sb.Append(text[i]);
            }

            string drawableText = sb.ToString();
            bool needsHyphen = false;
            int charactersDrawn = 0, charsOnThisRow = drawableText.Length;
            if (hasNewLine)
            {
                goto Drawing;
            }

            charsOnThisRow = (int)painter!.BreakText(drawableText, elementSize.Width);
            if (charsOnThisRow <= 0)
            {
                charsOnThisRow = 1;
            }

            if (charsOnThisRow < drawableText.Length)
            {
                //if the last drawable character or the next one is a space, no hyphenation is necessary;
                //else, use hyphenation
                if (drawableText[charsOnThisRow - 1] != ' ' &&
                    drawableText[Math.Min(charsOnThisRow, drawableText.Length - 1)] != ' ')
                {
                    if (breakMode == TextBreakMode.NoBreak)
                    {
                        int spaceChar;
                        for (spaceChar = charsOnThisRow - 1; spaceChar > 0; spaceChar--)
                        {
                            if (drawableText[spaceChar] == ' ')
                            {
                                break;
                            }
                        }

                        //if we found a space, let the word pass to the next row and skip the space
                        if (spaceChar != 0)
                        {
                            charsOnThisRow = spaceChar;
                        }
                    }
                    else if (breakMode == TextBreakMode.SoftBreak)
                    {
                        int shyChar;
                        for (shyChar = Math.Max(charsOnThisRow - 1, 0); shyChar > 0; shyChar--)
                        {
                            if (shyPositions.Contains(shyChar) || drawableText[shyChar] == ' ')
                            {
                                break;
                            }
                        }

                        //if we found a shy character, let the portion of the word pass to the next row and skip the shy character
                        if (shyChar != 0)
                        {
                            charsOnThisRow = shyChar;
                            if (drawableText[shyChar] != ' ')
                            {
                                needsHyphen = true;
                            }
                            else
                            {
                                //skip the space so as to not show it on the next row
                                charsOnThisRow++;
                            }
                        }
                    }
                    else if (breakMode == TextBreakMode.HardBreak)
                    {
                        charsOnThisRow--;
                        needsHyphen = true;
                    }
                }
                //if the next character is a space, skip it so as to not show it on the next row
                else if (drawableText[charsOnThisRow] == ' ')
                {
                    charsOnThisRow++;
                }
            }

        Drawing:
            if (needsHyphen)
            {
                string newString = new string(drawableText.AsSpan(charactersDrawn, charsOnThisRow));
                this.Canvas?.DrawText(newString + hyphenCharacter, drawPoint, painter);
            }
            else
            {
                this.Canvas?.DrawText(drawableText.Substring(charactersDrawn, charsOnThisRow), drawPoint, painter);
            }
            charactersDrawn += charsOnThisRow;

            //also add all the shy characters in the count
            for (int i = 0; i < shyPositions.Count; i++)
            {
                if (shyPositions[i] < charsOnThisRow)
                {
                    charactersDrawn++;
                }
                else
                {
                    break;
                }
            }

            return charactersDrawn;
        }

        /// <summary>
        /// Draws the specified text until a newline is found or until the element size limit is reached and overflowMode
        /// is <see cref="TextOverflowMode.Ellipsis"/> or <see cref="TextOverflowMode.Clip"/>.
        /// </summary>
        /// <param name="text">The text to render. This method MIGHT NOT render the whole text.</param>
        /// <param name="topLeftPoint">The top-left point of the text that needs to be drawn.</param>
        /// <param name="fontSize">The font size of the text.</param>
        /// <param name="elementSize">
        /// The size of the element that contains the text. This method does not use word wrapping.
        /// This method does NOT account for vertical size, so vertical overflow is possible.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The text alignment to use. <see cref="HorizontalAlignmentType.Stretch"/> won't have any effect
        /// and will work as <see cref="HorizontalAlignmentType.Left"/>.
        /// </param>
        /// <param name="overflowMode">Specifies the text overflow behavior.</param>
        /// <param name="ellipsisStringOverride">
        /// An alternative string to use instead of \u2026 when overflowMode is <see cref="TextOverflowMode.Ellipsis"/>.
        /// Setting this to null (default) will display the \u2026 when ellipsis is necessary.
        /// </param>
        /// <returns>The number of characters drawn.</returns>
        /// <exception cref="ArgumentException">Thrown if the text contains an invalid newline (\r instead of \n or \r\n).</exception>
        public int DrawTextRow(
            string text,
            Point2D topLeftPoint,
            Dimension fontSize,
            Size elementSize,
            Color color,
            HorizontalAlignmentType horizontalAlignment = HorizontalAlignmentType.Left,
            TextOverflowMode overflowMode = TextOverflowMode.Ellipsis,
            string? ellipsisStringOverride = null)
        {
            if (!PaintCache.TryGetFontPaint(
                    CalculateDimension(fontSize),
                    color,
                    PaintMode.Fill,
                    horizontalAlignment,
                    0f,
                    out SKPaint? painter))
            {
                painter = PaintCache.DefaultPainter.Clone();
                if (horizontalAlignment != HorizontalAlignmentType.Stretch)
                {
                    painter.TextAlign = (SKTextAlign)(horizontalAlignment - 1);
                }
                painter.TextSize = CalculateDimension(fontSize);
                painter.Color = color;

                PaintCache.CacheNewPaint(painter);
            }

            float drawPointX = topLeftPoint.X;
            if (horizontalAlignment == HorizontalAlignmentType.Center)
            {
                drawPointX += elementSize.Width / 2;
            }
            else if (horizontalAlignment == HorizontalAlignmentType.Right)
            {
                drawPointX += elementSize.Width;
            }
            SKPoint drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\u00ad')
                {
                    sb.Append(text[i]);
                }
            }
            text = sb.ToString();

            if (overflowMode == TextOverflowMode.Overflow)
            {
                this.Canvas?.DrawText(text, drawPoint, painter);
                return text.Length;
            }

            string ellipsisString;
            if (ellipsisStringOverride != null)
            {
                ellipsisString = ellipsisStringOverride;
            }
            else
            {
                ellipsisString = "\u2026";
            }

            float ellipsisSize = painter!.MeasureText(ellipsisString);
            //exit early
            if (elementSize.Width < ellipsisSize && overflowMode == TextOverflowMode.Ellipsis)
            {
                this.Canvas?.DrawText(ellipsisString, drawPoint, painter);
                return 0;
            }

            int newLinePosition = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' && i == text.Length - 1)
                {
                    throw new ArgumentException("Invalid text: found CR (\\r) without LF (\\n)", text);
                }

                if (text[i] == '\n' || (text[i] == '\r' && text[i + 1] == '\n'))
                {
                    newLinePosition = i;
                    break;
                }
            }

            if (newLinePosition != -1)
            {
                text = text.Substring(0, newLinePosition);
            }

            if (overflowMode == TextOverflowMode.Ellipsis)
            {
                long charactersToDraw = painter!.BreakText(text, elementSize.Width);
                if (charactersToDraw == text.Length)
                {
                    this.Canvas?.DrawText(text, drawPoint, painter);
                }
                else
                {
                    charactersToDraw = painter!.BreakText(text, elementSize.Width - ellipsisSize);
                    text = text.Substring(0, (int)charactersToDraw);

                    this.Canvas?.DrawText(text + ellipsisString, drawPoint, painter);
                }

                return text.Length;
            }
            else if (overflowMode == TextOverflowMode.Clip)
            {
                //TODO
            }

            return 0;
        }
        #endregion

        private static SKPoint[] SetupRectCorners(CornerInset roundedCorners)
        {
            SKPoint[] radii = new SKPoint[4];

            if (roundedCorners.TopLeftEllipse.IsUnset())
            {
                if (roundedCorners.TopLeft.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.TopLeft.Value, roundedCorners.TopLeft.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.TopLeftEllipse.X.Value,
                        roundedCorners.TopLeftEllipse.Y.Value);
            }
            if (roundedCorners.TopRightEllipse.IsUnset())
            {
                if (roundedCorners.TopRight.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.TopRight.Value, roundedCorners.TopRight.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.TopRightEllipse.X.Value,
                        roundedCorners.TopRightEllipse.Y.Value);
            }
            if (roundedCorners.BottomRightEllipse.IsUnset())
            {
                if (roundedCorners.BottomRight.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.BottomRight.Value, roundedCorners.BottomRight.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.BottomRightEllipse.X.Value,
                        roundedCorners.BottomRightEllipse.Y.Value);
            }
            if (roundedCorners.BottomLeftEllipse.IsUnset())
            {
                if (roundedCorners.BottomLeft.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.BottomLeft.Value, roundedCorners.BottomLeft.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.BottomLeftEllipse.X.Value,
                        roundedCorners.BottomLeftEllipse.Y.Value);
            }

            return radii;
        }
    }
}
