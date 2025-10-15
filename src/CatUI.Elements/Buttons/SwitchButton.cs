using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Elements.Behaviors;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.ControlFlow;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Utils;

namespace CatUI.Elements.Buttons;

public class SwitchButton : BaseButton, IToggleable
{
    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<SwitchButton>? Ref
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

    private ObjectRef<SwitchButton>? _ref;

    public bool Value
    {
        get => _value;
        set => ValueProperty.Value = value;
    }

    private bool _value;

    public ObservableProperty<bool> ValueProperty { get; } = new(false);

    private void SetValue(bool value)
    {
        _value = value;
        SetLocalValue(nameof(Value), value);
        MarkLayoutDirty();
    }

    /// <summary>
    /// Represents the spacing between <see cref="IndicatorElement"/> and <see cref="TextElement"/>.
    /// </summary>
    public Dimension Spacing
    {
        get => _spacing;
        set => SpacingProperty.Value = value;
    }

    private Dimension _spacing = new();
    public ObservableProperty<Dimension> SpacingProperty { get; } = new(new Dimension());

    private void SetSpacing(Dimension value)
    {
        _spacing = value;
        SetLocalValue(nameof(Spacing), value);
        InternalRowContainer.Arrangement.Spacing = value;
    }

    /// <summary>
    /// Represents the horizontal arrangement of the content. A value other than <see cref="LinearArrangement.JustificationType.Start"/>,
    /// <see cref="LinearArrangement.JustificationType.Center"/> or <see cref="LinearArrangement.JustificationType.End"/>
    /// will make <see cref="Spacing"/> irrelevant. By default, this is <see cref="LinearArrangement.JustificationType.Center"/>.
    /// </summary>
    public LinearArrangement.JustificationType HorizontalArrangement
    {
        get => _horizontalArrangement;
        set => HorizontalArrangementProperty.Value = value;
    }

    private LinearArrangement.JustificationType _horizontalArrangement = LinearArrangement.JustificationType.Center;

    public ObservableProperty<LinearArrangement.JustificationType> HorizontalArrangementProperty
    {
        get;
    } = new(LinearArrangement.JustificationType.Center);

    private void SetHorizontalArrangement(LinearArrangement.JustificationType value)
    {
        _horizontalArrangement = value;
        SetLocalValue(nameof(HorizontalArrangement), value);
        InternalRowContainer.Arrangement.ContentJustification = value;
    }

    /// <summary>
    /// Represents the vertical alignment of the content. By default, this is <see cref="VerticalAlignmentType.Center"/>.
    /// </summary>
    public VerticalAlignmentType VerticalAlignment
    {
        get => _verticalAlignment;
        set => VerticalAlignmentProperty.Value = value;
    }

    private VerticalAlignmentType _verticalAlignment = VerticalAlignmentType.Center;

    public ObservableProperty<VerticalAlignmentType> VerticalAlignmentProperty { get; }
        = new(VerticalAlignmentType.Center);

    private void SetVerticalAlignment(VerticalAlignmentType value)
    {
        _verticalAlignment = value;
        SetLocalValue(nameof(VerticalAlignment), value);
        InternalRowContainer.VerticalAlignment = value;
    }

    /// <summary>
    /// Represents the text content of the radio button. Contrary to the name, this can be any kind of element, but
    /// it's much more common for it to be a <see cref="Label"/>.
    /// </summary>
    /// <remarks>
    /// To see when this is modified, assuming you don't interfere with <see cref="InternalRowContainer"/>'s
    /// children, you can listen to <see cref="ObservableList{T}"/> events on <see cref="Element.Children"/> on
    /// <see cref="InternalRowContainer"/>.
    /// </remarks>
    public Element? TextElement
    {
        get => _textElement;
        set
        {
            if (_textElement == null)
            {
                _textElement = value;
                if (value != null)
                {
                    InternalRowContainer.Children.Insert(0, value);
                }

                return;
            }

            if (value == null)
            {
                InternalRowContainer.Children.Remove(_textElement);
                _textElement = null;
                return;
            }

            _textElement = value;
            InternalRowContainer.Children[0] = value;
        }
    }

    private Element? _textElement;

    /// <summary>
    /// The actual visual indicator element (commonly a graphical element that looks like a switch). This cannot be
    /// null. It always has to be present in the hierarchy.
    /// </summary>
    public IfElement IndicatorElement
    {
        get => _indicatorElement;
        set
        {
            _indicatorElement = value;

            if (_textElement == null)
            {
                InternalRowContainer.Children[0] = value;
            }
            else
            {
                if (InternalRowContainer.Children.Count == 2)
                {
                    InternalRowContainer.Children[1] = value;
                }
                else
                {
                    InternalRowContainer.Children.Add(value);
                }
            }

            //bind the indicator to the radio button value
            value.Condition.BindBidirectional(ValueProperty);
        }
    }

    private IfElement _indicatorElement = null!;

    /// <summary>
    /// Gives direct access to the button's <see cref="RowContainer"/>, which holds <see cref="TextElement"/> and
    /// <see cref="IndicatorElement"/>. You should generally not modify this and certainly not remove it from the document,
    /// but you have access to it just in case you need it.
    /// </summary>
    /// <remarks>
    /// Modifying properties here directly will not reflect in some properties of the SwitchButton like
    /// <see cref="HorizontalArrangement"/>, that's why you should always use the SwitchButton properties instead of
    /// manually modifying this RowContainer where possible.
    /// </remarks>
    public RowContainer InternalRowContainer { get; private set; }

    private readonly IfElement _defaultIndicatorElement =
        new(
            new ObservableProperty<bool>(false),
            new RoundedRectangleElement(
                new ColorBrush(new Color(0x00_80_ff)),
                new ColorBrush(new Color(0)))
            {
                StyleClass = "SwitchButton::Indicator::Active::Outer",
                RoundCornersDescriptor = new CornerInset(1000),
                Position = new Dimension2(1, 1),
                Layout = new ElementLayout().SetFixedWidth(33).SetFixedHeight(18),
                OutlineParameters = new OutlineParams(2f),
                ClipType = ClipApplicability.HitTesting,
                Children =
                [
                    new EllipseElement(outlineBrush: new ColorBrush(new Color(0xbd_bd_bd)))
                    {
                        StyleClass = "SwitchButton::Indicator::Active::Inner",
                        Position = new Dimension2(20, 5),
                        Layout = new ElementLayout().SetFixedWidth(8).SetFixedHeight(8),
                        OutlineParameters = new OutlineParams(9f),
                        ClipType = ClipApplicability.HitTesting
                    }
                ]
            },
            new RoundedRectangleElement(outlineBrush: new ColorBrush(new Color(0)))
            {
                StyleClass = "SwitchButton::Indicator::Inactive::Outer",
                RoundCornersDescriptor = new CornerInset(1000),
                Position = new Dimension2(1, 1),
                Layout = new ElementLayout().SetFixedWidth(33).SetFixedHeight(18),
                OutlineParameters = new OutlineParams(2f),
                ClipType = ClipApplicability.HitTesting,
                Children =
                [
                    new EllipseElement(outlineBrush: new ColorBrush(new Color(0xbd_bd_bd)))
                    {
                        StyleClass = "SwitchButton::Indicator::Inactive::Inner",
                        Position = new Dimension2(4, 5),
                        Layout = new ElementLayout().SetFixedWidth(8).SetFixedHeight(8),
                        OutlineParameters = new OutlineParams(9f),
                        ClipType = ClipApplicability.HitTesting
                    }
                ]
            }
        ) { Layout = new ElementLayout().SetFixedWidth(35).SetFixedHeight(20) };

    /// <summary>
    /// The base constructor. Will create a new switch button given an Element as <see cref="TextElement"/> and
    /// a generic Element as the <see cref="IndicatorElement"/>. If indicatorElement is not given, it will be a
    /// default element.
    /// </summary>
    /// <param name="initialValue"></param>
    /// <param name="textElement">The value of <see cref="TextElement"/>.</param>
    /// <param name="indicatorElement">
    /// The value of <see cref="IndicatorElement"/>, will be a default element if omitted.
    /// </param>
    public SwitchButton(
        bool initialValue,
        Element textElement,
        IfElement? indicatorElement = null)
    {
        //silence compiler
        InternalRowContainer = null!;

        Init(initialValue, textElement, indicatorElement);
    }

    /// <summary>
    /// Creates a new switch button with <see cref="TextElement"/> as a new <see cref="Label"/> with the given
    /// properties.
    /// </summary>
    /// <param name="initialValue">The initial value of the switch button.</param>
    /// <param name="text">
    /// The text that a <see cref="Label"/> will have when set as the value of <see cref="TextElement"/>.
    /// </param>
    /// <param name="fontSize">The value of <see cref="Text.TextElement.FontSize"/>.</param>
    /// <param name="textBrush">The value of <see cref="Label.TextBrush"/>.</param>
    public SwitchButton(
        bool initialValue,
        string text,
        Dimension? fontSize = null,
        ColorBrush? textBrush = null) :
        this(
            initialValue,
            new Label(text, TextAlignmentType.Center)
            {
                StyleClass = "SwitchButton::TextElement",
                FontSize = fontSize ?? "1em",
                TextBrush = textBrush ?? new ColorBrush(new Color(0)),
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredHeight(0, 0, "100%")
                        .SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                ElementContainerSizing = new RowContainerSizing(1f, VerticalAlignmentType.Center)
            }
        )
    {
    }

    public SwitchButton(SwitchButton other) : base(other)
    {
        //silence compiler
        InternalRowContainer = null!;

        Init(other.Value, other.TextElement?.Duplicate() ?? new Label(), other.IndicatorElement.Duplicate());
        Spacing = other.Spacing;
        HorizontalArrangement = other.HorizontalArrangement;
        VerticalAlignment = other.VerticalAlignment;
    }

    public override SwitchButton Duplicate()
    {
        var el = new SwitchButton(this);
        DuplicateChildrenUtil(el);
        return el;
    }

    private void Init(
        bool initialValue,
        Element textElement,
        IfElement? indicatorElement = null)
    {
        ClickEvent += PrivateOnClick;

        ValueProperty.ValueChangedEvent += SetValue;
        SpacingProperty.ValueChangedEvent += SetSpacing;
        HorizontalArrangementProperty.ValueChangedEvent += SetHorizontalArrangement;
        VerticalAlignmentProperty.ValueChangedEvent += SetVerticalAlignment;

        InternalRowContainer = new RowContainer
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
            Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Center, 0),
            VerticalAlignment = VerticalAlignmentType.Center,
            //we need to have at least one child, as IndicatorElement will access index 0 (this will be replaced
            //when IndicatorElement is set, immediately in this constructor)
            Children = [new Element()]
        };

        InternalRowContainer.VerticalAlignmentProperty.BindBidirectional(VerticalAlignmentProperty);
        Children.Add(InternalRowContainer);

        indicatorElement ??= _defaultIndicatorElement;
        IndicatorElement = indicatorElement;

        TextElement = textElement;
        Value = initialValue;
    }

    private void PrivateOnClick(object sender, ClickEventArgs e)
    {
        Value = !Value;
    }
}
