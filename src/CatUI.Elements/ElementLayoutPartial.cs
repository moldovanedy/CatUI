using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CatUI.Data;
using CatUI.Data.ElementData;
using CatUI.Data.Events.Document;
using Size = CatUI.Data.Size;

namespace CatUI.Elements
{
    public partial class Element
    {
        public ElementLayout Layout
        {
            get => _layout;
            set
            {
                _layout = value;
                LayoutProperty.Value = value;
            }
        }

        private ElementLayout _layout = new();
        public ObservableProperty<ElementLayout> LayoutProperty { get; private set; } = new(new ElementLayout());

        public event ChildLayoutChangedEventHandler? ChildLayoutChangedEvent;

        protected virtual void OnChildLayoutChanged(object? sender, ChildLayoutChangedEventArgs e)
        {
            // Rect parentBounds = _parent?.Bounds ?? Bounds;
            // Point2D parentAbsolutePosition = new(parentBounds.X, parentBounds.Y);
            // Size parentSize = new(parentBounds.Width, parentBounds.Height);
            //

            GetParentLayoutMetrics(out Size parentSize, out Size parentMaxSize, out Point2D parentAbsolutePosition);
            Point2D absolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
            Size thisMaxSize = GetMaxSizeUtil(parentSize);

            if (e.ChildIndex >= _children.Count)
            {
                return;
            }

            _children[e.ChildIndex].RecomputeLayout(thisSize, thisMaxSize, absolutePosition);
        }

        /// <summary>
        /// Returns the parent's layout by calculating the metrics from the root element to this element's parent.
        /// This is a pretty expensive call, so use with caution.
        /// </summary>
        /// <param name="parentSize">The parent's direct size.</param>
        /// <param name="parentMaxSize">The parent's max size.</param>
        /// <param name="parentAbsolutePosition">The parent's absolute position in the document.</param>
        protected void GetParentLayoutMetrics(
            out Size parentSize,
            out Size parentMaxSize,
            out Point2D parentAbsolutePosition)
        {
            Element currentElement = this;
            List<int> childIndices = [];
            while (currentElement._parent != null)
            {
                currentElement = currentElement._parent;
                childIndices.Add(currentElement.IndexInParent);
            }

            //the root will always have its max size the same as its bounds 
            parentMaxSize = new Size(currentElement.Bounds.Width, currentElement.Bounds.Height);
            parentSize = parentMaxSize;
            parentAbsolutePosition = new Point2D();
            //from the direct child of root to this element's parent
            for (int i = childIndices.Count - 2; i >= 0; i--)
            {
                Element next = currentElement._children[childIndices[i]];
                parentMaxSize = next.GetMaxSizeUtil(parentMaxSize);
                parentSize = next.GetDirectSizeUtil(parentSize, parentMaxSize);
                parentAbsolutePosition = next.GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);

                currentElement = next;
            }
        }

        /// <summary>
        /// The most important function for the layout system. You should generally not override this and instead use
        /// already existing elements to achieve the desired result. If you however do override this, see the remarks for
        /// more info.
        /// </summary>
        /// <remarks>
        /// This method is responsible for determining the preferred size of this element (and return it), while
        /// also calling this method recursively to all the children and making sure to give children the correct data.
        /// Before returning, you MUST set <see cref="Bounds"/> to in absolute coordinates (relative only to the viewport).
        /// You can call this only on children that are visible in the viewport or are important to the general layout
        /// of this element for more efficiency.
        /// </remarks>
        /// <param name="parentSize">
        /// The parent's preferred size. For children, you should pass your element's preferred size.
        /// </param>
        /// <param name="parentMaxSize">
        /// The parent's max size. Your element should NOT exceed this size. Warning! This size did not subtract your
        /// element's position, so you MUST take into consideration your position relative to the parent. When you call
        /// this on children, you MUST NOT subtract their relative position from this size.
        /// </param>
        /// <param name="parentAbsolutePosition">
        /// The parent's absolute position (relative to the viewport). For children, you should pass your element's
        /// absolute position. Warning! Inside containers, this value is generally (0, 0) because the container is
        /// responsible for placing the children.
        /// </param>
        /// <param name="parentEnforcedSize">
        /// An optional parameter that is generally given inside containers. It represents the parent's forced
        /// size for this element. You MUST respect this and not give your own value, as the container is free to
        /// assume that the given value was set or even set it manually, in which case not obeying will create a broken
        /// UI.
        /// </param>
        /// <returns>
        /// The preferred size of this element. This might NOT be your final size, especially inside containers.
        /// </returns>
        public virtual Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            Size? parentEnforcedSize = null)
        {
            Point2D absolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisSize, thisMaxSize;

            if (parentEnforcedSize == null)
            {
                thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
                thisMaxSize = GetMaxSizeUtil(parentSize);
            }
            else
            {
                thisSize = parentEnforcedSize.Value;
                thisMaxSize = parentEnforcedSize.Value;
            }

            RecomputeChildrenUtil(thisSize, thisMaxSize, absolutePosition);
            Bounds = new Rect(absolutePosition, thisSize);

            return thisSize;
        }

        /// <summary>
        /// Utility that can be used in overrides of <see cref="RecomputeLayout"/> that determines the element's
        /// preferred size as stated by <see cref="Layout"/>. You must supply both the parent's preferred size and
        /// its max size. The preferred size will be taken into account for everything, unless it exceeds the
        /// parent's max size constraint.
        /// </summary>
        /// <param name="parentPreferredSize">The parent's preferred size (this is NOT necessarily its final size).</param>
        /// <param name="parentMaxSize">
        /// The parent's maximum allowed size. You MUST take into account your current position.
        /// </param>
        /// <returns>The element's preferred size, determined by <see cref="Layout"/>.</returns>
        protected Size GetDirectSizeUtil(Size parentPreferredSize, Size parentMaxSize)
        {
            Dimension? abstractWidth = _layout.GetSuggestedWidth();
            Dimension? abstractHeight = _layout.GetSuggestedHeight();

            float width;
            Dimension minWidth = _layout.MinWidth ?? Dimension.Unset;
            Dimension maxWidth = _layout.MaxWidth ?? Dimension.Unset;
            Dimension setWidth = abstractWidth ?? 0;

            Size maxAllowedSize = new(
                parentMaxSize.Width - CalculateDimension(Position.X, parentPreferredSize.Width),
                parentMaxSize.Height - CalculateDimension(Position.Y, parentPreferredSize.Height));

            switch (_layout.WidthMode)
            {
                default:
                case ElementLayout.LayoutMode.Fixed:
                    width = CalculateDimension(setWidth, parentPreferredSize.Width);
                    break;
                case ElementLayout.LayoutMode.MinMax:
                    if (_layout.PrefersMaxWidth)
                    {
                        //ensure max is not smaller than min
                        width = Math.Max(
                            minWidth.IsUnset()
                                ? float.MinValue
                                : CalculateDimension(minWidth, parentPreferredSize.Width),
                            //ensure the max width is not larger than the parent max width
                            Math.Min(
                                CalculateDimension(setWidth, parentPreferredSize.Width),
                                maxAllowedSize.Width));
                    }
                    else
                    {
                        //ensure min is not larger than max
                        width = Math.Min(
                            CalculateDimension(setWidth, parentPreferredSize.Width),
                            maxWidth.IsUnset()
                                ? float.MaxValue
                                : CalculateDimension(maxWidth, parentPreferredSize.Width));
                    }

                    break;
                case ElementLayout.LayoutMode.MinMaxAndPreferred:
                    float min =
                        minWidth.IsUnset()
                            ? float.MinValue
                            : CalculateDimension(minWidth, parentPreferredSize.Width);

                    float max;
                    if (maxWidth.IsUnset())
                    {
                        max = float.MaxValue;
                    }
                    else
                    {
                        //ensure the max width is not larger than the parent max width
                        max =
                            Math.Min(
                                CalculateDimension(maxWidth, parentPreferredSize.Width),
                                maxAllowedSize.Width);
                    }

                    width = Math.Clamp(
                        CalculateDimension(_layout.PreferredWidth ?? 0, parentPreferredSize.Width), min, max);
                    break;
            }

            float height;
            Dimension minHeight = _layout.MinHeight ?? Dimension.Unset;
            Dimension maxHeight = _layout.MaxHeight ?? Dimension.Unset;
            Dimension setHeight = abstractHeight ?? 0;

            switch (_layout.HeightMode)
            {
                default:
                case ElementLayout.LayoutMode.Fixed:
                    height = CalculateDimension(setHeight, parentPreferredSize.Height);
                    break;
                case ElementLayout.LayoutMode.MinMax:
                    if (_layout.PrefersMaxHeight)
                    {
                        //ensure max is not smaller than min
                        height = Math.Max(
                            minHeight.IsUnset()
                                ? float.MinValue
                                : CalculateDimension(minHeight, parentPreferredSize.Height),
                            //ensure the max height is not larger than the parent max height
                            Math.Min(
                                CalculateDimension(setHeight, parentPreferredSize.Height),
                                maxAllowedSize.Height));
                    }
                    else
                    {
                        //ensure min is not larger than max
                        height = Math.Min(
                            CalculateDimension(setHeight, parentPreferredSize.Height),
                            maxHeight.IsUnset()
                                ? float.MaxValue
                                : CalculateDimension(maxHeight, parentPreferredSize.Height));
                    }

                    break;
                case ElementLayout.LayoutMode.MinMaxAndPreferred:
                    float min =
                        minHeight.IsUnset()
                            ? float.MinValue
                            : CalculateDimension(minHeight, parentPreferredSize.Height);

                    float max;
                    if (maxHeight.IsUnset())
                    {
                        max = float.MaxValue;
                    }
                    else
                    {
                        //ensure the max height is not larger than the parent max height
                        max =
                            Math.Min(
                                CalculateDimension(maxHeight, parentPreferredSize.Height),
                                maxAllowedSize.Height);
                    }

                    height = Math.Clamp(
                        CalculateDimension(_layout.PreferredHeight ?? 0, parentPreferredSize.Height), min, max);
                    break;
            }

            return new Size(width, height);
        }

        /// <summary>
        /// Utility that can be used in overrides of <see cref="RecomputeLayout"/> that determines the element's
        /// absolute position base on the parent's position. Keep in mind that the returned value is absolute to the
        /// viewport, not relative to an element.
        /// </summary>
        /// <param name="parentAbsolutePosition">The absolute coordinates of the top-left corner.</param>
        /// <param name="parentSize">The preferred size of the parent.</param>
        /// <returns>The absolute coordinates of the top-left corner of this element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Point2D GetAbsolutePositionUtil(Point2D parentAbsolutePosition, Size parentSize)
        {
            Point2D absolutePosition;
            if (!Position.IsUnset())
            {
                absolutePosition = new Point2D(
                    parentAbsolutePosition.X + CalculateDimension(Position.X, parentSize.Width),
                    parentAbsolutePosition.Y + CalculateDimension(Position.Y, parentSize.Height));
            }
            else
            {
                absolutePosition = new Point2D(parentAbsolutePosition.X, parentAbsolutePosition.Y);
            }

            return absolutePosition;
        }

        /// <summary>
        /// Gets the maximum allowed size of this element (in regard to <see cref="Layout"/>). If the
        /// <see cref="ElementLayout.LayoutMode"/> is <see cref="ElementLayout.LayoutMode.Fixed"/>, the preferred width
        /// will be returned, otherwise the set maximum width or height will be considered, but if those are not set,
        /// <see cref="float.PositiveInfinity"/> will be returned for that size.
        /// </summary>
        /// <param name="parentSize">The parent's preferred size.</param>
        /// <returns>The maximum allowed size of this element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Size GetMaxSizeUtil(Size parentSize)
        {
            float maxWidth, maxHeight;

            if (_layout.WidthMode == ElementLayout.LayoutMode.Fixed)
            {
                maxWidth = CalculateDimension(_layout.PreferredWidth ?? Dimension.Unset, parentSize.Width);
            }
            else
            {
                maxWidth =
                    (_layout.MaxWidth ?? Dimension.Unset).IsUnset()
                        ? float.PositiveInfinity
                        : CalculateDimension(_layout.MaxWidth!.Value, parentSize.Width);
            }

            if (_layout.HeightMode == ElementLayout.LayoutMode.Fixed)
            {
                maxHeight = CalculateDimension(_layout.PreferredHeight ?? Dimension.Unset, parentSize.Height);
            }
            else
            {
                maxHeight =
                    (_layout.MaxHeight ?? Dimension.Unset).IsUnset()
                        ? float.MaxValue
                        : CalculateDimension(_layout.MaxHeight!.Value, parentSize.Height);
            }

            return new Size(maxWidth, maxHeight);
        }

        /// <summary>
        /// Utility that should only be used in overrides of <see cref="RecomputeLayout"/>. It recomputes the layout
        /// of all the children (by calling <see cref="RecomputeLayout"/>) so you don't have to create the same
        /// functionality again. Only call this inside <see cref="RecomputeLayout"/>!
        /// </summary>
        /// <param name="currentPrefSize">The current preferred size (you can use <see cref="GetDirectSizeUtil"/>).</param>
        /// <param name="currentMaxSize">
        /// The element's max allowed size (from <see cref="ElementLayout.MaxWidth"/> and
        /// <see cref="ElementLayout.MaxHeight"/>).
        /// </param>
        /// <param name="currentAbsolutePosition">The current absolute position (relative to the viewport).</param>
        protected void RecomputeChildrenUtil(Size currentPrefSize, Size currentMaxSize, Point2D currentAbsolutePosition)
        {
            foreach (Element child in _children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                child.RecomputeLayout(currentPrefSize, currentMaxSize, currentAbsolutePosition);
            }
        }
    }
}
