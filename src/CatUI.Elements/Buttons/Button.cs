using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Elements.Behaviors;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Media;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.Utils;

namespace CatUI.Elements.Buttons
{
    /// <summary>
    /// A UI button that is specialized in having a text, an icon, or both. For a button that has any kind of
    /// content, see <see cref="BaseButton"/>. Do NOT set the <see cref="Element.Children"/> directly or interfere in any
    /// way with the first child, as it is used internally. Modifying it without using <see cref="TextElement"/> and
    /// <see cref="IconElement"/> might result in crashes or unexpected behavior in general.
    /// </summary>
    /// <remarks>
    /// <see cref="Element.Children"/> is controlled internally. You should never set or modify any children directly,
    /// but use convenience properties and methods instead. Direct children modification might cause crashes.
    /// </remarks>
    public class Button : BaseButton, IPaddingAware
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<Button>? Ref
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

        private ObjectRef<Button>? _ref;

        public EdgeInset Padding
        {
            get => _padding;
            set
            {
                SetPadding(value);
                PaddingProperty.Value = value;
            }
        }

        private EdgeInset _padding = new();
        public ObservableProperty<EdgeInset> PaddingProperty { get; private set; } = new(new EdgeInset());

        private void SetPadding(EdgeInset value)
        {
            _padding = value;
        }

        /// <summary>
        /// Represents the spacing between <see cref="TextElement"/> and <see cref="IconElement"/>. This will be considered
        /// only when both a text and an icon are given.
        /// </summary>
        public Dimension Spacing
        {
            get => _spacing;
            set
            {
                SetSpacing(value);
                SpacingProperty.Value = value;
            }
        }

        private Dimension _spacing = new();
        public ObservableProperty<Dimension> SpacingProperty { get; private set; } = new(new Dimension());

        private void SetSpacing(Dimension value)
        {
            _spacing = value;
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
            set
            {
                SetHorizontalArrangement(value);
                HorizontalArrangementProperty.Value = value;
            }
        }

        private LinearArrangement.JustificationType _horizontalArrangement = LinearArrangement.JustificationType.Center;

        public ObservableProperty<LinearArrangement.JustificationType>
            HorizontalArrangementProperty { get; private set; }
            = new(LinearArrangement.JustificationType.Center);

        private void SetHorizontalArrangement(LinearArrangement.JustificationType value)
        {
            _horizontalArrangement = value;
            InternalRowContainer.Arrangement.ContentJustification = value;
        }

        /// <summary>
        /// Represents the vertical alignment of the content. By default, this is <see cref="VerticalAlignmentType.Center"/>.
        /// </summary>
        public VerticalAlignmentType VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                SetVerticalAlignment(value);
                VerticalAlignmentProperty.Value = value;
            }
        }

        private VerticalAlignmentType _verticalAlignment = VerticalAlignmentType.Center;

        public ObservableProperty<VerticalAlignmentType> VerticalAlignmentProperty { get; private set; }
            = new(VerticalAlignmentType.Center);

        private void SetVerticalAlignment(VerticalAlignmentType value)
        {
            _verticalAlignment = value;
            InternalRowContainer.VerticalAlignment = value;
        }

        /// <summary>
        /// Represents the text content of the button, but it's optional, as you can have this, an <see cref="IconElement"/>
        /// or both. Contrary to the name, this can be any kind of element, but it's much more common for it to be a
        /// <see cref="TextBlock"/>.
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
                InternalRowContainer.Children[_iconElement == null ? 0 : 1] = value;
            }
        }

        private Element? _textElement;

        /// <summary>
        /// Represents content of the icon, but it's optional, as you can have this, an <see cref="TextElement"/> or both.
        /// Contrary to the name, this can be any kind of element, but it's generally used as an icon, like a
        /// <see cref="GeometricPathElement"/> or <see cref="ImageView"/>.
        /// </summary>
        /// <remarks>
        /// In order to see when this is modified, assuming you don't interfere with <see cref="InternalRowContainer"/>'s
        /// children, you can listen to <see cref="ObservableList{T}"/> events on <see cref="Element.Children"/> on
        /// <see cref="InternalRowContainer"/>.
        /// </remarks>
        public Element? IconElement
        {
            get => _iconElement;
            set
            {
                if (_iconElement == null)
                {
                    _iconElement = value;
                    if (value != null)
                    {
                        InternalRowContainer.Children.Insert(0, value);
                    }

                    return;
                }

                if (value == null)
                {
                    InternalRowContainer.Children.Remove(_iconElement);
                    _iconElement = null;
                    return;
                }

                _iconElement = value;
                InternalRowContainer.Children[0] = value;
            }
        }

        private Element? _iconElement;

        /// <summary>
        /// Gives direct access to the button's <see cref="PaddingElement"/>. You should generally not modify this
        /// and certainly not remove it from the document, but you have access to it just in case you need it.
        /// </summary>
        public PaddingElement InternalPaddingElement { get; }

        /// <summary>
        /// Gives direct access to the button's <see cref="RowContainer"/>, which holds <see cref="TextElement"/> and
        /// <see cref="IconElement"/>. You should generally not modify this and certainly not remove it from the document,
        /// but you have access to it just in case you need it.
        /// </summary>
        /// <remarks>
        /// Modifying properties from here won't reflect in properties of Button like <see cref="HorizontalArrangement"/>,
        /// that's why you should always modify everything that's possible from Button properties, not from this container
        /// directly.
        /// </remarks>
        public RowContainer InternalRowContainer { get; }

        /// <summary>
        /// A constructor that creates a Button based on the given text element and icon element. Both are optional
        /// because you can have a button with none of them, one of them (either text-only or icon-only) or both
        /// (a text accompanied by an icon).
        /// </summary>
        /// <param name="textElement">The value of <see cref="TextElement"/>.</param>
        /// <param name="iconElement">The value of <see cref="IconElement"/>.</param>
        public Button(Element? textElement = null, Element? iconElement = null)
        {
            _textElement = textElement;
            _iconElement = iconElement;

            InternalRowContainer = new RowContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Center, 0),
                VerticalAlignment = VerticalAlignmentType.Center
            };

            if (_iconElement != null)
            {
                InternalRowContainer.Children.Add(_iconElement);
            }

            if (_textElement != null)
            {
                InternalRowContainer.Children.Add(_textElement);
            }

            InternalPaddingElement = new PaddingElement
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                Children = [InternalRowContainer]
            };
            Children.Add(InternalPaddingElement);

            InternalPaddingElement.PaddingProperty.BindBidirectional(PaddingProperty);

            PaddingProperty.ValueChangedEvent += SetPadding;
            SpacingProperty.ValueChangedEvent += SetSpacing;
            HorizontalArrangementProperty.ValueChangedEvent += SetHorizontalArrangement;
            VerticalAlignmentProperty.ValueChangedEvent += SetVerticalAlignment;
        }

        /// <summary>
        /// A helper constructor that will set the <see cref="TextElement"/> as a default <see cref="TextBlock"/> with
        /// the given text, font size and text brush and will give it a padding.
        /// </summary>
        /// <param name="text">
        /// The text that a <see cref="TextBlock"/> will have when set as the value of <see cref="TextElement"/>.
        /// </param>
        /// <param name="fontSize">The value of <see cref="Text.TextElement.FontSize"/>.</param>
        /// <param name="textBrush">The value of <see cref="TextBlock.TextBrush"/>.</param>
        /// <param name="padding">The value of <see cref="Padding"/>.</param>
        public Button(
            string text,
            Dimension? fontSize = null,
            ColorBrush? textBrush = null,
            EdgeInset? padding = null) :
            this(
                new TextBlock(text, TextAlignmentType.Center)
                {
                    FontSize = fontSize ?? 12,
                    TextBrush = textBrush ?? new ColorBrush(new Color(0)),
                    Layout =
                        new ElementLayout()
                            .SetMinMaxHeight(0, "100%")
                            .SetMinMaxWidth(0, "100%", true),
                    ElementContainerSizing = new RowContainerSizing(1f, VerticalAlignmentType.Center)
                }
            )
        {
            if (padding != null)
            {
                Padding = padding.Value;
            }
        }

        /// <summary>
        /// A helper constructor that will set the <see cref="IconElement"/> as a given <see cref="AbstractShapeElement"/>
        /// and will give it a padding.
        /// </summary>
        /// <param name="shapeElement">
        /// A shape element that will be set as the value of <see cref="IconElement"/>.
        /// </param>
        /// <param name="padding">The value of <see cref="Padding"/>.</param>
        public Button(AbstractShapeElement shapeElement, EdgeInset? padding = null) :
            this(null, shapeElement)
        {
            if (padding != null)
            {
                Padding = padding.Value;
            }
        }

        ~Button()
        {
            PaddingProperty = null!;
            SpacingProperty = null!;
            HorizontalArrangementProperty = null!;
            VerticalAlignmentProperty = null!;
        }

        public override Button Duplicate()
        {
            return new Button(_textElement, _iconElement)
            {
                Padding = Padding,
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
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };
        }
    }
}
