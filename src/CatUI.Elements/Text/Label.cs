using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Managers;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;
using CatUI.RenderingEngine;
using SkiaSharp;

namespace CatUI.Elements.Text
{
    public partial class Label : TextElement
    {
        public TextBreakMode TextBreakMode
        {
            get
            {
                return _textBreakMode;
            }
            set
            {
                _textBreakMode = value;
                TextBreakModeProperty.Value = value;
            }
        }
        private TextBreakMode _textBreakMode = TextBreakMode.SoftBreak;
        public ObservableProperty<TextBreakMode> TextBreakModeProperty { get; }
            = new ObservableProperty<TextBreakMode>();

        public char HyphenCharacter
        {
            get
            {
                return _hyphenCharacter;
            }
            set
            {
                _hyphenCharacter = value;
                HyphenCharacterProperty.Value = value;
            }
        }
        private char _hyphenCharacter = '-';
        public ObservableProperty<char> HyphenCharacterProperty { get; } = new ObservableProperty<char>();

        private List<KeyValuePair<string, float>>? _cachedRows;
        private float _maxRowWidth;
        private float _hyphenCharacterWidth;

        public Label(
            string text,

            TextBreakMode breakMode = TextBreakMode.SoftBreak,
            char hyphenCharacter = '-',

            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            bool wordWrap = false,
            bool allowsExpansion = true,

            List<Element>? children = null,
            ThemeDefinition<LabelThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,

            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :
            base(text: text,
                 textAlignment: textAlignment,
                 textOverflowMode: textOverflowMode,
                 wordWrap: wordWrap,
                 allowsExpansion: allowsExpansion,

                 children: children,
                 position: position,
                 preferredWidth: preferredWidth,
                 preferredHeight: preferredHeight,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth,

                 onDraw: onDraw,
                 onEnterDocument: onEnterDocument,
                 onExitDocument: onExitDocument,
                 onLoad: onLoad,
                 onPointerEnter: onPointerEnter,
                 onPointerLeave: onPointerLeave,
                 onPointerMove: onPointerMove)
        {
            DrawEvent += DrawText;
            base.TextProperty.ValueChangedEvent += OnTextChanged;

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
            base.TextProperty.ValueChangedEvent -= OnTextChanged;
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

            if (!PreferredWidth.IsUnset())
            {
                normalWidth = Math.Clamp(
                    CalculateDimension(PreferredWidth, parentWidth),
                    MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                    MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            }

            if (!PreferredHeight.IsUnset())
            {
                normalHeight = Math.Clamp(
                    CalculateDimension(PreferredHeight, parentHeight),
                    MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));
            }

            if (!Position.IsUnset())
            {
                normalPosition = new Point2D(
                     parentXPos + CalculateDimension(Position.X, parentWidth),
                     parentYPos + CalculateDimension(Position.Y, parentHeight));
            }

            //TODO: optimize this so that a recalculation doesn't happen on resizing, but only when it's necessary
            if (MathF.Round(base.InternalHeight, 1) != MathF.Round(normalHeight, 1) ||
                MathF.Round(base.InternalWidth, 1) != MathF.Round(normalWidth, 1))
            {
                _cachedRows = null;
            }

            if (string.IsNullOrEmpty(Text))
            {
                base.InternalWidth = normalWidth;
                base.InternalHeight = normalHeight;
                base.InternalPosition = normalPosition;
                return;
            }

            if (_cachedRows != null || !base.WordWrap)
            {
                base.InternalWidth = normalWidth;
                base.InternalHeight = normalHeight;
                base.InternalPosition = normalPosition;
                return;
            }

            //calculate the actual dimensions occupied by the text
            _cachedRows = new List<KeyValuePair<string, float>>();

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
            float newHeight = 0;
            LabelThemeData currentTheme = base.GetElementFinalThemeData<LabelThemeData>(Label.STYLE_NORMAL);

            float fontSize = CalculateDimension(currentTheme.FontSize);
            float lineHeightPixels = fontSize * currentTheme.LineHeight;

            SKPaint paint = PaintManager.GetPaint(
                paintMode: PaintMode.Fill,
                fontSize: fontSize);

            int characterPosition = 0;
            bool isFirstRow = true;
            ReadOnlySpan<char> textSpan = drawableText.AsSpan();
            _hyphenCharacterWidth = paint.MeasureText(HyphenCharacter.ToString());

            int lastNewLinePosition = 0;
            while (characterPosition < textSpan.Length)
            {
                ReadOnlySpan<char> nextSlice = textSpan.Slice(characterPosition);
                float avgCharSize = Renderer.EstimateCharSizeSafe(nextSlice, paint);
                int safeCharacterNumber = Math.Min((int)(normalWidth / avgCharSize), nextSlice.Length);

                for (int i = lastNewLinePosition; i < newlinePositions.Count; i++)
                {
                    if (newlinePositions[i] > characterPosition && newlinePositions[i] < characterPosition + safeCharacterNumber)
                    {
                        safeCharacterNumber = newlinePositions[i] - characterPosition;
                        lastNewLinePosition = i;
                        break;
                    }
                }
                nextSlice = nextSlice.Slice(0, safeCharacterNumber);
                if (nextSlice.Length == 0)
                {
                    Debug.WriteLine("WARN: Label width to small for the specified font size. Skipping to the next character.");
                    characterPosition++;
                    continue;
                }

                bool needsHyphen = false;
                //take into account an eventual hyphen
                int characterCount = (int)paint.BreakText(nextSlice, normalWidth - _hyphenCharacterWidth);
                int normalCharacterCount = characterCount;

                if (characterCount == 0)
                {
                    characterCount = 1;
                    normalCharacterCount = 1;
                }
                else if (characterCount < nextSlice.Length)
                {
                    //if the last character is a space
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
                                needsHyphen = true;
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

                float rowHeight = (lineHeightPixels / 2f) + (fontSize / 2f);
                //if the element doesn't allow expansion we just break the loop
                if (!base.AllowsExpansion && newHeight + rowHeight > CalculateDimension(base.MaxHeight))
                {
                    //replace the last characters of the last row with an ellipsis
                    string lastRow = _cachedRows[_cachedRows.Count - 1].Key;
                    float ellipsisWidth = paint.MeasureText(base.EllipsisString);

                    int charactersToTrim = 0;
                    float widthToTrim = 0;
                    while (widthToTrim < ellipsisWidth)
                    {
                        widthToTrim += paint.MeasureText(lastRow.AsSpan(lastRow.Length - 1 - charactersToTrim, 1));
                    }

                    lastRow = lastRow.Substring(0, lastRow.Length - 1 - charactersToTrim);
                    _cachedRows[_cachedRows.Count - 1] = new KeyValuePair<string, float>(
                        lastRow,
                        _cachedRows[_cachedRows.Count - 1].Value);
                    break;
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
                            needsHyphen ?
                            new string(nextSlice.Slice(0, characterCount)) + HyphenCharacter :
                            new string(nextSlice.Slice(0, characterCount)),
                            newWidth));
                }
                else
                {
                    _cachedRows.Add(
                        new KeyValuePair<string, float>(
                            needsHyphen ?
                            new string(nextSlice.Slice(0, characterCount)) + HyphenCharacter :
                            new string(nextSlice.Slice(0, characterCount)),
                            normalWidth));
                }

                if (isFirstRow)
                {
                    newHeight += rowHeight;
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
            base.InternalHeight = Math.Max(newHeight, normalHeight);

            base.InternalWidth = base.AllowsExpansion ? maxWidth : normalWidth;
            base.InternalPosition = normalPosition;

            //determine the largest's row width
            for (int i = 0; i < _cachedRows.Count; i++)
            {
                if (_cachedRows[i].Value > _maxRowWidth)
                {
                    _maxRowWidth = _cachedRows[i].Value;
                }
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
                if (_cachedRows == null)
                {
                    return;
                }

                int rowsDrawn = 0;
                int charactersDrawn = 0;
                while (
                    rowsDrawn < _cachedRows.Count &&
                    charactersDrawn < Text.Length &&
                    //TODO: also take into account the line height and next row's vertical size on the left-hand expression
                    (AllowsExpansion ? true : rowPosition.Y < base.Bounds.StartPoint.Y + base.Bounds.Width))
                {
                    SKPaint painter = PaintManager.GetPaint(
                        paintMode: PaintMode.Fill,
                        fontSize: fontSize);
                    painter.Color = currentTheme.FillBrush.ToSkiaPaint().Color;

                    base.Document?.Renderer?.DrawTextRowFast(_cachedRows[rowsDrawn].Key, rowPosition, painter);
                    charactersDrawn += _cachedRows[rowsDrawn].Key.Length;
                    rowPosition = new Point2D(rowPosition.X, rowPosition.Y + rowSize);
                    rowsDrawn++;
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
                    overflowMode: base.TextOverflowMode,
                    ellipsisStringOverride: base.EllipsisString);
            }
        }

        private void OnTextChanged(string? newText)
        {
            _cachedRows = null;
        }
    }
}
