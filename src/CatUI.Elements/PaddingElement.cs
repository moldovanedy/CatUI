using System;
using System.Numerics;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Utils;

namespace CatUI.Elements
{
    /// <summary>
    /// Represents an element that always spans the entire width and height of the parent minus the padding values
    /// from top, right, bottom and left. Properties referring to position, width or height are ignored.
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
                _padding = value;
                PaddingProperty.Value = value;
            }
        }

        private EdgeInset _padding = new();
        public ObservableProperty<EdgeInset> PaddingProperty { get; private set; } = new(new EdgeInset());


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

        private void SetPadding(EdgeInset value)
        {
            _padding = value;
        }

        protected override void RecalculateLayout()
        {
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
                parentWidth = GetParent()?.Bounds.BoundingRect.Width ?? 0;
                parentHeight = GetParent()?.Bounds.BoundingRect.Height ?? 0;
                parentXPos = GetParent()?.Bounds.BoundingRect.X ?? 0;
                parentYPos = GetParent()?.Bounds.BoundingRect.Y ?? 0;
            }

            float x = parentXPos + Math.Min(parentWidth / 2f, CalculateDimension(_padding.Left, parentWidth));
            float y = parentYPos + Math.Min(parentHeight / 2f, CalculateDimension(_padding.Top, parentHeight));
            float width = parentWidth -
                          Math.Min(parentWidth / 2f, CalculateDimension(_padding.Right, parentWidth));
            float height = parentHeight -
                           Math.Min(parentHeight / 2f, CalculateDimension(_padding.Bottom, parentHeight));

            Bounds = new ElementBounds(
                new Rect(x, y, width, height),
                new Vector4());
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

            Bounds = new ElementBounds(
                new Rect(x, y, thisSize.Width, thisSize.Height),
                new Vector4());
            return thisSize;
        }

        public override PaddingElement Duplicate()
        {
            return new PaddingElement
            {
                Padding = _padding,
                //
                Position = Position,
                PreferredWidth = PreferredWidth,
                PreferredHeight = PreferredHeight,
                MinWidth = MinWidth,
                MinHeight = MinHeight,
                MaxWidth = MaxWidth,
                MaxHeight = MaxHeight,
                Margin = Margin,
                Background = Background.Duplicate(),
                CornerRadius = CornerRadius,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate()
            };
        }
    }
}
