using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Elements.Containers.Linear;
using CatUI.Utils;

namespace CatUI.Elements.Utils
{
    /// <inheritdoc cref="Divider"/>
    /// <remarks>
    /// This will display a vertical line, so it's generally used inside layouts that work like a row
    /// (like <see cref="RowContainer"/>).
    /// </remarks>
    public class VerticalDivider : Divider
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<VerticalDivider>? Ref
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

        private ObjectRef<VerticalDivider>? _ref;

        /// <summary>
        /// Represents the spacing between the line and the element on the left. The value is in <see cref="Unit.Dp"/>.
        /// The default value is 0.
        /// </summary>
        public float LeftSpacing
        {
            get => Spacing.Item1;
            set
            {
                SetLeftSpacing(value);
                LeftSpacingProperty.Value = value;
            }
        }

        public ObservableProperty<float> LeftSpacingProperty { get; private set; } = new(0);

        private void SetLeftSpacing(float value)
        {
            //will mark the layout dirty automatically
            Spacing = (value, Spacing.Item2);
        }

        /// <summary>
        /// Represents the spacing between the line and the element on the right. The value is in <see cref="Unit.Dp"/>.
        /// The default value is 0.
        /// </summary>
        public float RightSpacing
        {
            get => Spacing.Item2;
            set
            {
                SetRightSpacing(value);
                RightSpacingProperty.Value = value;
            }
        }

        public ObservableProperty<float> RightSpacingProperty { get; private set; } = new(0);

        private void SetRightSpacing(float value)
        {
            //will mark the layout dirty automatically
            Spacing = (Spacing.Item1, value);
        }

        /// <summary>
        /// Represents the padding (or space) between the top end of the line (line cap) and the element bounds.
        /// The percentage will represent the height of the element. The default value is 0.
        /// </summary>
        public Dimension TopLinePadding
        {
            get => LinePadding.Item1;
            set
            {
                SetTopLinePadding(value);
                TopLinePaddingProperty.Value = value;
            }
        }

        public ObservableProperty<Dimension> TopLinePaddingProperty { get; private set; } = new(0);

        private void SetTopLinePadding(Dimension value)
        {
            //will mark the document dirty automatically
            LinePadding = (value, LinePadding.Item2);
        }

        /// <summary>
        /// Represents the padding (or space) between the bottom end of the line (line cap) and the element bounds.
        /// The percentage will represent the height of the element. The default value is 0.
        /// </summary>
        public Dimension BottomLinePadding
        {
            get => LinePadding.Item1;
            set
            {
                SetBottomLinePadding(value);
                BottomLinePaddingProperty.Value = value;
            }
        }

        public ObservableProperty<Dimension> BottomLinePaddingProperty { get; private set; } = new(0);

        private void SetBottomLinePadding(Dimension value)
        {
            //will mark the document dirty automatically
            LinePadding = (LinePadding.Item1, value);
        }

        public VerticalDivider(float thickness = 2, IBrush? brush = null) : base(Orientation.Vertical)
        {
            LeftSpacingProperty.ValueChangedEvent += SetLeftSpacing;
            RightSpacingProperty.ValueChangedEvent += SetRightSpacing;
            TopLinePaddingProperty.ValueChangedEvent += SetTopLinePadding;
            BottomLinePaddingProperty.ValueChangedEvent += SetBottomLinePadding;

            LineThickness = thickness;
            if (brush != null)
            {
                LineBrush = brush;
            }
        }

        ~VerticalDivider()
        {
            LeftSpacingProperty = null!;
            RightSpacingProperty = null!;
            TopLinePaddingProperty = null!;
            BottomLinePaddingProperty = null!;
        }

        public override VerticalDivider Duplicate()
        {
            return new VerticalDivider
            {
                LeftSpacing = LeftSpacing,
                RightSpacing = RightSpacing,
                TopLinePadding = TopLinePadding,
                BottomLinePadding = BottomLinePadding,
                //Divider
                LineOrientation = LineOrientation,
                LineThickness = LineThickness,
                LineBrush = LineBrush,
                LineCap = LineCap,
                //
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
