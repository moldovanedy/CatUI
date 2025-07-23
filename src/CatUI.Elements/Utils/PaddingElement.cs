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
    /// <para>
    /// The padding cannot be larger than the half of the width for left and right and larger than height for top and bottom.
    /// Negative values are allowed, but will cause the element to exceed its parent's size. Use with caution.
    /// </para>
    /// <para>
    /// If the content is larger than this element, it will try to expand (if the maximum size allows it to) so that
    /// the padding values are still respected. If it cannot expand, then the padding values for top and left might
    /// not be correct as they can't be respected.
    /// </para>
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
            set => PaddingProperty.Value = value;
        }

        private EdgeInset _padding = new();
        public ObservableProperty<EdgeInset> PaddingProperty { get; } = new(new EdgeInset());

        private void SetPadding(EdgeInset value)
        {
            _padding = value;
            SetLocalValue(nameof(Padding), value);
        }

        public PaddingElement()
        {
            PaddingProperty.ValueChangedEvent += SetPadding;
        }

        public PaddingElement(EdgeInset padding)
        {
            PaddingProperty.ValueChangedEvent += SetPadding;
            Padding = padding;
        }

        public override Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            float? parentEnforcedWidth = null,
            float? parentEnforcedHeight = null)
        {
            float pLeft = CalculateDimension(_padding.Left, parentSize.Width);
            float pTop = CalculateDimension(_padding.Top, parentSize.Height);
            float pRight = CalculateDimension(_padding.Right, parentSize.Width);
            float pBottom = CalculateDimension(_padding.Bottom, parentSize.Height);

            float x = parentAbsolutePosition.X + Math.Min(parentSize.Width / 2f, pLeft);
            float y = parentAbsolutePosition.Y + Math.Min(parentSize.Height / 2f, pTop);

            float width =
                parentEnforcedWidth != null
                    ? parentEnforcedWidth.Value - pLeft - pRight
                    : parentSize.Width - pLeft - Math.Min(parentSize.Width / 2f, pRight);
            float height =
                parentEnforcedHeight != null
                    ? parentEnforcedHeight.Value - pTop - pBottom
                    : parentSize.Height - pTop - Math.Min(parentSize.Height / 2f, pBottom);

            Size thisSize = new(Math.Max(0, width), Math.Max(0, height));

            Point2D thisAbsolutePosition = new(x, y);
            RecomputeChildrenUtil(thisSize, thisSize, thisAbsolutePosition);

            Rect contentBounds = GetFinalBoundsUtil(thisAbsolutePosition, thisSize);
            Point2D offset = Point2D.Zero;

            //after all children are computed, see if the given size and the actual size match;
            //if not and there is enough space to grow, recalculate the content position, then update the position
            //of each child directly using UpdatePositionOfChildren
            if (contentBounds.Width > thisSize.Width)
            {
                float actualPaddingOnLeft = Math.Min(parentSize.Width / 2f, pLeft);
                //float actualPaddingOnRight = Math.Min(parentSize.Width / 2f, pRight);

                float maxStretchAllowed = parentMaxSize.Width - thisSize.Width;
                if (pLeft + pRight <= maxStretchAllowed)
                {
                    float diff = pLeft - actualPaddingOnLeft;
                    x += diff;
                    offset = new Point2D(diff, 0);
                }
            }

            if (contentBounds.Height > thisSize.Height)
            {
                float actualPaddingOnTop = Math.Min(parentSize.Height / 2f, pTop);
                //float actualPaddingOnBottom = Math.Min(parentSize.Height / 2f, pBottom);

                float maxStretchAllowed = parentMaxSize.Height - thisSize.Height;
                if (pTop + pBottom <= maxStretchAllowed)
                {
                    float diff = pTop - actualPaddingOnTop;
                    y += diff;
                    offset = new Point2D(offset.X, diff);
                }
            }

            if (offset.X != 0 || offset.Y != 0)
            {
                UpdatePositionOfChildren(this, offset);
            }

            Bounds = new Rect(x, y, contentBounds.Width, contentBounds.Height);
            return thisSize;
        }

        /*
        protected override void OnChildLayoutChanged(object? sender, ChildLayoutChangedEventArgs e)
        {
            if (IsChildOfContainer)
            {
                MarkLayoutDirty();
                return;
            }

            GetParentLayoutMetrics(out Size parentSize, out Size parentMaxSize, out Point2D parentAbsolutePosition);

            float pLeft = CalculateDimension(_padding.Left, parentSize.Width);
            float pTop = CalculateDimension(_padding.Top, parentSize.Height);
            float pRight = CalculateDimension(_padding.Right, parentSize.Width);
            float pBottom = CalculateDimension(_padding.Bottom, parentSize.Height);

            float x = parentAbsolutePosition.X + Math.Min(parentSize.Width / 2f, pLeft);
            float y = parentAbsolutePosition.Y + Math.Min(parentSize.Height / 2f, pTop);
            // float width = parentSize.Width - pLeft - Math.Min(parentSize.Width / 2f, pRight);
            // float height = parentSize.Height - pTop - Math.Min(parentSize.Height / 2f, pBottom);

            //Point2D absolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
            Size thisMaxSize = GetMaxSizeUtil(parentSize);

            Point2D absolutePosition = new(x, y);
            thisSize = new Size(
                Math.Max(thisSize.Width - pLeft - Math.Min(parentSize.Width / 2f, pRight), 0),
                Math.Max(thisSize.Height - pTop - Math.Min(parentSize.Height / 2f, pBottom), 0));
            thisMaxSize = new Size(
                Math.Max(thisMaxSize.Width - pLeft - Math.Min(parentSize.Width / 2f, pRight), 0),
                Math.Max(thisMaxSize.Height - pTop - Math.Min(parentSize.Height / 2f, pBottom), 0));

            if (e.ChildIndex >= Children.Count)
            {
                return;
            }

            Children[e.ChildIndex].RecomputeLayout(thisSize, thisMaxSize, absolutePosition);
            Rect previousBounds = Bounds;
            Bounds = Rect.GetCommonBoundingRect(Bounds, Children[e.ChildIndex].Bounds);

            if (Math.Abs(previousBounds.X - Bounds.X) > 0.01 ||
                Math.Abs(previousBounds.Y - Bounds.Y) > 0.01 ||
                Math.Abs(previousBounds.Width - Bounds.Width) > 0.01 ||
                Math.Abs(previousBounds.Height - Bounds.Height) > 0.01)
            {
                MarkLayoutDirty();
            }
        }
        */

        public override PaddingElement Duplicate()
        {
            PaddingElement el = new()
            {
                Padding = _padding,
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

        private static void UpdatePositionOfChildren(Element element, Point2D constOffset)
        {
            foreach (Element child in element.Children)
            {
                child.Bounds = new Rect(
                    child.Bounds.X + constOffset.X,
                    child.Bounds.Y + constOffset.Y,
                    child.Bounds.Width,
                    child.Bounds.Height);
                UpdatePositionOfChildren(child, constOffset);
            }
        }
    }
}
