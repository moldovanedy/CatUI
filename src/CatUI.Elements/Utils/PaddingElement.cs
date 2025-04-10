using System;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Utils
{
    /// <summary>
    /// Represents an element that always spans the entire width and height of the parent minus the padding values
    /// from top, right, bottom and left. Properties referring to position, width or height are ignored. Although its name
    /// refers to padding, it can also be used as margin.
    /// </summary>
    /// <remarks>
    /// The padding cannot be larger than the half of the width for left and right and larger than height for top and bottom.
    /// Negative values are allowed, but will cause the element to exceed its parent's size. Use with caution.
    /// </remarks>
    public class PaddingElement : Element
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<PaddingElement>? Ref
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

        private ObjectRef<PaddingElement>? _ref;

        /// <summary>
        /// Represents the padding values. The default value is a default <see cref="EdgeInset"/> (0 padding on all sides).
        /// </summary>
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

        public PaddingElement()
        {
            PaddingProperty.ValueChangedEvent += SetPadding;
        }

        public PaddingElement(EdgeInset padding)
        {
            Padding = padding;
            PaddingProperty.ValueChangedEvent += SetPadding;
        }

        ~PaddingElement()
        {
            PaddingProperty = null!;
        }

        public override Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            Size? parentEnforcedSize = null)
        {
            float x =
                parentAbsolutePosition.X +
                Math.Min(parentSize.Width / 2f, CalculateDimension(_padding.Left, parentSize.Width));
            float y =
                parentAbsolutePosition.Y +
                Math.Min(parentSize.Height / 2f, CalculateDimension(_padding.Top, parentSize.Height));

            Size thisSize;
            if (parentEnforcedSize == null)
            {
                float width = parentSize.Width -
                              Math.Min(parentSize.Width / 2f, CalculateDimension(_padding.Right, parentSize.Width));
                float height = parentSize.Height -
                               Math.Min(parentSize.Height / 2f, CalculateDimension(_padding.Bottom, parentSize.Height));
                thisSize = new Size(width, height);
            }
            else
            {
                thisSize = parentEnforcedSize.Value;
            }

            Point2D thisAbsolutePosition = new(x, y);
            RecomputeChildrenUtil(thisSize, thisSize, thisAbsolutePosition);

            Bounds = GetFinalBoundsUtil(thisAbsolutePosition, thisSize);
            return thisSize;
        }

        public override PaddingElement Duplicate()
        {
            return new PaddingElement
            {
                Padding = _padding,
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
