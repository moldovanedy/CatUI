using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Data.Shapes;
using CatUI.Elements.Behaviors;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Utils;

namespace CatUI.Elements.Buttons
{
    public class CheckBox : BaseButton, IToggleable
    {
        /// <summary>
        /// The checkbox is in this state when <see cref="Value"/> is <see cref="CheckBoxState.Indeterminate"/>.
        /// </summary>
        public const string STATE_CHECKED_INDETERMINATE = "checked-indeterminate";

        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<CheckBox>? Ref
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

        private ObjectRef<CheckBox>? _ref;

        public CheckBoxState Value
        {
            get => _value;
            set => ValueProperty.Value = value;
        }

        private CheckBoxState _value = CheckBoxState.Unchecked;

        public ObservableProperty<CheckBoxState> ValueProperty { get; } = new(CheckBoxState.Unchecked);

        private void SetValue(CheckBoxState value)
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
        /// Represents the text content of the checkbox. Contrary to the name, this can be any kind of element, but
        /// it's much more common for it to be a <see cref="TextBlock"/>.
        /// </summary>
        /// <remarks>
        /// In order to see when this is modified, assuming you don't interfere with <see cref="InternalRowContainer"/>'s
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
                        InternalRowContainer.Children.Add(value);
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
                InternalRowContainer.Children[1] = value;
            }
        }

        private Element? _textElement;

        /// <summary>
        /// The actual visual indicator element (commonly a box with a check mark inside it). This cannot be null.
        /// It always has to be present in the hierarchy.
        /// </summary>
        public TriStateCheckBoxIndicator IndicatorElement
        {
            get => _indicatorElement;
            set
            {
                _indicatorElement = value;
                InternalRowContainer.Children[0] = value;
                //bind the indicator to the checkbox value
                value.ValueProperty.BindBidirectional(ValueProperty);
            }
        }

        private TriStateCheckBoxIndicator _indicatorElement = null!;

        /// <summary>
        /// Gives direct access to the button's <see cref="RowContainer"/>, which holds <see cref="TextElement"/> and
        /// <see cref="IndicatorElement"/>. You should generally not modify this and certainly not remove it from the document,
        /// but you have access to it just in case you need it.
        /// </summary>
        /// <remarks>
        /// Modifying properties here directly will not reflect in some properties of the CheckBox like
        /// <see cref="HorizontalArrangement"/>, that's why you should always use the CheckBox properties instead of
        /// manually modifying this RowContainer where possible.
        /// </remarks>
        public RowContainer InternalRowContainer { get; }

        private readonly TriStateCheckBoxIndicator _defaultIndicatorElement =
            new(
                CheckBoxState.Unchecked,
                new RectangleElement(outlineBrush: new ColorBrush(new Color(0)))
                {
                    StyleClass = "CheckBox::Indicator::Checked::Outer",
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                    OutlineParameters = new OutlineParams(2f),
                    Children =
                    [
                        new RectangleElement(new ColorBrush(new Color(0x00_80_ff)))
                        {
                            StyleClass = "CheckBox::Indicator::Checked::Inner",
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                            Children =
                            [
                                new GeometricPathElement(
                                    //from CatUI/IndirectAssets/checkmark.svg
                                    "m 9.118746,16.621968 c -0.892928,0.892928 -2.341566,0.892713 -3.2342794,0 C 4.2141044,14.951606 2.5437422,13.281244 0.87338,11.610881 c -0.8927129,-0.892713 -0.8927129,-2.341136 0,-3.2338485 0.892713,-0.8927129 2.3412869,-0.892864 3.2342797,0 0.9952072,0.9950635 1.9904144,1.9901275 2.9856213,2.9851905 0.225188,0.225156 0.591694,0.225387 0.817082,0 2.69473,-2.6947302 5.38946,-5.3894605 8.084191,-8.0841909 0.892712,-0.8927129 2.341351,-0.8929284 3.234279,0 0.428796,0.4287953 0.669697,1.0105778 0.669697,1.6169244 0,0.6063467 -0.240901,1.1881292 -0.669697,1.6169244 C 15.858804,9.88191 12.488775,13.251939 9.118746,16.621968 Z",
                                    new ColorBrush(new Color(0xff_ff_ff)))
                                {
                                    StyleClass = "CheckBox::Indicator::Checked::Graphic",
                                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                                }
                            ]
                        }
                    ]
                },
                new RectangleElement(outlineBrush: new ColorBrush(new Color(0)))
                {
                    StyleClass = "CheckBox::Indicator::Unchecked::Outer",
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                    OutlineParameters = new OutlineParams(2f)
                },
                new RectangleElement(outlineBrush: new ColorBrush(new Color(0)))
                {
                    StyleClass = "CheckBox::Indicator::Indeterminate::Outer",
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                    OutlineParameters = new OutlineParams(2f),
                    Children =
                    [
                        new RectangleElement(new ColorBrush(new Color(0x00_80_ff)))
                        {
                            StyleClass = "CheckBox::Indicator::Indeterminate::Inner",
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                            Children =
                            [
                                new GeometricPathElement(
                                    //from CatUI/IndirectAssets/radio-button.svg
                                    "m 5,7 h 10 c 1.662,0 3,1.338 3,3 0,1.662 -1.338,3 -3,3 H 5 C 3.338,13 2,11.662 2,10 2,8.338 3.338,7 5,7 Z",
                                    new ColorBrush(new Color(0xff_ff_ff)))
                                {
                                    StyleClass = "CheckBox::Indicator::Indeterminate::Graphic",
                                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                                }
                            ]
                        }
                    ]
                }
            ) { Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20) };

        /// <summary>
        /// The base constructor. Will create a new checkbox given an Element as <see cref="TextElement"/> and
        /// a generic Element as the <see cref="IndicatorElement"/>. If indicatorElement is not given, it will be a
        /// default element.
        /// </summary>
        /// <param name="initialValue"></param>
        /// <param name="textElement">The value of <see cref="TextElement"/>.</param>
        /// <param name="indicatorElement">
        /// The value of <see cref="IndicatorElement"/>, will be a default element if omitted.
        /// </param>
        public CheckBox(
            CheckBoxState initialValue,
            Element textElement,
            TriStateCheckBoxIndicator? indicatorElement = null)
        {
            OnClick += PrivateOnClick;

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
                //when IndicatorElement are set, immediately in this constructor)
                Children = [new Element()]
            };

            InternalRowContainer.VerticalAlignmentProperty.BindBidirectional(VerticalAlignmentProperty);
            Children.Add(InternalRowContainer);

            indicatorElement ??= _defaultIndicatorElement;
            IndicatorElement = indicatorElement;

            TextElement = textElement;
            Value = initialValue;
        }

        /// <summary>
        /// Creates a new checkbox with <see cref="TextElement"/> as a new <see cref="TextBlock"/> with the given
        /// properties.
        /// </summary>
        /// <param name="initialValue">The initial value of the checkbox.</param>
        /// <param name="text">
        /// The text that a <see cref="TextBlock"/> will have when set as the value of <see cref="TextElement"/>.
        /// </param>
        /// <param name="fontSize">The value of <see cref="Text.TextElement.FontSize"/>.</param>
        /// <param name="textBrush">The value of <see cref="TextBlock.TextBrush"/>.</param>
        public CheckBox(
            CheckBoxState initialValue,
            string text,
            Dimension? fontSize = null,
            ColorBrush? textBrush = null) :
            this(
                initialValue,
                new TextBlock(text, TextAlignmentType.Center)
                {
                    StyleClass = "CheckBox::TextElement",
                    FontSize = fontSize ?? "1em",
                    TextBrush = textBrush ?? new ColorBrush(new Color(0)),
                    Layout =
                        new ElementLayout()
                            .SetMinMaxHeight(0, "100%")
                            .SetMinMaxWidth(0, "100%", true),
                    ElementContainerSizing = new RowContainerSizing(1f, VerticalAlignmentType.Center)
                }
            )
        {
        }

        public override CheckBox Duplicate()
        {
            CheckBox el = new(Value, _textElement!, _indicatorElement)
            {
                Spacing = Spacing,
                HorizontalArrangement = HorizontalArrangement,
                VerticalAlignment = VerticalAlignment,
                //BaseButton
                CanUserCancelClick = CanUserCancelClick,
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                LocallyVisible = LocallyVisible,
                LocallyEnabled = LocallyEnabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }

        private void PrivateOnClick(object sender, ClickEventArgs e)
        {
            if (Value == CheckBoxState.Unchecked || Value == CheckBoxState.Indeterminate)
            {
                Value = CheckBoxState.Checked;
            }
            else
            {
                Value = CheckBoxState.Unchecked;
            }
        }


        public enum CheckBoxState
        {
            Unchecked = 0,
            Checked = 1,
            Indeterminate = 2
        }
    }
}
