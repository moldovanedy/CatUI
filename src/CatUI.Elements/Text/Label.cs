using System;
using System.Collections.Generic;
using System.Text;
using CatUI.Data;
using CatUI.Data.Containers;
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
    public class Label : TextElement
    {
        public TextBreakMode TextBreakMode
        {
            get => _textBreakMode;
            set
            {
                _textBreakMode = value;
                TextBreakModeProperty.Value = value;
            }
        }

        private TextBreakMode _textBreakMode = TextBreakMode.SoftBreak;

        public ObservableProperty<TextBreakMode> TextBreakModeProperty { get; }
            = new();

        public char HyphenCharacter
        {
            get => _hyphenCharacter;
            set
            {
                _hyphenCharacter = value;
                HyphenCharacterProperty.Value = value;
            }
        }

        private char _hyphenCharacter = '-';
        public ObservableProperty<char> HyphenCharacterProperty { get; } = new();

        private List<KeyValuePair<string, float>>? _cachedRows;
        private float _maxRowWidth;
        private float _hyphenCharacterWidth;

        public Label(
            //required
            string text,
            //own
            TextBreakMode breakMode = TextBreakMode.SoftBreak,
            char hyphenCharacter = '-',
            //TextElement
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            string ellipsisString = "\u2026",
            bool wordWrap = false,
            bool allowsExpansion = true,
            //Element
            string name = "",
            List<Element>? children = null,
            ThemeDefinition<LabelThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,
            ContainerSizing? elementContainerSizing = null,
            bool visible = true,
            bool enabled = true,
            //Element actions
            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :

            //ReSharper disable ArgumentsStyleNamedExpression
            base(
                text: text,
                textAlignment: textAlignment,
                textOverflowMode: textOverflowMode,
                ellipsisString: ellipsisString,
                wordWrap: wordWrap,
                allowsExpansion: allowsExpansion,
                //
                name: name,
                children: children,
                position: position,
                preferredWidth: preferredWidth,
                preferredHeight: preferredHeight,
                minHeight: minHeight,
                minWidth: minWidth,
                maxHeight: maxHeight,
                maxWidth: maxWidth,
                elementContainerSizing: elementContainerSizing,
                visible: visible,
                enabled: enabled,
                //
                onDraw: onDraw,
                onEnterDocument: onEnterDocument,
                onExitDocument: onExitDocument,
                onLoad: onLoad,
                onPointerEnter: onPointerEnter,
                onPointerLeave: onPointerLeave,
                onPointerMove: onPointerMove)
        //ReSharper enable ArgumentsStyleNamedExpression
        {
            DrawEvent += DrawText;
            TextProperty.ValueChangedEvent += OnTextChanged;

            TextBreakMode = breakMode;
            HyphenCharacter = hyphenCharacter;

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }
        }

        ~Label()
        {
            DrawEvent -= DrawText;
            TextProperty.ValueChangedEvent -= OnTextChanged;
        }

        internal override void RecalculateLayout()
        {
            if (IsChildOfContainer)
            {
                return;
            }

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
            if (Math.Abs(MathF.Round(AbsoluteHeight, 1) - MathF.Round(normalHeight, 1)) > 0.001 ||
                Math.Abs(MathF.Round(AbsoluteWidth, 1) - MathF.Round(normalWidth, 1)) > 0.001)
            {
                _cachedRows = null;
            }

            if (string.IsNullOrEmpty(Text))
            {
                AbsoluteWidth = normalWidth;
                AbsoluteHeight = normalHeight;
                AbsolutePosition = normalPosition;
                goto ChildRecalculation;
            }

            if (_cachedRows != null || !WordWrap)
            {
                AbsoluteWidth = normalWidth;
                AbsoluteHeight = normalHeight;
                AbsolutePosition = normalPosition;
                goto ChildRecalculation;
            }

            //calculate the actual dimensions occupied by the text
            _cachedRows = new List<KeyValuePair<string, float>>();

            var sb = new StringBuilder();
            List<int> shyPositions = new();
            List<int> newlinePositions = new();

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

            string? drawableText = sb.ToString();
            float maxWidth = normalWidth;
            float newHeight = 0;
            LabelThemeData currentTheme =
                GetElementFinalThemeData<LabelThemeData>(LabelThemeData.STYLE_NORMAL) ??
                new LabelThemeData().GetDefaultData<LabelThemeData>(LabelThemeData.STYLE_NORMAL);

            float fontSize = CalculateDimension(currentTheme.FontSize!);
            float lineHeightPixels = fontSize * (currentTheme.LineHeight ?? 1.2f);

            SKPaint paint = PaintManager.GetPaint(fontSize: fontSize);

            int characterPosition = 0;
            bool isFirstRow = true;
            ReadOnlySpan<char> textSpan = drawableText.AsSpan();
            _hyphenCharacterWidth = paint.MeasureText(HyphenCharacter.ToString());

            int lastNewLinePosition = 0;
            while (characterPosition < textSpan.Length)
            {
                ReadOnlySpan<char> nextSlice = textSpan[characterPosition..];
                float avgCharSize = Renderer.EstimateCharSizeSafe(nextSlice, paint);
                int safeCharacterNumber = Math.Min((int)(normalWidth / avgCharSize), nextSlice.Length);

                for (int i = lastNewLinePosition; i < newlinePositions.Count; i++)
                {
                    if (newlinePositions[i] > characterPosition &&
                        newlinePositions[i] < characterPosition + safeCharacterNumber)
                    {
                        safeCharacterNumber = newlinePositions[i] - characterPosition;
                        lastNewLinePosition = i;
                        break;
                    }
                }

                nextSlice = nextSlice[..safeCharacterNumber];
                if (nextSlice.Length == 0)
                {
                    CatLogger.LogWarning(
                        "Label width to small for the specified font size. Skipping to the next character.");
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
                            if (TextBreakMode == TextBreakMode.SoftBreak &&
                                shyPositions.Contains(characterPosition + characterCount))
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
                if (!AllowsExpansion && newHeight + rowHeight > CalculateDimension(MaxHeight))
                {
                    //replace the last characters of the last row with an ellipsis
                    string lastRow = _cachedRows[^1].Key;
                    float ellipsisWidth = paint.MeasureText(EllipsisString);

                    float widthToTrim = 0;
                    while (widthToTrim < ellipsisWidth)
                    {
                        widthToTrim += paint.MeasureText(lastRow.AsSpan(lastRow.Length - 1, 1));
                    }

                    lastRow = lastRow.Substring(0, lastRow.Length - 1);
                    _cachedRows[^1] = new KeyValuePair<string, float>(
                        lastRow,
                        _cachedRows[^1].Value);
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
                            needsHyphen
                                ? new string(nextSlice[..characterCount]) + HyphenCharacter
                                : new string(nextSlice[..characterCount]),
                            newWidth));
                }
                else
                {
                    _cachedRows.Add(
                        new KeyValuePair<string, float>(
                            needsHyphen
                                ? new string(nextSlice[..characterCount]) + HyphenCharacter
                                : new string(nextSlice[..characterCount]),
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
            AbsoluteHeight = Math.Max(newHeight, normalHeight);

            AbsoluteWidth = AllowsExpansion ? maxWidth : normalWidth;
            AbsolutePosition = normalPosition;

            //determine the largest row width
            foreach (KeyValuePair<string, float> t in _cachedRows)
            {
                if (t.Value > _maxRowWidth)
                {
                    _maxRowWidth = t.Value;
                }
            }

        ChildRecalculation:
            foreach (Element child in GetChildren(true))
            {
                child.RecalculateLayout();
            }
        }

        private void DrawText()
        {
            LabelThemeData currentTheme =
                GetElementFinalThemeData<LabelThemeData>(LabelThemeData.STYLE_NORMAL) ??
                new LabelThemeData().GetDefaultData<LabelThemeData>(LabelThemeData.STYLE_NORMAL);
            float fontSize = CalculateDimension(currentTheme.FontSize!);
            float rowSize = fontSize * (currentTheme.LineHeight ?? 1.2f);
            Point2D rowPosition = Bounds.StartPoint;
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
                    (AllowsExpansion || rowPosition.Y < Bounds.StartPoint.Y + Bounds.Width))
                {
                    SKPaint painter = PaintManager.GetPaint(fontSize: fontSize);
                    painter.Color = currentTheme.FillBrush!.ToSkiaPaint().Color;

                    Document?.Renderer?.DrawTextRowFast(_cachedRows[rowsDrawn].Key, rowPosition, painter);
                    charactersDrawn += _cachedRows[rowsDrawn].Key.Length;
                    rowPosition = new Point2D(rowPosition.X, rowPosition.Y + rowSize);
                    rowsDrawn++;
                }
            }
            else
            {
                Document?.Renderer.DrawTextRow(
                    Text,
                    Bounds.StartPoint,
                    currentTheme.FontSize!,
                    new Size(AbsoluteWidth, AbsoluteHeight),
                    currentTheme.FillBrush!,
                    currentTheme.OutlineBrush!,
                    TextAlignment,
                    TextOverflowMode,
                    EllipsisString);
            }
        }

        private void OnTextChanged(string? newText)
        {
            _cachedRows = null;
        }
    }
}
