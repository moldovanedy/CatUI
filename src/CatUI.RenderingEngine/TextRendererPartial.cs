using System;
using System.Collections.Generic;
using System.Text;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.RenderingEngine;

public partial class Renderer
{
    /// <summary>
    /// Draws the specified text until the text will exceed the width of the element size.
    /// This means that this method MIGHT NOT render the whole text but will return the number of characters rendered.
    /// </summary>
    /// <param name="text">The text to render. This method MIGHT NOT render the whole text.</param>
    /// <param name="topLeftPoint">The top-left point of the text that needs to be drawn.</param>
    /// <param name="fontAsset">The font to draw with.</param>
    /// <param name="fontSize">The font size of the text.</param>
    /// <param name="elementSize">
    /// The size of the element that contains the text.
    /// Use a very large width to guarantee the rendering of the whole text (this will use word wrap).
    /// This method does NOT account for vertical size, so vertical overflow is possible.
    /// </param>
    /// <param name="fillBrush"></param>
    /// <param name="outlineBrush"></param>
    /// <param name="textAlignment">
    /// The text alignment to use. <see cref="TextAlignmentType.Justify"/> won't have any effect
    /// and will work as <see cref="TextAlignmentType.Left"/>.
    /// </param>
    /// <param name="breakMode">
    /// Specifies the text break mode. See <see cref="TextBreakMode"/> for more information.
    /// </param>
    /// <param name="hyphenCharacter">
    /// Specifies the character used as a hyphen if necessary. For no hyphens, set this to the null character.
    /// </param>
    /// <param name="cachedMaxCharacters">
    /// If larger than 0, will use this value instead of using more expensive calculations with Skia's BreakText
    /// functions. If you already called BreakText and didn't modify the paint, you can safely pass the result here
    /// to avoid another call to BreakText.
    /// </param>
    /// <returns>The number of characters drawn.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the text contains an invalid newline (\r instead of \n or \r\n).
    /// </exception>
    public int DrawTextRow(
        string text,
        Point2D topLeftPoint,
        FontAsset fontAsset,
        float fontSize,
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
        if (
            fillBrush != null && !fillBrush.IsSkippable &&
            (outlineBrush == null || outlineBrush.IsSkippable))
        {
            painter = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(painter);
        }
        //outline, but no fill
        else if (
            outlineBrush != null && !outlineBrush.IsSkippable &&
            (fillBrush == null || fillBrush.IsSkippable))
        {
            painter = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(painter, PaintMode.Outline);
        }
        //both fill and outline
        else if (
            outlineBrush != null && !outlineBrush.IsSkippable &&
            fillBrush != null && !fillBrush.IsSkippable)
        {
            painter = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(painter, PaintMode.FillAndOutline);
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

        var drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);

        var sb = new StringBuilder();
        List<int> shyPositions = [];

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

            if (text[i] == '\u00ad')
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
            charsOnThisRow = fontAsset.GetSkiaFont(fontSize).BreakText(drawableText, elementSize.Width);
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
                            //skip the space to not show it on the next row
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
            //if the next character is a space, skip it to not show it on the next row
            else if (drawableText[charsOnThisRow] == ' ')
            {
                charsOnThisRow++;
            }
        }

        //actual drawing
        if (needsHyphen)
        {
            string newString = new(drawableText.AsSpan(charactersDrawn, charsOnThisRow));
            Canvas?.DrawText(
                newString + hyphenCharacter,
                drawPoint,
                GetSkiaTextAlignment(textAlignment),
                fontAsset.GetSkiaFont(fontSize),
                painter);
        }
        else
        {
            Canvas?.DrawText(
                drawableText.Substring(charactersDrawn, charsOnThisRow),
                drawPoint,
                GetSkiaTextAlignment(textAlignment),
                fontAsset.GetSkiaFont(fontSize),
                painter);
        }

        charactersDrawn += charsOnThisRow;

        //also add all the shy characters in the count
        foreach (int t in shyPositions)
        {
            if (t < charsOnThisRow)
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
    /// Draws the specified text on one row without doing any checks or measurements for better performance. 
    /// Only use this on sanitized text (no newlines, hyphens (because they will be drawn directly) 
    /// or any kind of control characters, as the text will be drawn directly) and when you are sure that the text 
    /// is not going to overflow the parent element or that overflowing doesn't matter.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="topLeftPoint"></param>
    /// <param name="fontSize"></param>
    /// <param name="elementSize"></param>
    /// <param name="font">The font that is used for drawing, otherwise it will use the platform default.</param>
    /// <param name="fillBrush"></param>
    /// <param name="outlineBrush"></param>
    /// <param name="textAlignment"></param>
    public void DrawTextRowFast(
        string text,
        Point2D topLeftPoint,
        float fontSize,
        Size elementSize,
        FontAsset? font = null,
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
        if (fillBrush != null && !fillBrush.IsSkippable && (outlineBrush == null || outlineBrush.IsSkippable))
        {
            painter = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(painter);
        }
        //outline, but no fill
        else if (outlineBrush != null && !outlineBrush.IsSkippable && (fillBrush == null || fillBrush.IsSkippable))
        {
            painter = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(painter, PaintMode.Outline);
        }
        //both fill and outline
        else if (outlineBrush != null && !outlineBrush.IsSkippable && fillBrush != null && !fillBrush.IsSkippable)
        {
            painter = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(painter, PaintMode.FillAndOutline);
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

        var drawPoint = new SKPoint(drawPointX, topLeftPoint.Y);
        font ??= FontAsset.Default;
        Canvas?.DrawText(text, drawPoint, GetSkiaTextAlignment(textAlignment), font.GetSkiaFont(fontSize), painter);
    }

    /// <summary>
    /// Draws the specified text on one row without doing any checks or measurements for better performance. 
    /// Only use this on sanitized text (no newlines, hyphens, or any kind of control characters,
    /// as the text will be drawn directly) and when you are sure that the text is not going to overflow
    /// the parent element or that overflowing doesn't matter.
    /// </summary>
    public void DrawTextRowFast(
        string text,
        Point2D topLeftPoint,
        SKPaint rawPaint,
        FontAsset font,
        TextAlignmentType textAlignment,
        float fontSize)
    {
        var drawPoint = new SKPoint(topLeftPoint.X, topLeftPoint.Y);
        Canvas?.DrawText(
            text, drawPoint, GetSkiaTextAlignment(textAlignment), font.GetSkiaFont(fontSize), rawPaint);
    }


    /// <summary>
    /// Will heuristically estimate the average size of a character using the given the paint. The estimate is smaller 
    /// than an actual character width (generally half) to ensure that an eventual text span to not be smaller 
    /// than the actual size of the element (although this estimation reduces that risk significantly, 
    /// it does NOT GUARANTEE that this won't happen).
    /// </summary>
    /// <param name="largeText">The large text string.</param>
    /// <param name="font">The font that will be used to draw the text.</param>
    /// <param name="fontSize">The size of the font.</param>
    /// <returns>
    /// The estimated average size of a character (estimation will generally be half of the actual average).
    /// </returns>
    public static float EstimateCharSizeSafe(string largeText, FontAsset font, float fontSize)
    {
        return EstimateCharSizeSafe(largeText.AsSpan(), font, fontSize);
    }

    /// <summary>
    /// Will heuristically estimate the average size of a character using the given the paint. The estimate is smaller 
    /// than an actual character width (generally half) to ensure that an eventual text span to not be smaller 
    /// than the actual size of the element (although this estimation reduces that risk significantly, 
    /// it does NOT GUARANTEE that this won't happen).
    /// </summary>
    /// <param name="largeText">The large text as a character span.</param>
    /// <param name="font">The font that will be used to draw the text.</param>
    /// <param name="fontSize">The size of the font.</param>
    /// <returns>
    /// The estimated average size of a character (estimation will generally be half of the actual average).
    /// </returns>
    public static float EstimateCharSizeSafe(ReadOnlySpan<char> largeText, FontAsset font, float fontSize)
    {
        if (largeText.Length <= 5)
        {
            return font.GetSkiaFont(fontSize).MeasureText(largeText) * 0.4f;
        }

        int upperLimit = largeText.Length;
        int minMeasuredCharacters = Math.Clamp(upperLimit / 10, 3, 20);
        int maxMeasuredCharacters = Math.Clamp(upperLimit / 10, 5, 35);
        var rand = new Random();

        int values;
        float sum = 0;
        int limit = rand.Next(minMeasuredCharacters, maxMeasuredCharacters);
        for (values = 0; values < limit; values++)
        {
            int index = rand.Next(0, upperLimit - 1);
            sum += font.GetSkiaFont(fontSize).MeasureText(largeText.Slice(index, 1));
        }

        int normalizedCharNumber = Math.Clamp(upperLimit, 100, 400);
        float multiplicationFactor = NumberUtils.Remap(normalizedCharNumber, 100, 400, 0.35f, 0.6f);
        //invert the scale (so small char number will have high multiplication factor)
        multiplicationFactor = 0.6f - multiplicationFactor + 0.35f;

        return sum / values * multiplicationFactor;
    }


    private static SKTextAlign GetSkiaTextAlignment(TextAlignmentType textAlignment)
    {
        switch (textAlignment)
        {
            default:
            case TextAlignmentType.Left:
                return SKTextAlign.Left;
            case TextAlignmentType.Center:
                return SKTextAlign.Center;
            case TextAlignmentType.Right:
                return SKTextAlign.Right;
        }
    }
}
