using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Elements.Behaviors;
using CatUI.RenderingEngine.GraphicsCaching;
using SkiaSharp;

namespace CatUI.Elements.Text
{
    /// <summary>
    /// A complex element that is designed to display text. It has advanced text features such as word wrap, multiple
    /// lines support and is expandable. It is pretty efficient for both small and large amounts of text, so it can
    /// be used to display entire paragraphs of text.
    /// </summary>
    public class Label : TextElement, IWordWrappable, IExpandable
    {
        /// <inheritdoc cref="IWordWrappable.WordWrap"/>
        /// <remarks>
        /// </remarks>
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                WordWrapProperty.Value = value;
            }
        }

        private bool _wordWrap;
        public ObservableProperty<bool> WordWrapProperty { get; } = new(false);

        /// <inheritdoc cref="IExpandable.CanExpandHorizontally"/>
        /// <remarks>
        /// By default, this is false.
        /// </remarks>
        public bool CanExpandHorizontally
        {
            get => _canExpandHorizontally;
            set
            {
                _canExpandHorizontally = value;
                CanExpandHorizontallyProperty.Value = value;
            }
        }

        private bool _canExpandHorizontally;
        public ObservableProperty<bool> CanExpandHorizontallyProperty { get; } = new(false);

        /// <inheritdoc cref="IExpandable.CanExpandVertically"/>
        /// <remarks>
        /// By default, this is false. If it's true, when <see cref="Element.MaxHeight"/> is reached,
        /// <see cref="TextElement.OverflowString"/> will be put at the end of the last row IF it can be drawn without
        /// exceeding the maximum width; otherwise, it will remove some characters from the row (not taking into account
        /// the hyphens or spaces, simply removing a maximum of 3 characters regardless of the length of
        /// <see cref="TextElement.OverflowString"/>) to accomodate for <see cref="TextElement.OverflowString"/>.
        /// </remarks>
        public bool CanExpandVertically
        {
            get => _canExpandVertically;
            set
            {
                _canExpandVertically = value;
                CanExpandVerticallyProperty.Value = value;
            }
        }

        private bool _canExpandVertically;
        public ObservableProperty<bool> CanExpandVerticallyProperty { get; } = new(false);

        /// <summary>
        /// Represents the text's word break mode. It is only relevant when <see cref="WordWrap"/> is true.
        /// See <see cref="TextBreakMode"/> for more info. The default value is <see cref="TextBreakMode.SoftBreak"/>.
        /// </summary>
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

        public ObservableProperty<TextBreakMode> BreakModeProperty { get; } = new(TextBreakMode.SoftBreak);

        /// <summary>
        /// Represents the character that will be used for hyphenation when <see cref="BreakMode"/> is
        /// <see cref="TextBreakMode.SoftBreak"/> and <see cref="TextElement.Text"/> has soft hyphens. In other cases
        /// it's irrelevant (that's most of the time because hyphenation is not used that much). The default value is
        /// '-' (U+002D or ASCII 45).
        /// </summary>
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
        public ObservableProperty<char> HyphenCharacterProperty { get; } = new('-');

        /// <summary>
        /// Represents the brush that is used to draw the text. The default value is a solid black color.
        /// </summary>
        public IBrush TextBrush
        {
            get => _textBrush;
            set
            {
                _textBrush = value;
                TextBrushProperty.Value = value;
            }
        }

        private IBrush _textBrush = new ColorBrush(new Color(0));
        public ObservableProperty<IBrush> TextBrushProperty { get; } = new(new ColorBrush(new Color(0)));

        /// <summary>
        /// Represents the brush that is used to create the text outline. The default value is a completely transparent
        /// color, so it won't get drawn.
        /// </summary>
        public IBrush OutlineTextBrush
        {
            get => _outlineTextBrush;
            set
            {
                _outlineTextBrush = value;
                OutlineTextBrushProperty.Value = value;
            }
        }

        private IBrush _outlineTextBrush = new ColorBrush(Color.Default);
        public ObservableProperty<IBrush> OutlineTextBrushProperty { get; } = new(new ColorBrush(Color.Default));

        /// <summary>
        /// A dimensionless value that represents the spacing between the rows. A value of 1 will not leave any space
        /// (note that space might be visible depending on the text and the used font), otherwise it's this value - 1
        /// multiplied with <see cref="TextElement.FontSize"/>. The default value is 1.2.
        /// </summary>
        /// <remarks>Negative values are not allowed and will be clamped to 0.</remarks>
        public float LineHeight
        {
            get => _lineHeight;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _lineHeight = value;
                LineHeightProperty.Value = value;
            }
        }

        private float _lineHeight = 1.2f;
        public ObservableProperty<float> LineHeightProperty { get; } = new(1.2f);

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
        /// Only relevant when <see cref="WordWrap"/> is true.
        /// </summary>
        private float _maxRowWidth;

        /// <summary>
        /// Represents the total height of the visible text (the number of rows * (font size + some line height calculation)).
        /// Does NOT include the line height spacing after the last visible row.
        /// </summary>
        private float _visibleTextTotalHeight;

        public Label(
            string text,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                text,
                textAlignment,
                preferredWidth,
                preferredHeight)
        {
            TextProperty.ValueChangedEvent += OnTextChanged;
            TextProperty.ForceRecallEvents();
        }

        ~Label()
        {
            TextProperty.ValueChangedEvent -= OnTextChanged;
        }

        internal override void RecalculateLayout()
        {
            float normalWidth = 0, normalHeight = 0;

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

            float maxWidth = MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth);
            float maxHeight = MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight);

            if (!PreferredWidth.IsUnset())
            {
                if (CanExpandHorizontally)
                {
                    normalWidth = maxWidth;
                }
                else
                {
                    normalWidth = Math.Clamp(
                        CalculateDimension(PreferredWidth, parentWidth),
                        MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                        maxWidth);
                    AbsoluteWidth = normalWidth;
                }
            }

            if (!PreferredHeight.IsUnset())
            {
                if (CanExpandVertically)
                {
                    normalHeight = maxHeight;
                }
                else
                {
                    normalHeight = Math.Clamp(
                        CalculateDimension(PreferredHeight, parentHeight),
                        MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                        maxHeight);
                    AbsoluteHeight = normalHeight;
                }
            }

            if (!Position.IsUnset())
            {
                var normalPosition = new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight));
                AbsolutePosition = normalPosition;
            }

            //it is very likely that this is when the label is instantiated, and it is unnecessary to calculate text here
            if (normalWidth == 0 && normalHeight == 0)
            {
                return;
            }

            //temp
            _visibleTextTotalHeight = float.PositiveInfinity;

            if (!string.IsNullOrEmpty(Text) && (normalWidth < _maxRowWidth || normalHeight < _visibleTextTotalHeight))
            {
                CreateFinalText(
                    CanExpandHorizontally ? maxWidth : AbsoluteWidth,
                    CanExpandVertically ? maxHeight : AbsoluteHeight);
            }

            foreach (Element child in Children)
            {
                child.RecalculateLayout();
            }
        }

        public override void Draw()
        {
            base.Draw();

            float fontSize = CalculateDimension(FontSize);
            float rowSize = fontSize * LineHeight;
            //half of line height + 0.5 (so for line height of 2 it is 1 + 0.5, for 4 is 2 + 0.5 etc.)
            Point2D rowPosition = new(Bounds.StartPoint.X, Bounds.StartPoint.Y + (rowSize / 2f) + (fontSize / 2f));

            int rowsDrawn = 0;
            int charactersDrawn = 0;
            while (
                rowsDrawn < _drawableRows.Count &&
                charactersDrawn < Text.Length &&
                (CanExpandVertically || rowPosition.Y <= Bounds.StartPoint.Y + Bounds.Height))
            {
                Document?.Renderer?.DrawTextRowFast(
                    _drawableRows[rowsDrawn].Text,
                    rowPosition,
                    fontSize,
                    new Size(AbsoluteWidth, AbsoluteHeight),
                    TextBrush,
                    OutlineTextBrush,
                    TextAlignment);
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

        private void CreateFinalText(float maxAllowedWidth, float maxAllowedHeight)
        {
            _drawableRows.Clear();
            _maxRowWidth = 0;
            _visibleTextTotalHeight = 0;

            bool stoppedEarly = false;
            bool usedBreakPoints = false;

            float fontSize = CalculateDimension(FontSize);
            float rowHeight = fontSize * LineHeight;
            SKPaint painter = PaintManager.GetPaint(fontSize: fontSize);

            float overflowStringWidth = TextMeasuringCache.GetValueOrCalculate(OverflowString, fontSize);
            float hyphenCharacterWidth = TextMeasuringCache.GetValueOrCalculate(
                new string(new[] { HyphenCharacter }), fontSize);
            float currentHeight = (rowHeight / 2f) + (fontSize / 2f);

            if (WordWrap)
            {
                int userRowIdx = 0;

                while (userRowIdx < _userRows.Count)
                {
                    RowInformation row = _userRows[userRowIdx];
                    stoppedEarly = MustStop(currentHeight, rowHeight, maxAllowedHeight);
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
                            int length = (int)painter.BreakText(row.Text.AsSpan(), maxAllowedWidth);
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
                            int charCount = (int)painter.BreakText(row.Text.AsSpan(startIndex), maxAllowedWidth);
                            //at least one char to avoid infinite loops (both in code and in UI as infinite newlines)
                            if (charCount == 0)
                            {
                                charCount = 1;
                            }

                            if (charCount == row.Text.AsSpan(startIndex).Length)
                            {
                                usedBreakPoints = true;
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
                        stoppedEarly = MustStop(currentHeight, rowHeight, maxAllowedHeight);
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

                            float nextPortionWidth =
                                TextMeasuringCache.GetValueOrCalculate(nextPortion.ToString(), fontSize);

                            //exit when at least one portion was added and there is no room for the next portion
                            if (hasFitAtLeastOne &&
                                currentRowWidth + nextPortionWidth > maxAllowedWidth)
                            {
                                usedBreakPoints = true;

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
                            usedBreakPoints = true;
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
                                stoppedEarly = MustStop(currentHeight, rowHeight, maxAllowedHeight);
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
                                stoppedEarly = MustStop(currentHeight, rowHeight, maxAllowedHeight);
                                if (stoppedEarly)
                                {
                                    goto End;
                                }

                                StringBuilder sb = new();

                                //if the entire row can be drawn
                                if (painter.BreakText(row.Text, maxAllowedWidth) == row.Text.Length)
                                {
                                    sb.Append(row.Text.AsSpan());
                                }
                                else
                                {
                                    sb.Append(row.Text.AsSpan(
                                        0,
                                        (int)painter.BreakText(
                                            row.Text, maxAllowedWidth - overflowStringWidth)));
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
            if (CanExpandVertically)
            {
                //the height minus last row (because it is added even when there are no more characters left)
                AbsoluteHeight = currentHeight - ((rowHeight / 2f) + (fontSize / 2f));
            }
            else
            {
                AbsoluteHeight = maxAllowedHeight;
            }

            if (CanExpandHorizontally && !usedBreakPoints)
            {
                AbsoluteWidth = _maxRowWidth;
            }
            else
            {
                AbsoluteWidth = maxAllowedWidth;
            }

            //edge case: the label doesn't have the height necessary for not even a single row
            if (AbsoluteHeight < (rowHeight / 2f) + (fontSize / 2f))
            {
                _drawableRows.Clear();
                return;
            }

            if (stoppedEarly)
            {
                //if we can draw the ellipsis without removing from the row, do it
                if (_drawableRows[^1].Width + overflowStringWidth < maxAllowedWidth)
                {
                    _drawableRows[^1] = new RowInformation
                    {
                        Text = string.Concat(_drawableRows[^1].Text, OverflowString),
                        PossibleBreakPoints = new List<int>(),
                        Width = _drawableRows[^1].Width + overflowStringWidth,
                        WidthWithoutLastBreakPoint = 0
                    };
                    return;
                }

                int charsToRemove = _drawableRows[^1].Text.Length;
                float removeWidth = _drawableRows[^1].Width;

                if (_drawableRows[^1].Text.Length >= 3)
                {
                    //the last 5 chars reversed
                    string lastChars = new(
                        _drawableRows[^1].Text
                                         .AsSpan(_drawableRows[^1].Text.Length - 3)
                                         .ToArray().Reverse().ToArray());
                    //calculate the number of characters needed to remove in order to add the overflow string and add 1
                    //to ensure we don't exceed the element bounds, but don't remove more than 3 characters
                    charsToRemove = (int)Math.Min(painter.BreakText(lastChars, overflowStringWidth) + 1, 3);
                    removeWidth = painter.MeasureText(lastChars.AsSpan(0, charsToRemove));
                }

                _drawableRows[^1] = new RowInformation
                {
                    Text = string.Concat(
                        _drawableRows[^1].Text.AsSpan(0, _drawableRows[^1].Text.Length - charsToRemove),
                        OverflowString),
                    PossibleBreakPoints = new List<int>(),
                    Width = _drawableRows[^1].Width - removeWidth + overflowStringWidth,
                    WidthWithoutLastBreakPoint = 0
                };
            }
        }

        private bool MustStop(float currentHeight, float rowHeight, float maxAllowedHeight = 0)
        {
            float referenceHeight = CanExpandVertically ? maxAllowedHeight : AbsoluteHeight;

            switch (OverflowMode)
            {
                case TextOverflowMode.Ellipsis:
                    return currentHeight > referenceHeight;
                case TextOverflowMode.Clip:
                    return currentHeight + rowHeight > referenceHeight;
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
