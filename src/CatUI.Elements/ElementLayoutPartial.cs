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
        public ElementLayout? Layout
        {
            get => _layout;
            set => LayoutProperty.Value = value;
        }

        private ElementLayout? _layout;
        public ObservableProperty<ElementLayout> LayoutProperty { get; } = new();

        private void SetLayout(ElementLayout? value)
        {
            _layout = value;
            MarkLayoutDirty();
        }

        public event ChildLayoutChangedEventHandler? ChildLayoutChangedEvent;

        protected virtual void OnChildLayoutChanged(object? sender, ChildLayoutChangedEventArgs e)
        {
            if (IsChildOfContainer)
            {
                MarkLayoutDirty();
                return;
            }

            GetParentLayoutMetrics(out Size parentSize, out Size parentMaxSize, out Point2D parentAbsolutePosition);

            Point2D absolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
            Size thisMaxSize = GetMaxSizeUtil(parentSize);

            if (e.ChildIndex >= _children.Count)
            {
                return;
            }

            _children[e.ChildIndex].RecomputeLayout(thisSize, thisMaxSize, absolutePosition);
            Rect previousBounds = Bounds;
            Bounds = Rect.GetCommonBoundingRect(Bounds, _children[e.ChildIndex].Bounds);

            if (Math.Abs(previousBounds.X - Bounds.X) > 0.01 ||
                Math.Abs(previousBounds.Y - Bounds.Y) > 0.01 ||
                Math.Abs(previousBounds.Width - Bounds.Width) > 0.01 ||
                Math.Abs(previousBounds.Height - Bounds.Height) > 0.01)
            {
                MarkLayoutDirty();
            }
        }

        /// <summary>
        /// Returns the parent's layout by calculating the metrics from the root element to this element's parent.
        /// Only the max size is actually calculated, other metrics are taken from the parent bounds directly.
        /// </summary>
        /// <param name="parentSize">The parent's direct size.</param>
        /// <param name="parentMaxSize">The parent's max size.</param>
        /// <param name="parentAbsolutePosition">The parent's absolute position in the document.</param>
        protected void GetParentLayoutMetrics(
            out Size parentSize,
            out Size parentMaxSize,
            out Point2D parentAbsolutePosition)
        {
            Rect parentBounds = _parent?.Bounds ?? Bounds;
            parentAbsolutePosition = new Point2D(parentBounds.X, parentBounds.Y);
            parentSize = new Size(parentBounds.Width, parentBounds.Height);

            Element currentElement = this;
            List<int> childIndices = [];
            while (currentElement._parent != null)
            {
                currentElement = currentElement._parent;
                childIndices.Add(currentElement.IndexInParent);
            }

            //the root will always have its max size the same as its bounds 
            parentMaxSize = new Size(currentElement.Bounds.Width, currentElement.Bounds.Height);
            //from the direct child of root to this element's parent
            for (int i = childIndices.Count - 2; i >= 0; i--)
            {
                Element next = currentElement._children[childIndices[i]];
                parentMaxSize = next.GetMaxSizeUtil(parentMaxSize);
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
        /// <param name="parentEnforcedWidth">
        /// An optional parameter that is generally given inside containers. It represents the parent's forced
        /// width for this element. You MUST respect this and not give your own value, as the container is free to
        /// assume that the given value was set or even set it manually, in which case not obeying will create a broken
        /// UI.
        /// </param>
        /// <param name="parentEnforcedHeight">
        /// An optional parameter that is generally given inside containers. It represents the parent's forced
        /// width for this element. You MUST respect this and not give your own value, as the container is free to
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
            float? parentEnforcedWidth = null,
            float? parentEnforcedHeight = null)
        {
            Point2D absolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
            Size thisMaxSize = GetMaxSizeUtil(parentSize);

            if (parentEnforcedWidth != null)
            {
                thisSize = new Size(parentEnforcedWidth.Value, thisSize.Height);
                thisMaxSize = new Size(parentEnforcedWidth.Value, thisMaxSize.Height);
            }

            if (parentEnforcedHeight != null)
            {
                thisSize = new Size(thisSize.Width, parentEnforcedHeight.Value);
                thisMaxSize = new Size(thisMaxSize.Width, parentEnforcedHeight.Value);
            }

            RecomputeChildrenUtil(thisSize, thisMaxSize, absolutePosition);

            //IMPORTANT: When we consider all the children's bounds, it's important to know that if the bounds are not
            //the expected ones (from thisSize) because an element overflowed, we DON'T recalculate again, but leave
            //all elements as-is. The consequence is that the elements that have layouts in percentages might not really
            //be that exact percentage, so it's "not a bug, it's a feature" which should be stated clearly in the docs
            Bounds = GetFinalBoundsUtil(absolutePosition, thisSize);

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
            ElementLayout layout = _layout ?? new ElementLayout();
            Dimension? abstractWidth = layout.GetSuggestedWidth();
            Dimension? abstractHeight = layout.GetSuggestedHeight();

            float width;
            Dimension minWidth = layout.MinWidth ?? Dimension.Unset;
            Dimension maxWidth = layout.MaxWidth ?? Dimension.Unset;
            Dimension setWidth = abstractWidth ?? 0;

            Size maxAllowedSize = new(
                parentMaxSize.Width - CalculateDimension(Position.X, parentPreferredSize.Width),
                parentMaxSize.Height - CalculateDimension(Position.Y, parentPreferredSize.Height));

            switch (layout.WidthMode)
            {
                default:
                case ElementLayout.LayoutMode.Fixed:
                    width = CalculateDimension(setWidth, parentPreferredSize.Width);
                    break;
                case ElementLayout.LayoutMode.MinMax:
                    if (layout.PrefersMaxWidth)
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
                        CalculateDimension(layout.PreferredWidth ?? 0, parentPreferredSize.Width), min, max);
                    break;
            }

            float height;
            Dimension minHeight = layout.MinHeight ?? Dimension.Unset;
            Dimension maxHeight = layout.MaxHeight ?? Dimension.Unset;
            Dimension setHeight = abstractHeight ?? 0;

            switch (layout.HeightMode)
            {
                default:
                case ElementLayout.LayoutMode.Fixed:
                    height = CalculateDimension(setHeight, parentPreferredSize.Height);
                    break;
                case ElementLayout.LayoutMode.MinMax:
                    if (layout.PrefersMaxHeight)
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
                        CalculateDimension(layout.PreferredHeight ?? 0, parentPreferredSize.Height), min, max);
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
            ElementLayout layout = _layout ?? new ElementLayout();
            float maxWidth, maxHeight;

            if (layout.WidthMode == ElementLayout.LayoutMode.Fixed)
            {
                maxWidth = CalculateDimension(layout.PreferredWidth ?? Dimension.Unset, parentSize.Width);
            }
            else
            {
                maxWidth =
                    (layout.MaxWidth ?? Dimension.Unset).IsUnset()
                        ? float.PositiveInfinity
                        : CalculateDimension(layout.MaxWidth!.Value, parentSize.Width);
            }

            if (layout.HeightMode == ElementLayout.LayoutMode.Fixed)
            {
                maxHeight = CalculateDimension(layout.PreferredHeight ?? Dimension.Unset, parentSize.Height);
            }
            else
            {
                maxHeight =
                    (layout.MaxHeight ?? Dimension.Unset).IsUnset()
                        ? float.MaxValue
                        : CalculateDimension(layout.MaxHeight!.Value, parentSize.Height);
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
                if (!child.IsCurrentlyEnabled)
                {
                    continue;
                }

                child.RecomputeLayout(currentPrefSize, currentMaxSize, currentAbsolutePosition);
            }
        }

        /// <summary>
        /// Computes the final bounds by also considering every child's bounds (it's basically
        /// <see cref="Rect.GetCommonBoundingRect(CatUI.Data.Rect[])"/> for these bounds and all children bounds).
        /// Note that this might create bounds that are larger than the predicted size or position when children overflow
        /// this element. In this case, all CatUI elements just accept the given bounds and will not recompute the
        /// elements again. Read the documentation for layouts to get a better view of this feature.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The reason that we don't recalculate is that it will create infinite recalculations when some elements
        /// use percentages as layout descriptors, especially the ones that overflow the element in the first place.
        /// This behavior is respected by all built-in CatUI elements, but you are free to do whatever you want in your
        /// custom elements.
        /// </para>
        /// <para>
        /// Note that for elements with a large child count, this will take longer to compute (O(n), where n is the
        /// child count), so ordered containers generally don't call this, as they have more efficient ways of
        /// calculating this. If your custom element has the children ordered by their position, you should generally
        /// find alternative ways of doing what this function does, so you can increase performance.
        /// </para>
        /// </remarks>
        /// <param name="absolutePosition">The absolute position calculated for this element.</param>
        /// <param name="thisSize">The expected size of this element after all calculations.</param>
        /// <returns></returns>
        protected Rect GetFinalBoundsUtil(Point2D absolutePosition, Size thisSize)
        {
            Rect finalBounds = new(absolutePosition, thisSize);
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Element child in Children)
            {
                finalBounds = Rect.GetCommonBoundingRect(finalBounds, child.Bounds);
            }

            return finalBounds;
        }
    }
}
