using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Timers;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Data.Events.Input.Keyboard;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Gestures;
using CatUI.Data.Managers;
using CatUI.Elements.Behaviors;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.RenderingEngine.GraphicsCaching;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.Elements.Input;

public partial class TextField : InputField, IFocusable
{
    public class TextCaretOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Specifies the interval in milliseconds at which the text caret (or text cursor) toggles between visible and
        /// invisible. A value of 0 disabled blinking completely. The default value is 1000.
        /// </summary>
        /// <example>
        /// A value of 1000 means that the caret is visible for 1 second, then invisible for 1 second, then visible
        /// again, so the cycle repeats.
        /// </example>
        public int BlinkInterval
        {
            get => _blinkInterval;
            set
            {
                _blinkInterval = value;
                OnPropertyChanged();
            }
        }

        private int _blinkInterval = 1000;

        /// <summary>
        /// Specifies the width of the text caret in <see cref="Unit.Dp"/>. The default value is 1.
        /// </summary>
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        private double _width = 1;

        /// <summary>
        /// Specifies the brush used for drawing the text caret. The default value is a black color brush, but a text
        /// field generally sets it to the font color.
        /// </summary>
        public IBrush Brush
        {
            get => _brush;
            set
            {
                _brush = value;
                OnPropertyChanged();
            }
        }

        private IBrush _brush = new ColorBrush(new Color(0x00_00_00));

        /// <summary>
        /// Specifies the brush used for drawing the text caret. The default value is a black color brush, but a text
        /// field generally sets it to the font color.
        /// </summary>
        public IBrush SelectionBrush
        {
            get => _selectionBrush;
            set
            {
                _selectionBrush = value;
                OnPropertyChanged();
            }
        }

        //default taken from https://stackoverflow.com/a/16094931/23361865 (Chrome 107)
        private IBrush _selectionBrush = new ColorBrush(new Color(0x0_74_ff_cc, Color.ColorType.RGBA));

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    private const float LEFT_RIGHT_LABEL_PADDING = 3f;
    private const float CARET_EXTRA_SIZE = 4f;

    private static readonly Regex WordBreakRegex = WordBreakCompiledRegex();

    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<TextField>? Ref
    {
        get => _ref;
        set
        {
            _ref = value;
            if (_ref != null)
            {
                _ref.Value = this;
            }
        }
    }

    private ObjectRef<TextField>? _ref;

    /// <summary>
    /// Represents the current text selection and the caret position. The start value is the caret position (the index
    /// of the character AFTER the caret), the end value is either the caret position when there is no text selected or
    /// the index of the first element that is NOT selected (the character after the selection).
    /// </summary>
    /// <example>
    /// Text: ABCDE, caret is between B and C; Selection = (2, 2)<br />
    /// Text: ABCDE, selection starts at B and ends at D (BCD is selected); Selection = (1, 4)<br />
    /// Text: ABCDE, all text is selected; Selection = (0, 5)<br />
    /// </example>
    public Range Selection
    {
        get => _selection;
        set => SelectionProperty.Value = value;
    }

    private Range _selection = new(0, 0);

    public ObservableProperty<Range> SelectionProperty { get; } = new(new Range(0, 0));

    private void SetSelection(Range value)
    {
        _selection = value;
        SetLocalValue(nameof(Selection), value);
        MarkLayoutDirty();
    }

    /// <summary>
    /// Specifies the options used to draw the caret. See <see cref="TextCaretOptions"/> for more info.
    /// </summary>
    public TextCaretOptions CaretOptions
    {
        get => _caretOptions;
        set => CaretOptionsProperty.Value = value;
    }

    private TextCaretOptions _caretOptions = new();

    public ObservableProperty<TextCaretOptions> CaretOptionsProperty { get; } = new(new TextCaretOptions());

    private void SetCaretOptions(TextCaretOptions? value)
    {
        _caretOptions = value!;
        SetLocalValue(nameof(CaretOptions), value);
        MarkLayoutDirty();

        _caretTimer.Interval = _caretOptions.BlinkInterval;
    }

    /// <summary>
    /// The element that holds all the internal elements of this field. It always has 2 children. DO NOT manually
    /// add or remove the children of this element. Use <see cref="ActualValueElement"/> and <see cref="DecorElement"/>
    /// instead.
    /// </summary>
    public Element InternalContainer { get; private set; }

    /// <inheritdoc cref="InputField.ActualValueElement"/>
    /// <remarks>
    /// For TextField, this must be a <see cref="Label"/>, otherwise a <see cref="ArgumentException"/> is thrown.
    /// This will be the first child of a <see cref="ColumnContainer"/> in a hierarchy that is eventually the second
    /// child of <see cref="InternalContainer"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when setting this to an instance that is not of type <see cref="Label"/>.
    /// </exception>
    public override Element ActualValueElement
    {
        get => _label;
        set
        {
            if (value is Label label)
            {
                _label = label;
                _scroller.Children[0] = label;
            }
            else
            {
                throw new ArgumentException("The value element must be of type Label", nameof(ActualValueElement));
            }
        }
    }

    private Label _label;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This will be the first child of <see cref="InternalContainer"/>.
    /// </remarks>
    public Element DecorElement
    {
        get => _decorElement;
        set
        {
            _decorElement = value;
            InternalContainer.Children[0] = _decorElement;
        }
    }

    private Element _decorElement;

    private ScrollContainer _scroller = null!;
    private ColumnContainer _labelParent = null!;

    private Point2D _caretTopLeftPosition = new(2, 0);
    private bool _isDrawingCaret;
    private readonly Timer _caretTimer = new();
    private readonly List<float> _characterSizes = new(1024);

    private bool _isPointerDown;
    private float _previousMousePos;
    private (float, float) _selectionPositionRange = (0, 0);
    private (float, float) _selectionFractionalPositionRange = (0, 0);

    #region Focus

    public IFocusable? NextFocusableElement { get; set; }
    public IFocusable? PreviousFocusableElement { get; set; }

    public void OnFocusStateChanged(bool hasEnteredFocus)
    {
        if (hasEnteredFocus)
        {
            AddPseudoClass(IFocusable.PSEUDO_CLASS_FOCUSED);
        }
        else
        {
            RemovePseudoClass(IFocusable.PSEUDO_CLASS_FOCUSED);
        }

        FocusChangedEvent?.Invoke(this, hasEnteredFocus);
    }

    public bool IsFocusEnabled
    {
        get => _isFocusEnabled;
        set => IsFocusEnabledProperty.Value = value;
    }

    private bool _isFocusEnabled = true;
    public ObservableProperty<bool> IsFocusEnabledProperty { get; } = new(true);

    private void SetIsFocusEnabled(bool value)
    {
        _isFocusEnabled = value;
        SetLocalValue(nameof(IsFocusEnabled), value);
    }

    public event FocusChangedEventHandler? FocusChangedEvent;

    public FocusChangedEventHandler? OnFocusChanged
    {
        get => _onFocusChanged;
        set
        {
            FocusChangedEvent -= _onFocusChanged;
            _onFocusChanged = value;
            FocusChangedEvent += _onFocusChanged;
        }
    }

    private FocusChangedEventHandler? _onFocusChanged;

    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public TextField()
    {
        Init();
    }

    public TextField(TextField other) : base(other)
    {
        Init();

        Selection = other.Selection;
        CaretOptions = other.CaretOptions;
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Returns the text of <see cref="ActualValueElement"/> (i.e. the text of this field).
    /// </summary>
    /// <returns></returns>
    public override string GetInputTextualRepresentation()
    {
        return _label.Text;
    }

    protected override void Draw(object sender)
    {
        base.Draw(sender);

        //selection
        Document?.Renderer.DrawRect(
            new Rect(
                new Point2D(Bounds.X + _selectionPositionRange.Item1, Bounds.Y + _caretTopLeftPosition.Y),
                new Size(
                    _selectionPositionRange.Item2 - _selectionPositionRange.Item1,
                    CalculateDimension(_label.FontSize, Bounds.Height) + CARET_EXTRA_SIZE)),
            CaretOptions.SelectionBrush);

        if (_isDrawingCaret)
        {
            Document?.Renderer.DrawRect(
                new Rect(
                    new Point2D(Bounds.X + _caretTopLeftPosition.X, Bounds.Y + _caretTopLeftPosition.Y),
                    new Size(
                        CalculateDimension((Dimension)CaretOptions.Width, Bounds.Height),
                        CalculateDimension(_label.FontSize, Bounds.Height) + CARET_EXTRA_SIZE)),
                CaretOptions.Brush);
        }
    }

    private void Init()
    {
        InternalContainer = new Element { Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%") };

        _decorElement = new RectangleElement(null, new ColorBrush(0xFF_FF_FF_FF))
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
        };
        InternalContainer.Children.Add(_decorElement);

        ObjectRef<ScrollContainer> containerRef = new();
        ObjectRef<ColumnContainer> labelParentRef = new();
        ObjectRef<Label> labelRef = new();

        ScrollContainer scrollContainer = new()
        {
            Ref = containerRef,
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            Content = new PaddingElement(new EdgeInset(0, 3))
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                Children =
                [
                    new ColumnContainer
                    {
                        Ref = labelParentRef,
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                        Children =
                        [
                            new Label
                            {
                                Ref = labelRef,
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 0, Dimension.Unset)
                                        .SetMinMaxAndPreferredHeight("100%", 0, Dimension.Unset)
                            }
                        ]
                    }
                ]
            }
        };

        if (labelRef.Value != null)
        {
            labelRef.Value.TextBrushProperty.ValueChangedEvent += brush =>
            {
                CaretOptions.Brush = brush ?? new ColorBrush(new Color(0x00_00_00));
            };
        }

        _scroller = containerRef.Value!;
        _scroller.Ref = null;
        _label = labelRef.Value!;
        _label.Ref = null;
        _labelParent = labelParentRef.Value!;
        _labelParent.Ref = null;

        Cursor = CursorIcon.CURSOR_TEXT;
        InternalContainer.Children.Add(scrollContainer);
        Children.Add(InternalContainer);

        IsFocusEnabledProperty.ValueChangedEvent += SetIsFocusEnabled;
        SelectionProperty.ValueChangedEvent += SetSelection;
        CaretOptionsProperty.ValueChangedEvent += SetCaretOptions;

        FocusChangedEvent += PrivateFocusChanged;
        CharTypedEvent += PrivateOnCharTyped;
        KeyEvent += PrivateOnKey;

        PointerDownEvent += PrivateOnPointerDown;
        PointerUpEvent += PrivateOnPointerUp;
        PointerMoveEvent += PrivateOnPointerMove;

        //caret control
        _caretTimer.Interval = _caretOptions.BlinkInterval;
        _caretTimer.Elapsed += (_, _) =>
        {
            _isDrawingCaret = !_isDrawingCaret;
            RequestRedraw();
        };
        ExitDocumentEvent += _ =>
        {
            _caretTimer.Stop();
        };
    }


    private void PrivateFocusChanged(object sender, bool hasEnteredFocus)
    {
        if (hasEnteredFocus)
        {
            _isDrawingCaret = true;
            _caretTimer.Interval = _caretOptions.BlinkInterval;
            _caretTimer.Start();
        }
        else
        {
            _isDrawingCaret = false;
            _caretTimer.Stop();
        }
    }

    private void PrivateOnCharTyped(object sender, CharTypedEventArgs e)
    {
        string newText = _label.Text;
        if (Selection.Start.Value != Selection.End.Value)
        {
            newText = newText.Remove(Selection.Start.Value, Selection.End.Value - Selection.Start.Value);
            Selection = new Range(Selection.Start.Value, Selection.Start.Value);
        }

        SKPaint painter = _label.TextBrush.ToSkiaPaint();
        float charSize = TextMeasuringCache.Calculate([e.Character], painter);

        if (Selection.Start.Value == newText.Length)
        {
            _label.Text = newText + e.Character;
            _characterSizes.Add(charSize);
        }
        else
        {
            _label.Text = newText.Insert(Selection.Start.Value, e.Character.ToString());
            _characterSizes.Insert(Selection.Start.Value, charSize);
        }

        UpdateSelectionAndCaret(new Range(Selection.Start.Value + 1, Selection.Start.Value + 1));
    }

    private void PrivateOnKey(object sender, KeyEventArgs e)
    {
        if (_label.Text.Length == 0)
        {
            return;
        }

        if (e.Action == KeyAction.Released)
        {
            return;
        }

        //deletion
        if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_DELETE_FROM_BEGINNING))
        {
            Delete(true);
        }
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_DELETE_FROM_END))
        {
            Delete(false);
        }
        //one char navigation
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_NAVIGATE_TO_LEFT))
        {
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                EndNav(1);
            }
            else
            {
                HomeNav(1);
            }
        }
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_NAVIGATE_TO_RIGHT))
        {
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                HomeNav(1);
            }
            else
            {
                EndNav(1);
            }
        }
        //one word navigation
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_NAVIGATE_TO_NEXT_WORD))
        {
            int pos = GoToAdjacentWord(true);
            EndNav(pos - Selection.End.Value);
        }
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_NAVIGATE_TO_PREVIOUS_WORD))
        {
            int pos = GoToAdjacentWord(false);
            HomeNav(Selection.Start.Value - pos);
        }
        //entire row navigation
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_NAVIGATE_TO_ROW_BEGINNING))
        {
            //an enormous value to ensure that an end is reached (will get clamped anyway) while avoiding overflow;
            // this the middle of the integer range
            HomeNav(1 << 30);
        }
        else if (InputManager.IsShortcutCurrentlyTriggered(DefaultShortcutNames.TEXT_NAVIGATE_TO_ROW_END))
        {
            EndNav(1 << 30);
        }
    }

    private void PrivateOnPointerDown(object sender, PointerDownEventArgs e)
    {
        _isPointerDown = true;
        this.GrabFocus();

        float xPos = e.Position.X - LEFT_RIGHT_LABEL_PADDING;
        float currentCharPos = 0;
        float previousCharPos = 0;
        int charIdx = 0;

        while (currentCharPos < xPos && charIdx < _characterSizes.Count)
        {
            previousCharPos = currentCharPos;
            currentCharPos += _characterSizes[charIdx];
            charIdx++;
        }

        //if it was closer to the previous char than to this one, go to the previous char
        if (Math.Abs(xPos - previousCharPos) < Math.Abs(xPos - currentCharPos))
        {
            charIdx--;
        }

        _selectionPositionRange = (currentCharPos, currentCharPos);
        _selectionFractionalPositionRange = (currentCharPos, currentCharPos);
        UpdateSelectionAndCaret(new Range(charIdx, charIdx));
    }

    private void PrivateOnPointerUp(object sender, PointerUpEventArgs e)
    {
        _isPointerDown = false;
        _previousMousePos = 0;
    }

    private void PrivateOnPointerMove(object sender, PointerMoveEventArgs e)
    {
        if (!_isPointerDown)
        {
            return;
        }

        if (_previousMousePos == 0)
        {
            _previousMousePos = e.Position.X;
        }

        float currentMousePos = e.Position.X;
        bool wasSelectionModified = false;
        Range newSelection = Selection;

        //going to the right
        if (currentMousePos > _selectionFractionalPositionRange.Item2)
        {
            _selectionFractionalPositionRange.Item2 = currentMousePos;

            //if rightwards from the last selected letter
            if (
                _selectionFractionalPositionRange.Item2 > _selectionPositionRange.Item2
             && Selection.End.Value < _label.Text.Length - 1)
            {
                //if the distance between the raw selection and the actual selection is more than half of the letter,
                // move the selection
                float delta = _selectionFractionalPositionRange.Item2 - _selectionPositionRange.Item2;
                if (delta > _characterSizes[Selection.End.Value + 1] / 2.0)
                {
                    wasSelectionModified = true;
                    newSelection = new Range(newSelection.Start, newSelection.End.Value + 1);
                }
            }
        }
        //going to the left, moving the selection end towards the start
        else if (
            currentMousePos < _selectionFractionalPositionRange.Item2
         && currentMousePos > _selectionFractionalPositionRange.Item1)
        {
            _selectionFractionalPositionRange.Item2 = currentMousePos;

            //if leftwards from the last selected letter
            if (
                _selectionFractionalPositionRange.Item2 < _selectionPositionRange.Item2
             && Selection.End.Value > 0)
            {
                //if the distance between the raw selection and the actual selection is more than half of the letter,
                // move the selection
                float delta = _selectionPositionRange.Item2 - _selectionFractionalPositionRange.Item2;
                if (delta > _characterSizes[Selection.End.Value - 1] / 2.0)
                {
                    wasSelectionModified = true;
                    newSelection = new Range(newSelection.Start, newSelection.End.Value - 1);
                }
            }
        }
        //going to the left, moving the selection start to left
        else
        {
            _selectionFractionalPositionRange.Item1 = currentMousePos;

            //if leftwards from the last selected letter
            if (
                _selectionFractionalPositionRange.Item2 < _selectionPositionRange.Item2
             && Selection.Start.Value > 0)
            {
                //if the distance between the raw selection and the actual selection is more than half of the letter,
                // move the selection
                float delta = _selectionPositionRange.Item2 - _selectionFractionalPositionRange.Item2;
                if (delta > _characterSizes[Selection.End.Value - 1] / 2.0)
                {
                    wasSelectionModified = true;
                    newSelection = new Range(newSelection.Start.Value - 1, newSelection.End);
                }
            }
        }

        _previousMousePos = e.Position.X;
        if (wasSelectionModified)
        {
            UpdateSelectionAndCaret(newSelection);
        }
    }


    private int GoToAdjacentWord(bool wantsNextWord)
    {
        //TODO: handle this according to UAX #29 (Unicode Standard Annex), this is only an approximation;
        // this also does not work properly, as it skips words sometimes

        int pos = wantsNextWord ? Selection.End.Value : Selection.Start.Value;
        if (wantsNextWord)
        {
            Match nextMatch = WordBreakRegex.Match(_label.Text, pos);
            return nextMatch.Length > 0 ? nextMatch.Index + nextMatch.Length : _label.Text.Length;
        }

        //we heuristically try to reduce the workload of the RegEx matcher by only starting at the last whitespace
        // (whitespace is always a word boundary)
        int currentPos = 0;
        int lastWhitespacePos = 0;

        while (currentPos < pos)
        {
            if (char.IsWhiteSpace(_label.Text[currentPos]))
            {
                lastWhitespacePos = currentPos;
            }

            currentPos++;
        }

        MatchCollection prevMatches = WordBreakRegex.Matches(_label.Text, lastWhitespacePos);
        if (prevMatches.Count == 0)
        {
            return 0;
        }

        int startIdx = 0;
        for (int i = 0; i < prevMatches.Count; i++)
        {
            if (prevMatches[i].Index > pos)
            {
                break;
            }

            startIdx = prevMatches[i].Index;
        }

        return startIdx - prevMatches[^1].Length;
    }

    private void Delete(bool isFromBackspace)
    {
        Range newRange;

        //there is no selection
        if (Selection.Start.Value == Selection.End.Value)
        {
            //backspace key and not at the start
            if (isFromBackspace && Selection.Start.Value > 0)
            {
                _label.Text = _label.Text.Remove(Selection.Start.Value - 1, 1);
                newRange = new Range(Selection.Start.Value - 1, Selection.Start.Value - 1);
            }
            //delete key and not at the end
            else if (!isFromBackspace && Selection.Start.Value < _label.Text.Length)
            {
                _label.Text = _label.Text.Remove(Selection.Start.Value, 1);
                newRange = new Range(Selection.Start.Value, Selection.Start.Value);
                //forcefully remove the curent character
                _characterSizes.RemoveAt(Selection.Start.Value);
            }
            //no action
            else
            {
                newRange = Selection;
            }
        }
        //text selected
        else
        {
            _label.Text = _label.Text.Remove(Selection.Start.Value, Selection.End.Value - Selection.Start.Value);
            newRange = new Range(Selection.Start.Value, Selection.Start.Value);
        }

        int selectionEnd = Selection.End.Value;
        UpdateSelectionAndCaret(newRange);
        _characterSizes.RemoveRange(newRange.Start.Value, selectionEnd - newRange.Start.Value);
    }

    private void HomeNav(int numCharacters)
    {
        int position = Math.Max(0, Selection.Start.Value - numCharacters);
        UpdateSelectionAndCaret(new Range(position, position));
    }

    private void EndNav(int numCharacters)
    {
        int position = Math.Min(Selection.End.Value + numCharacters, _label.Text.Length);
        UpdateSelectionAndCaret(new Range(position, position));
    }

    private void UpdateSelectionAndCaret(Range newSelection)
    {
        if (
            newSelection.Start.Value > newSelection.End.Value
         || newSelection.Start.Value < 0
         || newSelection.End.Value > _label.Text.Length)
        {
            return;
        }

        //reset the caret and its timer
        _isDrawingCaret = true;
        _caretTimer.Interval = _caretOptions.BlinkInterval;

        float textSize = 0;

        if (newSelection.Start.Value == 0)
        {
            _selectionPositionRange.Item1 = 0;
        }

        if (newSelection.End.Value == 0)
        {
            _selectionPositionRange.Item2 = 0;
        }

        for (int i = 0; i < _characterSizes.Count; i++)
        {
            float charSize = _characterSizes[i];
            textSize += charSize;

            if (i == newSelection.Start.Value - 1)
            {
                _selectionPositionRange.Item1 = textSize;
            }

            if (i == newSelection.End.Value - 1)
            {
                _selectionPositionRange.Item2 = textSize;
            }
        }

        _caretTopLeftPosition = new Point2D(
            _selectionPositionRange.Item2 + LEFT_RIGHT_LABEL_PADDING,
            _caretTopLeftPosition.Y);
        Selection = newSelection;
    }

    [GeneratedRegex(@"\b\w+\b", RegexOptions.Compiled)]
    private static partial Regex WordBreakCompiledRegex();
}
