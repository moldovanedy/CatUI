using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;
using CatUI.Shared;
using SkiaSharp;

namespace CatUI.Elements.Text
{
    public class Label : TextElement
    {
        public TextBreakMode TextBreakMode { get; set; } = TextBreakMode.SoftBreak;
        public char HyphenCharacter { get; set; } = '-';

        private List<KeyValuePair<string, float>>? _cachedRows;

        public Label() : base(string.Empty)
        {
            DrawEvent += DrawText;
        }

        public Label(
            string text,

            TextBreakMode breakMode = TextBreakMode.SoftBreak,
            char hyphenCharacter = '-',

            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            bool wordWrap = false,
            bool allowsExpansion = true,

            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<LabelThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(text: text,
                 textAlignment: textAlignment,
                 textOverflowMode: textOverflowMode,
                 wordWrap: wordWrap,
                 allowsExpansion: allowsExpansion,

                 doc: doc,
                 children: children,
                 position: position,
                 width: width,
                 height: height,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth)
        {
            DrawEvent += DrawText;
            TextBreakMode = breakMode;
            HyphenCharacter = hyphenCharacter;

            if (themeOverrides != null)
            {
                base.SetElementThemeOverrides(themeOverrides);
            }
        }

        ~Label()
        {
            DrawEvent -= DrawText;
        }

        #region Builder
        public Label SetInitialTextBreakMode(TextBreakMode textBreakMode)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            TextBreakMode = textBreakMode;
            return this;
        }

        public Label SetInitialHyphenCharacter(char hyphenCharacter)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            HyphenCharacter = hyphenCharacter;
            return this;
        }
        #endregion //Builder

        protected override void RecalculateLayout()
        {
            float parentWidth, parentHeight, parentXPos, parentYPos;
            if (Document?.Root == this)
            {
                parentWidth = Document.ViewportSize.Width;
                parentHeight = Document.ViewportSize.Height;
                parentXPos = 0;
                parentYPos = 0;
            }
            else
            {
                Element? parent = GetParent();
                parentWidth = parent?.Bounds.Width ?? 0;
                parentHeight = parent?.Bounds.Height ?? 0;
                parentXPos = parent?.Bounds.StartPoint.X ?? 0;
                parentYPos = parent?.Bounds.StartPoint.Y ?? 0;
            }

            float normalWidth = 0, normalHeight = 0;
            Point2D normalPosition = Point2D.Zero;

            if (!Width.IsUnset())
            {
                normalWidth = Math.Clamp(
                    CalculateDimension(Width, parentWidth),
                    MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                    MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            }

            if (!Height.IsUnset())
            {
                normalHeight = Math.Clamp(
                    CalculateDimension(Height, parentHeight),
                    MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));
            }

            if (!Position.IsUnset())
            {
                normalPosition = new Point2D(
                     parentXPos + CalculateDimension(Position.X, parentWidth),
                     parentYPos + CalculateDimension(Position.Y, parentHeight));
            }

            //calculate the actual dimensions occupied by the text
            if (base.AllowsExpansion && !string.IsNullOrEmpty(Text))
            {
                if (_cachedRows == null)
                {
                    _cachedRows = new List<KeyValuePair<string, float>>();
                }

                StringBuilder sb = new StringBuilder();
                List<int> shyPositions = new List<int>();
                List<int> newlinePositions = new List<int>();

                for (int i = 0; i < Text.Length; i++)
                {
                    if (Text[i] == '\r' && i == Text.Length - 1)
                    {
                        throw new ArgumentException("Invalid text: found CR (\\r) without LF (\\n)", Text);
                    }

                    if (Text[i] == '\n' || (Text[i] == '\r' && Text[i + 1] == '\n'))
                    {
                        newlinePositions.Add(sb.Length);
                        continue;
                    }
                    else if (Text[i] == '\u00ad')
                    {
                        if (TextBreakMode == TextBreakMode.SoftBreak)
                        {
                            shyPositions.Add(sb.Length);
                        }
                        continue;
                    }

                    sb.Append(Text[i]);
                }

                string drawableText = sb.ToString();
                float maxWidth = normalWidth;
                float newHeight = normalHeight;
                LabelThemeData currentTheme = base.GetElementFinalThemeData<LabelThemeData>(Label.STYLE_NORMAL);

                float fontSize = CalculateDimension(currentTheme.FontSize);
                float lineHeightPixels = fontSize * currentTheme.LineHeight;

                SKPaint paint = PaintManager.GetPaint(
                    paintMode: PaintMode.Fill,
                    fontSize: fontSize);

                int characterPosition = 0;
                bool isFirstRow = true;
                ReadOnlySpan<char> textSpan = drawableText.AsSpan();

                while (characterPosition < textSpan.Length)
                {
                    ReadOnlySpan<char> nextSlice = textSpan.Slice(characterPosition);
                    float avgCharSize = Renderer.EstimateCharSizeSafe(nextSlice, paint);
                    int safeCharacterNumber = Math.Min((int)(normalWidth / avgCharSize), nextSlice.Length);

                    for (int i = 0; i < newlinePositions.Count; i++)
                    {
                        if (newlinePositions[i] > characterPosition && newlinePositions[i] < characterPosition + safeCharacterNumber)
                        {
                            safeCharacterNumber = newlinePositions[i] - characterPosition;
                            break;
                        }
                    }
                    nextSlice = nextSlice.Slice(0, safeCharacterNumber);

                    int characterCount = (int)paint.BreakText(nextSlice, normalWidth);
                    int normalCharacterCount = characterCount;
                    if (characterCount == 0)
                    {
                        characterCount = 1;
                        normalCharacterCount = 1;
                    }
                    else if (characterCount < nextSlice.Length)
                    {
                        //if (characterCount < nextSlice.Length - 1 && char.IsWhiteSpace(nextSlice[characterCount + 1]))
                        if (characterCount > 0 && char.IsWhiteSpace(nextSlice[characterCount - 1]))
                        {
                            characterCount--;
                        }
                        else
                        {
                            int safeValue = characterCount;
                            while (characterCount > 0)
                            {
                                if (TextBreakMode == TextBreakMode.SoftBreak && shyPositions.Contains(characterPosition + characterCount))
                                {
                                    break;
                                }
                                if (char.IsWhiteSpace(nextSlice[characterCount]))
                                {
                                    break;
                                }

                                characterCount--;
                            }

                            //if there was no space or hyphen, then just use the previous size to avoid infinite loops
                            if (characterCount == 0)
                            {
                                characterCount = safeValue;
                            }
                        }
                    }

                    characterPosition += characterCount;

                    if (normalCharacterCount < characterCount)
                    {
                        float newWidth = normalWidth + paint.MeasureText(
                            nextSlice.Slice(normalCharacterCount, characterCount - normalCharacterCount));
                        if (newWidth > maxWidth)
                        {
                            maxWidth = newWidth;
                        }
                        _cachedRows.Add(
                            new KeyValuePair<string, float>(
                                new string(nextSlice.Slice(0, characterCount)),
                                newWidth));
                    }
                    else
                    {
                        _cachedRows.Add(
                            new KeyValuePair<string, float>(
                                new string(nextSlice.Slice(0, characterCount)),
                                normalWidth));
                    }

                    if (isFirstRow)
                    {
                        newHeight += (lineHeightPixels / 2f) + (fontSize / 2f);
                        isFirstRow = false;
                    }
                    else
                    {
                        newHeight += lineHeightPixels;
                    }

                    //solves the case where the row is truncated right before a space, 
                    //thus causing a blank space like an indent on the next row
                    if (characterPosition < textSpan.Length && textSpan[characterPosition] == ' ')
                    {
                        characterPosition++;
                    }
                }

                newHeight += (lineHeightPixels / 2f) - (fontSize / 2f);

                base.InternalWidth = maxWidth;
                base.InternalHeight = newHeight;
                base.InternalPosition = normalPosition;
            }
            else
            {
                base.InternalWidth = normalWidth;
                base.InternalHeight = normalHeight;
                base.InternalPosition = normalPosition;
            }

            if (_cachedRows != null)
            {
                for (int i = 0; i < _cachedRows.Count; i++)
                {
                    Debug.WriteLine(_cachedRows[i].Key);
                }

                Debug.WriteLine("----------------------------");
                _cachedRows.Clear();
            }
        }

        private void DrawText()
        {
            LabelThemeData currentTheme = base.GetElementFinalThemeData<LabelThemeData>(Label.STYLE_NORMAL);
            float fontSize = CalculateDimension(currentTheme.FontSize);
            float rowSize = fontSize * currentTheme.LineHeight;
            Point2D rowPosition = base.Bounds.StartPoint;
            //half of line width + 0.5 (so for line height of 2 it is 1 + 0.5, for 4 is 2 + 0.5 etc.)
            rowPosition.Y += (rowSize / 2f) + (fontSize / 2f);

            if (WordWrap)
            {
                int charactersDrawn = 0;
                while (
                    charactersDrawn < Text.Length &&
                    //TODO: also take into account the line height and next row's vertical size on the left-hand expression
                    (AllowsExpansion ? true : rowPosition.Y < base.Bounds.StartPoint.Y + base.Bounds.Width))
                {
                    while (charactersDrawn < Text.Length && (Text[charactersDrawn] == '\n' || Text[charactersDrawn] == '\r'))
                    {
                        charactersDrawn++;
                    }

                    int thisRowChars =
                        base.Document?.Renderer?.DrawTextRow(
                            //TODO: find ways to optimize this, preferably inside the rendering engine
                            text: Text.Substring(charactersDrawn),
                            topLeftPoint: rowPosition,
                            fontSize: currentTheme.FontSize,
                            elementSize: new Size(base.InternalWidth, base.InternalHeight),
                            fillBrush: currentTheme.FillBrush,
                            outlineBrush: currentTheme.OutlineBrush,
                            textAlignment: base.TextAlignment,
                            breakMode: TextBreakMode,
                            hyphenCharacter: HyphenCharacter) ??
                        1;
                    charactersDrawn += thisRowChars;
                    rowPosition = new Point2D(rowPosition.X, rowPosition.Y + rowSize);
                }
            }
            else
            {
                base.Document?.Renderer?.DrawTextRow(
                    text: Text,
                    topLeftPoint: this.Bounds.StartPoint,
                    fontSize: currentTheme.FontSize,
                    elementSize: new Size(base.InternalWidth, base.InternalHeight),
                    fillBrush: currentTheme.FillBrush,
                    outlineBrush: currentTheme.OutlineBrush,
                    textAlignment: base.TextAlignment,
                    overflowMode: base.TextOverflowMode);
            }
        }
    }
}
