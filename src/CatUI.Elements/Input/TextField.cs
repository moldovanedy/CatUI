using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Data.Events.Input.Keyboard;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Behaviors;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.Utils;

namespace CatUI.Elements.Input;

public class TextField : InputField, IFocusable
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
        /// Specifies the brush used for drawing the text caret. The default value is a black color brush.
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

        private IBrush _brush = new ColorBrush(new Color(0x00_00_ff));

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


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

        if (_isDrawingCaret)
        {
            Document?.Renderer.DrawRect(
                new Rect(
                    new Point2D(Bounds.X + _caretTopLeftPosition.X, Bounds.Y + _caretTopLeftPosition.Y),
                    new Size(
                        CalculateDimension((Dimension)CaretOptions.Width, Bounds.Height),
                        CalculateDimension(_label.FontSize, Bounds.Height))),
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

        _scroller = containerRef.Value!;
        _scroller.Ref = null;
        _label = labelRef.Value!;
        _label.Ref = null;
        _labelParent = labelParentRef.Value!;
        _labelParent.Ref = null;

        InternalContainer.Children.Add(scrollContainer);
        Children.Add(InternalContainer);

        IsFocusEnabledProperty.ValueChangedEvent += SetIsFocusEnabled;
        SelectionProperty.ValueChangedEvent += SetSelection;
        CaretOptionsProperty.ValueChangedEvent += SetCaretOptions;

        CharTypedEvent += PrivateOnCharTyped;
        KeyEvent += PrivateOnKey;
        PointerDownEvent += PrivateOnPointerDown;

        //caret control
        _caretTimer.Interval = _caretOptions.BlinkInterval;
        _caretTimer.Elapsed += (_, _) =>
        {
            _isDrawingCaret = !_isDrawingCaret;
            RequestRedraw();
        };
        EnterDocumentEvent += _ =>
        {
            _caretTimer.Start();
        };
        ExitDocumentEvent += _ =>
        {
            _caretTimer.Stop();
        };
    }


    private void PrivateOnCharTyped(object sender, CharTypedEventArgs e)
    {
        string newText = _label.Text;
        if (Selection.Start.Value != Selection.End.Value)
        {
            // newText =
            //     string.Concat(
            //         newText.AsSpan(0, Selection.Start.Value),
            //         newText.AsSpan(Selection.End.Value, newText.Length - Selection.End.Value));

            newText = newText.Remove(Selection.Start.Value, Selection.End.Value - Selection.Start.Value);
            Selection = new Range(Selection.Start.Value, Selection.Start.Value);
        }

        if (Selection.Start.Value == newText.Length)
        {
            _label.Text = newText + e.Character;
        }
        else
        {
            _label.Text = newText.Insert(Selection.Start.Value, e.Character.ToString());
        }

        Selection = new Range(Selection.Start.Value + 1, Selection.Start.Value + 1);
    }

    private void PrivateOnKey(object sender, KeyEventArgs e)
    {
        //TODO: configurable shortcuts
        if (
            e.Key != PhysicalKey.Backspace
         || _label.Text.Length == 0
         || (e.Action != KeyAction.Pressed && e.Action != KeyAction.Repeat)
            //the caret is at the start and there is no selection
         || (Selection.Start.Value == Selection.End.Value && Selection.Start.Value == 0))
        {
            return;
        }

        //the caret is at the end
        if (Selection.Start.Value == _label.Text.Length)
        {
            _label.Text = _label.Text.Substring(0, _label.Text.Length - 1);
            Selection = new Range(Selection.Start.Value - 1, Selection.Start.Value - 1);
        }
        //the caret is not at the end, but there is no selection
        else if (Selection.Start.Value == Selection.End.Value)
        {
            _label.Text = _label.Text.Remove(Selection.Start.Value - 1, 1);
        }
        //text selected
        else
        {
            _label.Text = _label.Text.Remove(Selection.Start.Value, Selection.End.Value - Selection.Start.Value);

            // _label.Text =
            //     string.Concat(
            //         _label.Text.AsSpan(0, Selection.Start.Value),
            //         _label.Text.AsSpan(Selection.End.Value, _label.Text.Length - Selection.End.Value));
        }
    }

    private void PrivateOnPointerDown(object sender, PointerDownEventArgs e)
    {
        this.GrabFocus();
    }
}
