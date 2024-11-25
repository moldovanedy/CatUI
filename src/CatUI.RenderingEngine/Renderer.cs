using System;
using System.Collections.Generic;
using System.Text;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Shared;
using SkiaSharp;

namespace CatUI.Data
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

        private Color _bgColor = new Color(0);
        private int _framebufferBinding;
        private int _stencilBits;
        private int _samples;
        private float _scale = 1;

        public void SetContentScale(float scale)
        {
            _scale = scale;
        }

        public float GetContentScale()
        {
            return _scale;
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

        public int CalculateDimension(Dimension dimension, float dimensionForPercent = 0)
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
                        (dimensionForPercent == 0 ?
                            Canvas?.DeviceClipBounds.Size.Width ?? 0 :
                            dimensionForPercent) /
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

        /// <summary>
        /// Will heuristically estimate the average size of a character using the given the paint. The estimate is smaller 
        /// than an actual character width (generally half) to ensure that an eventual text span to not be smaller 
        /// than the actual size of the element (although this estimation reduces that risk significantly, 
        /// it does NOT GUARANTEE that this won't happen).
        /// </summary>
        /// <param name="largeText">The large text string.</param>
        /// <param name="painter">The paint that will be used to draw the text.</param>
        /// <returns>The estimated average size of a character (estimation will generally be half of the actual average).</returns>
        public static float EstimateCharSizeSafe(string largeText, SKPaint painter)
        {
            return EstimateCharSizeSafe(largeText.AsSpan(), painter);
        }

        /// <summary>
        /// Will heuristically estimate the average size of a character using the given the paint. The estimate is smaller 
        /// than an actual character width (generally half) to ensure that an eventual text span to not be smaller 
        /// than the actual size of the element (although this estimation reduces that risk significantly, 
        /// it does NOT GUARANTEE that this won't happen).
        /// </summary>
        /// <param name="largeText">The large text as a character span.</param>
        /// <param name="painter">The paint that will be used to draw the text.</param>
        /// <returns>The estimated average size of a character (estimation will generally be half of the actual average).</returns>
        public static float EstimateCharSizeSafe(ReadOnlySpan<char> largeText, SKPaint painter)
        {
            if (largeText.Length <= 5)
            {
                return painter.MeasureText(largeText) * 0.4f;
            }

            int upperLimit = largeText.Length;
            int minMeasuredCharacters = Math.Clamp(upperLimit / 10, 3, 20);
            int maxMeasuredCharacters = Math.Clamp(upperLimit / 10, 5, 35);
            Random rand = new Random();

            int values;
            float sum = 0;
            int limit = rand.Next(minMeasuredCharacters, maxMeasuredCharacters);
            for (values = 0; values < limit; values++)
            {
                int index = rand.Next(0, upperLimit - 1);
                sum += painter.MeasureText(largeText.Slice(index, 1));
            }

            int normalizedCharNumber = Math.Clamp(upperLimit, 100, 400);
            float multiplicationFactor = NumberUtils.Remap(normalizedCharNumber, 100, 400, 0.35f, 0.6f);
            //invert the scale (so small char number will have high multiplication factor)
            multiplicationFactor = 0.6f - multiplicationFactor + 0.35f;

            return sum / values * multiplicationFactor;
        }

        #region Drawing
        /// <summary>
        /// Draws a rect directly on the canvas. The corners values are interpreted as pixels regardless of the measuring unit.
        /// Leaving corners to default will draw a sharp rectangle, with no rounded corners.
        /// </summary>
        /// <remarks>
        /// This method can only draw a filled rectangle, for only drawing the outline (aka outline), use <see cref="DrawRectOutline(Rect, IBrush, CornerInset)"/>
        /// </remarks>
        /// <param name="rect">The direct pixel measurements of the rect.</param>
        /// <param name="fillBrush">The brush to use to paint the rect.</param>
        /// <param name="roundedCorners">
        /// Optionally provide the details for rounded corners. The corners values are interpreted as pixels regardless of the measuring unit.
        /// </param>
        public void DrawRect(Rect rect, IBrush fillBrush, CornerInset? roundedCorners = null)
        {
            SKPaint paint = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(paint: paint, paintMode: PaintMode.Fill);

            if (roundedCorners != null && roundedCorners.HasNonTrivialValues)
            {
                SKRoundRect roundRect = new SKRoundRect();
                SKPoint[] radii = SetupRectCorners(roundedCorners);
                roundRect.SetRectRadii(rect, radii);
                Canvas?.DrawRoundRect(roundRect, paint);
            }
            else
            {
                Canvas?.DrawRect(rect, paint);
            }
        }

        /// <summary>
        /// Draws a rect outline (outline) directly on the canvas. The corners values are interpreted as pixels regardless of the measuring unit.
        /// Leaving corners to default will draw a sharp rectangle, with no rounded corners.
        /// </summary>
        /// <remarks>
        /// This method can only draw a outlined rectangle, for drawing the rectangle as filled, use <see cref="DrawRect(Rect, IBrush, CornerInset)"/>
        /// </remarks>
        /// <param name="rect">The direct pixel measurements of the rect.</param>
        /// <param name="outlineBrush">The brush to use to paint the rect.</param>
        /// <param name="roundedCorners">
        /// Optionally provide the details for rounded corners. The corners values are interpreted as pixels regardless of the measuring unit.
        /// </param>
        /// <param name="outlineWidth">The width of the outline in pixels.</param>
        public void DrawRectOutline(Rect rect, IBrush outlineBrush, OutlineParams outlineParams, CornerInset? roundedCorners = null)
        {
            SKPaint paint = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(paint: paint, paintMode: PaintMode.Outline, outlineParams: outlineParams);

            if (roundedCorners != null && roundedCorners.HasNonTrivialValues)
            {
                SKRoundRect roundRect = new SKRoundRect();
                SKPoint[] radii = SetupRectCorners(roundedCorners);
                roundRect.SetRectRadii(rect, radii);

                Canvas?.DrawRoundRect(roundRect, paint);
            }
            else
            {
                Canvas?.DrawRect(rect, paint);
            }
        }

        public void DrawEllipse(Point2D center, float rx, float ry, IBrush fillBrush)
        {
            SKPaint paint = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(paint: paint, paintMode: PaintMode.Fill);
            Canvas?.DrawOval(center.X, center.Y, rx, ry, paint);
        }

        public void DrawEllipseOutline(Point2D center, float rx, float ry, IBrush outlineBrush, OutlineParams outlineParams)
        {
            SKPaint paint = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(
                paint: paint,
                paintMode: PaintMode.Outline,
                outlineParams: outlineParams);

            Canvas?.DrawOval(center.X, center.Y, rx, ry, paint);
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
        /// <param name="cachedMaxCharacters">
        /// If larger than 0, will use this value instead of using more expensive calculations with Skia's BreakText functions.
        /// If you already called BreakText and didn't modify the paint, you can safely pass the result here to avoid another call to BreakText.
        /// </param>
        /// <returns>The number of characters drawn.</returns>
        /// <exception cref="ArgumentException">Thrown if the text contains an invalid newline (\r instead of \n or \r\n).</exception>
        public int DrawTextRow(
            string text,
            Point2D topLeftPoint,
            Dimension fontSize,
            Size elementSize,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextBreakMode breakMode = TextBreakMode.SoftBreak,
            char hyphenCharacter = '-',
            int cachedMaxCharacters = 0)
        {
            SKPaint? painter;

            if (fillBrush == null && outlineBrush == null)
            {
                return 0;
            }
            //fill, but no outline
            else if (
                fillBrush != null && !fillBrush.IsSkippable &&
                (outlineBrush == null || outlineBrush.IsSkippable))
            {
                painter = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.Fill,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            //outline, but no fill
            else if (
                outlineBrush != null && !outlineBrush.IsSkippable &&
                (fillBrush == null || fillBrush.IsSkippable))
            {
                painter = outlineBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.Outline,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            //both fill and outline
            else if (
                outlineBrush != null && !outlineBrush.IsSkippable &&
                fillBrush != null && !fillBrush.IsSkippable)
            {
                painter = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.FillAndOutline,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            else
            {
                return 0;
            }

            float drawPointX = topLeftPoint.X;
            if (textAlignment == TextAlignmentType.Center)
            {
                drawPointX += elementSize.Width / 2;
            }
            else if (textAlignment == TextAlignmentType.Right)
            {
                drawPointX += elementSize.Width;
            }
            SKPoint drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);

            StringBuilder sb = new StringBuilder();
            List<int> shyPositions = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' && i == text.Length - 1)
                {
                    throw new ArgumentException("Invalid text: found CR (\\r) without LF (\\n)", text);
                }

                if (text[i] == '\n' || (text[i] == '\r' && text[i + 1] == '\n'))
                {
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
            int charactersDrawn = 0, charsOnThisRow;

            if (cachedMaxCharacters > 0)
            {
                charsOnThisRow = cachedMaxCharacters;
            }
            else
            {
                charsOnThisRow = (int)painter!.BreakText(drawableText, elementSize.Width);
            }

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

            //actual drawing
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
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode overflowMode = TextOverflowMode.Ellipsis,
            string? ellipsisStringOverride = null)
        {
            SKPaint? painter;

            if (fillBrush == null && outlineBrush == null)
            {
                return 0;
            }
            //fill, but no outline
            else if (
                fillBrush != null && !fillBrush.IsSkippable &&
                (outlineBrush == null || outlineBrush.IsSkippable))
            {
                painter = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.Fill,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            //outline, but no fill
            else if (
                outlineBrush != null && !outlineBrush.IsSkippable &&
                (fillBrush == null || fillBrush.IsSkippable))
            {
                painter = outlineBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.Outline,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            //both fill and outline
            else if (
                outlineBrush != null && !outlineBrush.IsSkippable &&
                fillBrush != null && !fillBrush.IsSkippable)
            {
                painter = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.FillAndOutline,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            else
            {
                return 0;
            }

            float drawPointX = topLeftPoint.X;
            if (textAlignment == TextAlignmentType.Center)
            {
                drawPointX += elementSize.Width / 2;
            }
            else if (textAlignment == TextAlignmentType.Right)
            {
                drawPointX += elementSize.Width;
            }
            SKPoint drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);

            bool hasHyphens = TextUtils.RemoveSoftHyphens(text, out string textWithoutHyphens);
            if (hasHyphens)
            {
                text = textWithoutHyphens;
            }

            if (overflowMode == TextOverflowMode.Overflow)
            {
                Canvas?.DrawText(text, drawPoint, painter);
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
                Canvas?.DrawText(ellipsisString, drawPoint, painter);
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
                    Canvas?.DrawText(text, drawPoint, painter);
                }
                else
                {
                    charactersToDraw = painter!.BreakText(text, elementSize.Width - ellipsisSize);
                    text = text.Substring(0, (int)charactersToDraw);

                    Canvas?.DrawText(text + ellipsisString, drawPoint, painter);
                }

                return text.Length;
            }
            else if (overflowMode == TextOverflowMode.Clip)
            {
                //TODO
            }

            return 0;
        }

        /// <summary>
        /// Draws the specified text on one row without doing any checks or measurements for better performance. 
        /// Only use this on sanitized text (no newlines, hyphens (because they will be drawn directly) 
        /// or any kind of control characters, as the text will be drawn directly) and when you are sure that the text 
        /// is not going to overflow the parent element or that overflowing doesn't matter.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="topLeftPoint"></param>
        /// <param name="fontSize"></param>
        /// <param name="elementSize"></param>
        /// <param name="fillBrush"></param>
        /// <param name="outlineBrush"></param>
        /// <param name="textAlignment"></param>
        public void DrawTextRowFast(
            string text,
            Point2D topLeftPoint,
            Dimension fontSize,
            Size elementSize,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            TextAlignmentType textAlignment = TextAlignmentType.Left)
        {
            SKPaint? painter;

            if (fillBrush == null && outlineBrush == null)
            {
                return;
            }
            //fill, but no outline
            else if (
                fillBrush != null && !fillBrush.IsSkippable &&
                (outlineBrush == null || !outlineBrush.IsSkippable))
            {
                painter = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.Fill,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            //outline, but no fill
            else if (
                outlineBrush != null && !outlineBrush.IsSkippable &&
                (fillBrush == null || !fillBrush.IsSkippable))
            {
                painter = outlineBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.Outline,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            //both fill and outline
            else if (
                outlineBrush != null && !outlineBrush.IsSkippable &&
                fillBrush != null && !fillBrush.IsSkippable)
            {
                painter = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: painter,
                    paintMode: PaintMode.FillAndOutline,
                    textAlignment: textAlignment,
                    fontSize: CalculateDimension(fontSize));
            }
            else
            {
                return;
            }

            float drawPointX = topLeftPoint.X;
            if (textAlignment == TextAlignmentType.Center)
            {
                drawPointX += elementSize.Width / 2;
            }
            else if (textAlignment == TextAlignmentType.Right)
            {
                drawPointX += elementSize.Width;
            }
            SKPoint drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);

            Canvas?.DrawText(text, drawPoint, painter);
        }

        /// <summary>
        /// Draws the specified text on one row without doing any checks or measurements for better performance. 
        /// Only use this on sanitized text (no newlines, hyphens (because they will be drawn directly) 
        /// or any kind of control characters, as the text will be drawn directly) and when you are sure that the text 
        /// is not going to overflow the parent element or that overflowing doesn't matter.
        /// </summary>
        public void DrawTextRowFast(
            string text,
            Point2D topLeftPoint,
            SKPaint rawPaint)
        {
            SKPoint drawPoint = new SKPoint(topLeftPoint.X, topLeftPoint.Y);
            Canvas?.DrawText(text, drawPoint, rawPaint);
        }

        public void DrawPath(
            SKPath skiaPath,
            IBrush fillBrush,
            IBrush outlineBrush,
            OutlineParams outlineParams)
        {
            if (!fillBrush.IsSkippable)
            {
                SKPaint fillPaint = fillBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: fillPaint,
                    paintMode: PaintMode.Fill);

                Canvas?.DrawPath(skiaPath, fillPaint);
            }

            if (!outlineBrush.IsSkippable && outlineParams.OutlineWidth != 0)
            {
                SKPaint outlinePaint = outlineBrush.ToSkiaPaint();
                PaintManager.ModifyPaint(
                    paint: outlinePaint,
                    paintMode: PaintMode.Outline,
                    outlineParams: outlineParams);

                Canvas?.DrawPath(skiaPath, outlinePaint);
            }
        }
        #endregion

        // /// <summary>
        // /// Will remove the soft hyphens (U+00ad) if any, returning true if there were hyphens, false otherwise.
        // /// The text without the hyphens will be returned in clearText.
        // /// </summary>
        // /// <param name="textWithHyphens">The text that has hyphens.</param>
        // /// <param name="clearText">
        // /// The text cleared from any hyphens. Is an empty string when there were no hyphens (to avoid useless allocations).
        // /// </param>
        // /// <returns>True if there were hyphens, false otherwise.</returns>
        // private static bool RemoveSoftHyphens(string textWithHyphens, out string clearText)
        // {
        //     bool hasHyphens = false;
        //     StringBuilder sb = new StringBuilder();
        //     for (int i = 0; i < textWithHyphens.Length; i++)
        //     {
        //         if (textWithHyphens[i] != '\u00ad')
        //         {
        //             sb.Append(textWithHyphens[i]);
        //         }
        //         else
        //         {
        //             hasHyphens = true;
        //         }
        //     }

        //     if (hasHyphens)
        //     {
        //         clearText = sb.ToString();
        //         return true;
        //     }
        //     else
        //     {
        //         clearText = "";
        //         return false;
        //     }
        // }

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
