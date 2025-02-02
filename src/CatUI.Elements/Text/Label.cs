using System;
using System.Collections.Generic;
using System.Text;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;
using CatUI.RenderingEngine.GraphicsCaching;
using SkiaSharp;

namespace CatUI.Elements.Text
{
    public class Label : TextElement
    {
        public TextBreakMode BreakMode
        {
            get => _breakMode;
            set
            {
                _breakMode = value;
                BreakModeProperty.Value = value;
            }
        }

        private TextBreakMode _breakMode = TextBreakMode.SoftBreak;

        public ObservableProperty<TextBreakMode> BreakModeProperty { get; }
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

        /// <summary>
        /// Represents the rows given by the user. These are not affected by word wrap or expansion, they are purely data.
        /// The <see cref="RowInformation.Width"/> and <see cref="RowInformation.WidthWithoutLastBreakPoint"/>
        /// fields are irrelevant here and always have the default value.
        /// </summary>
        private readonly List<RowInformation> _userRows = new();

        /// <summary>
        /// Represents the rows that will be drawn. Will contain the hyphens and ellipsis if it's the case.
        /// The <see cref="RowInformation.PossibleBreakPoints"/> and <see cref="RowInformation.WidthWithoutLastBreakPoint"/>
        /// fields are irrelevant here and always have the default value.
        /// </summary>
        private readonly List<RowInformation> _drawableRows = new();

        /// <summary>
        /// The width of the most wide row currently drawn, (<see cref="_drawableRows"/>, not <see cref="_userRows"/>).
        /// Only relevant when <see cref="TextElement.WordWrap"/> is true.
        /// </summary>
        private float _maxRowWidth;

        /// <summary>
        /// Represents the total height of the visible text (the number of rows * (font size + some line height calculation)).
        /// Does NOT include the line height spacing after the last visible row.
        /// </summary>
        private float _visibleTextTotalHeight;

        private LabelThemeData? _themeCache;

        public Label(
            string text,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            ThemeDefinition<LabelThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                text,
                textAlignment,
                preferredWidth: preferredWidth,
                preferredHeight: preferredHeight)
        {
            TextProperty.ValueChangedEvent += OnTextChanged;

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }

            TextProperty.ForceRecallEvents();
        }

        ~Label()
        {
            TextProperty.ValueChangedEvent -= OnTextChanged;
        }

        internal override void RecalculateLayout()
        {
            float normalWidth = 0, normalHeight = 0;
            Point2D normalPosition = Point2D.Zero;

            if (IsChildOfContainer)
            {
                goto TextRecalculation;
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

        TextRecalculation:
            if (!IsChildOfContainer)
            {
                AbsoluteWidth = normalWidth;
                AbsoluteHeight = normalHeight;
                AbsolutePosition = normalPosition;
            }

            //temp
            _visibleTextTotalHeight = float.PositiveInfinity;

            if (string.IsNullOrEmpty(Text) || (normalWidth >= _maxRowWidth && normalHeight >= _visibleTextTotalHeight))
            {
                goto ChildRecalculation;
            }

            CreateFinalText();

            if (AbsoluteHeight < _visibleTextTotalHeight)
            {
                //TODO: skip the last row, draw the ellipsis on the second to last row
            }

        ChildRecalculation:
            foreach (Element child in Children)
            {
                child.RecalculateLayout();
            }
        }

        public override void Draw()
        {
            base.DrawBackground();

            LabelThemeData currentTheme =
                GetElementFinalThemeData<LabelThemeData>(LabelThemeData.STYLE_NORMAL) ??
                new LabelThemeData().GetDefaultData<LabelThemeData>(LabelThemeData.STYLE_NORMAL);
            float fontSize = CalculateDimension(currentTheme.FontSize!);
            float rowSize = fontSize * (currentTheme.LineHeight ?? 1.2f);
            //half of line height + 0.5 (so for line height of 2 it is 1 + 0.5, for 4 is 2 + 0.5 etc.)
            Point2D rowPosition = new(Bounds.StartPoint.X, Bounds.StartPoint.Y + (rowSize / 2f) + (fontSize / 2f));

            int rowsDrawn = 0;
            int charactersDrawn = 0;
            while (
                rowsDrawn < _drawableRows.Count &&
                charactersDrawn < Text.Length &&
                //TODO: also take into account the line height and next row's vertical size on the left-hand expression
                (AllowsExpansion || rowPosition.Y <= Bounds.StartPoint.Y + Bounds.Height))
            {
                SKPaint painter = PaintManager.GetPaint(fontSize: fontSize);
                painter.Color = currentTheme.FillBrush!.ToSkiaPaint().Color;

                Document?.Renderer?.DrawTextRowFast(_drawableRows[rowsDrawn].Text, rowPosition, painter);
                charactersDrawn += _drawableRows[rowsDrawn].Text.Length;
                rowPosition = new Point2D(rowPosition.X, rowPosition.Y + rowSize);
                rowsDrawn++;
            }
        }

        private void OnTextChanged(string? newText)
        {
            //empty the cache
            _userRows.Clear();
            _visibleTextTotalHeight = 0;
            RowInformation rowInfo;

            List<int> possibleBreakPoints = new();
            _themeCache = GetElementFinalThemeData<LabelThemeData>(LabelThemeData.STYLE_NORMAL);

            if (newText == null)
            {
                //this will trigger another call
                Text = string.Empty;
                return;
            }

            //the first row
            int columnIndex = 0;

            var sb = new StringBuilder();
            for (int i = 0; i < newText.Length; i++)
            {
                //check for newline
                if (
                    newText[i] == '\n' ||
                    (newText[i] == '\r' && i + 1 < newText.Length && newText[i + 1] == '\n') ||
                    newText[i] == '\r')
                {
                    //if it was CRLF
                    if (newText[i + 1] == '\n')
                    {
                        i++;
                    }

                    rowInfo = new RowInformation { Text = sb.ToString(), PossibleBreakPoints = possibleBreakPoints };
                    _userRows.Add(rowInfo);
                    sb.Clear();
                    possibleBreakPoints = new List<int>();
                    columnIndex = 0;
                    continue;
                }

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (newText[i] == '\u00ad')
                {
                    possibleBreakPoints.Add(columnIndex - 1);
                    continue;
                }

                if (newText[i] == ' ')
                {
                    //includes the space
                    possibleBreakPoints.Add(columnIndex);
                }

                sb.Append(newText[i]);
                columnIndex++;
            }

            //for the last (or the single) row
            rowInfo = new RowInformation { Text = sb.ToString(), PossibleBreakPoints = possibleBreakPoints };
            _userRows.Add(rowInfo);
            RecalculateLayout();
        }

        private void CreateFinalText()
        {
            _drawableRows.Clear();
            _maxRowWidth = 0;
            _visibleTextTotalHeight = 0;
            bool stoppedEarly = false;

            float fontSize = CalculateDimension(_themeCache!.FontSize!);
            float rowHeight = fontSize * (_themeCache!.LineHeight ?? 1.2f);
            SKPaint painter = PaintManager.GetPaint(fontSize: fontSize);

            float overflowStringWidth = TextMeasuringCache.GetValueOrCalculate(OverflowString.AsMemory(), fontSize);
            float hyphenCharacterWidth = TextMeasuringCache.GetValueOrCalculate(
                new ReadOnlyMemory<char>(new[] { HyphenCharacter }), fontSize);
            float currentHeight = (rowHeight / 2f) + (fontSize / 2f);

            if (WordWrap)
            {
                int userRowIdx = 0;

                while (userRowIdx < _userRows.Count)
                {
                    RowInformation row = _userRows[userRowIdx];
                    stoppedEarly = MustStop(currentHeight, rowHeight);
                    if (stoppedEarly)
                    {
                        goto End;
                    }

                    //edge case: no break points
                    if (row.PossibleBreakPoints.Count == 0)
                    {
                        ReadOnlyMemory<char> thisRow;

                        if (BreakMode == TextBreakMode.HardBreak)
                        {
                            int length = (int)painter.BreakText(row.Text.AsSpan(), AbsoluteWidth);
                            thisRow = row.Text.AsMemory(0, length);
                        }
                        else
                        {
                            thisRow = row.Text.AsMemory();
                        }

                        float currentRowWidth = painter.MeasureText(thisRow.Span);
                        _drawableRows.Add(new RowInformation()
                        {
                            Text = row.Text,
                            PossibleBreakPoints = new List<int>(),
                            Width = currentRowWidth,
                            WidthWithoutLastBreakPoint = 0
                        });
                        currentHeight += rowHeight;

                        userRowIdx++;
                        if (currentRowWidth > _maxRowWidth)
                        {
                            _maxRowWidth = currentRowWidth;
                        }


                        continue;
                    }

                    if (BreakMode == TextBreakMode.HardBreak)
                    {
                        int startIndex = 0;

                        while (startIndex < row.Text.Length)
                        {
                            int charCount = (int)painter.BreakText(row.Text.AsSpan(startIndex), AbsoluteWidth);
                            //at least one char to avoid infinite loops (both in code and in UI as infinite newlines)
                            if (charCount == 0)
                            {
                                charCount = 1;
                            }

                            float currentRowWidth = painter.MeasureText(row.Text.AsSpan(startIndex, charCount));

                            _drawableRows.Add(new RowInformation
                            {
                                Text = row.Text.Substring(startIndex, charCount), Width = currentRowWidth
                            });
                            currentHeight += rowHeight;

                            startIndex += charCount;
                            if (currentRowWidth > _maxRowWidth)
                            {
                                _maxRowWidth = currentRowWidth;
                            }
                        }

                        userRowIdx++;
                        continue;
                    }

                    //for the current user row
                    for (int breakPointIndex = -1; breakPointIndex < row.PossibleBreakPoints.Count; breakPointIndex++)
                    {
                        stoppedEarly = MustStop(currentHeight, rowHeight);
                        if (stoppedEarly)
                        {
                            goto End;
                        }

                        float currentRowWidth = 0;
                        StringBuilder sb = new();
                        bool hasFitAtLeastOne = false;
                        bool needsHyphen = false;
                        int lastConvenientBreakPosition = -1;
                        int lastConvenientBreakPointIndex = -1;

                        while (true)
                        {
                            if (breakPointIndex == row.PossibleBreakPoints.Count)
                            {
                                break;
                            }

                            ReadOnlyMemory<char> nextPortion;

                            if (breakPointIndex == -1)
                            {
                                nextPortion = row.Text.AsMemory(0, row.PossibleBreakPoints[0] + 1);
                            }
                            //until the second to last break point
                            else if (breakPointIndex + 1 < row.PossibleBreakPoints.Count)
                            {
                                int startIndex = row.PossibleBreakPoints[breakPointIndex] + 1;
                                nextPortion = row.Text.AsMemory(
                                    startIndex, row.PossibleBreakPoints[breakPointIndex + 1] - startIndex + 1);
                            }
                            //the last break point
                            else
                            {
                                nextPortion = row.Text.AsMemory(row.PossibleBreakPoints[breakPointIndex] + 1);
                            }

                            float nextPortionWidth = TextMeasuringCache.GetValueOrCalculate(nextPortion, fontSize);

                            //exit when at least one portion was added and there is no room for the next portion
                            if (hasFitAtLeastOne &&
                                currentRowWidth + nextPortionWidth > AbsoluteWidth)
                            {
                                if (BreakMode == TextBreakMode.NoBreak)
                                {
                                    if (lastConvenientBreakPosition != -1)
                                    {
                                        sb.Remove(lastConvenientBreakPosition, sb.Length - lastConvenientBreakPosition);
                                        //this will remain in place (no incrementation on the next loop)
                                        breakPointIndex = lastConvenientBreakPointIndex;
                                        needsHyphen = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            sb.Append(nextPortion);
                            currentRowWidth += nextPortionWidth;
                            breakPointIndex++;
                            hasFitAtLeastOne = true;

                            char lastCharacter = nextPortion[^1..].ToArray()[0];
                            needsHyphen = !char.IsWhiteSpace(lastCharacter);

                            if (BreakMode == TextBreakMode.NoBreak)
                            {
                                if (char.IsWhiteSpace(lastCharacter))
                                {
                                    lastConvenientBreakPosition = row.PossibleBreakPoints[breakPointIndex] + 1;
                                    lastConvenientBreakPointIndex = breakPointIndex;
                                }
                            }
                        }

                        //the second condition states that it must NOT be the end of the row (you can't have a hyphen
                        //on the end of a user row)
                        if (needsHyphen && breakPointIndex < row.PossibleBreakPoints.Count)
                        {
                            currentRowWidth += hyphenCharacterWidth;
                            sb.Append(HyphenCharacter);
                        }

                        //to remain on position (-1+1)
                        breakPointIndex--;
                        _drawableRows.Add(new RowInformation { Text = sb.ToString(), Width = currentRowWidth });

                        currentHeight += rowHeight;
                        if (currentRowWidth > _maxRowWidth)
                        {
                            _maxRowWidth = currentRowWidth;
                        }
                    }

                    userRowIdx++;
                }
            }
            //if no word wrap, then just consider the newlines manually inserted by the developer (_userRows)
            else
            {
                switch (OverflowMode)
                {
                    //TODO: treat clip case
                    case TextOverflowMode.Overflow:
                    case TextOverflowMode.Clip:
                        {
                            foreach (RowInformation row in _userRows)
                            {
                                stoppedEarly = MustStop(currentHeight, rowHeight);
                                if (stoppedEarly)
                                {
                                    goto End;
                                }

                                _drawableRows.Add(new RowInformation { Text = row.Text, Width = row.Width });
                                currentHeight += rowHeight;
                            }

                            break;
                        }
                    case TextOverflowMode.Ellipsis:
                        {
                            foreach (RowInformation row in _userRows)
                            {
                                stoppedEarly = MustStop(currentHeight, rowHeight);
                                if (stoppedEarly)
                                {
                                    goto End;
                                }

                                StringBuilder sb = new();

                                //if the entire row can be drawn
                                if (painter.BreakText(row.Text, AbsoluteWidth) == row.Text.Length)
                                {
                                    sb.Append(row.Text.AsSpan());
                                }
                                else
                                {
                                    sb.Append(row.Text.AsSpan(
                                        0,
                                        (int)painter.BreakText(
                                            row.Text, AbsoluteWidth - overflowStringWidth)));
                                    sb.Append(OverflowString);
                                }

                                string text = sb.ToString();
                                _drawableRows.Add(new RowInformation
                                {
                                    Text = text, Width = painter.MeasureText(text)
                                });

                                currentHeight += rowHeight;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentException("The OverflowMode is invalid.");
                }
            }

        End:
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (AllowsExpansion)
            {
                //the height minus last row (because it is added even when there are no more characters left)
                AbsoluteHeight = currentHeight - ((rowHeight / 2f) + (fontSize / 2f));
            }

            //edge case: the label doesn't have the width necessary for not even a single row
            if (AbsoluteHeight < (rowHeight / 2f) + (fontSize / 2f))
            {
                _drawableRows.Clear();
                return;
            }

            if (stoppedEarly)
            {
                _drawableRows[^1] = new RowInformation
                {
                    Text = OverflowString,
                    PossibleBreakPoints = new List<int>(),
                    Width = overflowStringWidth,
                    WidthWithoutLastBreakPoint = 0
                };
            }
        }

        private bool MustStop(float currentHeight, float rowHeight)
        {
            if (AllowsExpansion)
            {
                return false;
            }

            switch (OverflowMode)
            {
                case TextOverflowMode.Ellipsis:
                    return currentHeight > AbsoluteHeight;
                case TextOverflowMode.Clip:
                    return currentHeight + rowHeight > AbsoluteHeight;
                default:
                case TextOverflowMode.Overflow:
                    return false;
            }
        }


        private sealed class RowInformation
        {
            public string Text { get; set; } = "";
            public float Width { get; set; }

            /// <summary>
            /// Represents the width of the row without considering the last break point, meaning it will be the row's
            /// width if we move the last portion of text to the next row.
            /// </summary>
            public float WidthWithoutLastBreakPoint { get; set; }

            /// <summary>
            /// Contains the indices of the characters that could be used for word wrapping (line break), generally
            /// space and the character before a soft hyphen (shy, U+00AD). The list is for each break point.
            /// The indices are always relative to the row, not to the <see cref="TextElement.Text"/>.
            /// </summary>
            /// <example>
            /// Text: "fgh-ijk-lm opq"
            /// List: [2, 5, 8]
            /// </example>
            public List<int> PossibleBreakPoints { get; set; } = new();
        }
    }
}
