using System;
using CatUI.Data;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;

namespace CatUI.Elements.Containers.Linear
{
    public abstract partial class LinearContainerBase : Container
    {
        /// <summary>
        /// Specifies the arrangement of the children of this container. It only refers to the axis of orientation
        /// (i.e. <see cref="ContainerOrientation"/>, meaning horizontal for <see cref="RowContainer"/>, vertical
        /// for <see cref="ColumnContainer"/>); for the other axis, see ...
        /// </summary>
        public LinearArrangement Arrangement
        {
            get => _arrangement;
            set
            {
                SetArrangement(value);
                ArrangementProperty.Value = _arrangement;
            }
        }

        private LinearArrangement _arrangement = new();

        public ObservableProperty<LinearArrangement> ArrangementProperty { get; private set; } =
            new(new LinearArrangement());

        private void SetArrangement(LinearArrangement? arrangement)
        {
            _arrangement = arrangement ?? new LinearArrangement();
            MarkLayoutDirty();
        }

        protected AlignmentType PreferredAlignment { get; set; } = AlignmentType.Start;

        /// <summary>
        /// Specifies the orientation of this LinearContainerBase. Can be vertical or horizontal.
        /// </summary>
        public abstract Orientation ContainerOrientation { get; }

        public LinearContainerBase()
        {
            ArrangementProperty.ValueChangedEvent += SetArrangement;
        }

        //~LinearContainerBase()
        //{
        //    ArrangementProperty = null!;
        //}

        public override Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            float? parentEnforcedWidth = null,
            float? parentEnforcedHeight = null)
        {
            ElementLayout layout = Layout ?? new ElementLayout();
            Point2D thisAbsolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);

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

            if (Children.Count == 0)
            {
                Bounds = new Rect(thisAbsolutePosition, thisSize);
                return thisSize;
            }

            float estimatedDim = 0, minimumElementsDim = 0;
            float allocatedMinDim = 0, totalGrowthFactors = 0;
            bool canRespectPositioning = true;

            //this will be the children final dimension on the axis from ContainerOrientation
            float finalContainerDim = 0;
            //this will be the children final dimension on the opposite axis as the one from ContainerOrientation
            //this will be used at the final bounds calculations
            float finalOppositeDim = 0;
            Point2D initialAbsolutePosition = thisAbsolutePosition;

            //for the space-... type of justification, it will be set after the content size is estimated
            float actualSpacing = Arrangement.IsSpacingRelevant
                ? CalculateDimension(
                    Arrangement.Spacing,
                    ContainerOrientation == Orientation.Horizontal ? thisSize.Width : thisSize.Height)
                : 0;

            #region First pass

            //first pass: calculate the min and max sizes and estimate the final size of the content
            foreach (Element child in Children)
            {
                ElementLayout childLayout = child.Layout ?? new ElementLayout();
                if (!child.Enabled)
                {
                    continue;
                }

                if (ContainerOrientation == Orientation.Horizontal)
                {
                    if (child.ElementContainerSizing is RowContainerSizing rowContainerSizing &&
                        rowContainerSizing.GrowthFactor > 0)
                    {
                        totalGrowthFactors += rowContainerSizing.GrowthFactor;

                        float min = CalculateDimension(childLayout.MinWidth ?? Dimension.Unset, thisMaxSize.Width);
                        minimumElementsDim += min;
                        //TODO: here it should somehow cache the element's last width, not always assume it's the min width
                        estimatedDim += min;

                        canRespectPositioning = false;
                        continue;
                    }
                }
                else
                {
                    if (child.ElementContainerSizing is ColumnContainerSizing columnContainerSizing &&
                        columnContainerSizing.GrowthFactor > 0)
                    {
                        totalGrowthFactors += columnContainerSizing.GrowthFactor;

                        float min = CalculateDimension(childLayout.MinHeight ?? Dimension.Unset, thisMaxSize.Height);
                        minimumElementsDim += min;
                        //TODO: here it should somehow cache the element's last height, not always assume it's the min height
                        estimatedDim += min;

                        canRespectPositioning = false;
                        continue;
                    }
                }

                Dimension childPrefDimension =
                    (ContainerOrientation == Orientation.Horizontal
                        ? childLayout.GetSuggestedWidth()
                        : childLayout.GetSuggestedHeight()) ?? Dimension.Unset;

                Dimension childMinDimension =
                    GetChildMinDimension(
                        childLayout,
                        childPrefDimension,
                        out bool isConsideredGrowing,
                        out bool canContainerRespectPositioning);

                if (isConsideredGrowing)
                {
                    totalGrowthFactors++;
                }

                if (!canContainerRespectPositioning)
                {
                    canRespectPositioning = false;
                    continue;
                }

                float directMinDim = CalculateDimension(childMinDimension, thisSize.Height);

                minimumElementsDim += directMinDim + actualSpacing;
                allocatedMinDim += directMinDim + actualSpacing;

                //if the estimated dimension is infinity, it means that some elements that don't have a growth factor
                //want to stretch infinitely (ElementLayout.MaxHeight is unset and ElementLayout.PrefersMaxHeight is true
                //(Width or Height))
                if (!float.IsPositiveInfinity(estimatedDim))
                {
                    estimatedDim +=
                        CalculateDimension(
                            childPrefDimension,
                            ContainerOrientation == Orientation.Horizontal ? thisSize.Width : thisSize.Height)
                      + actualSpacing;
                }
            }

            #endregion

            float containerDim = ContainerOrientation == Orientation.Horizontal ? thisSize.Width : thisSize.Height;

            if (canRespectPositioning)
            {
                float maxDim = ContainerOrientation == Orientation.Horizontal ? thisMaxSize.Width : thisMaxSize.Height;
                bool isMinMax =
                    ContainerOrientation == Orientation.Horizontal
                        ? layout.WidthMode == ElementLayout.LayoutMode.MinMax
                        : layout.HeightMode == ElementLayout.LayoutMode.MinMax;

                //when it does NOT enter if, the whole content can fit and there's still space left, making it easy
                //to respect the position of the content
                if (estimatedDim >= maxDim && containerDim < estimatedDim)
                {
                    canRespectPositioning = false;
                }
                //when the container is min-max, the container dim is generally lower than the actual content size,
                //so set it to the estimate
                else if (isMinMax && estimatedDim > containerDim)
                {
                    containerDim = estimatedDim;
                }
            }
            //if the content can't fit, but the estimate is smaller than the container dim, make it equal to the container
            //dim so the calculations match
            else if (containerDim > estimatedDim)
            {
                estimatedDim = containerDim;
            }

            //when the justification type is one of the space-... and it can be respected
            if (!Arrangement.IsSpacingRelevant && canRespectPositioning && Children.Count > 1)
            {
                float totalSpacing = containerDim - estimatedDim;
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (Arrangement.ContentJustification)
                {
                    case LinearArrangement.JustificationType.SpaceAround:
                        actualSpacing = totalSpacing / Children.Count;
                        break;
                    case LinearArrangement.JustificationType.SpaceBetween:
                        actualSpacing = totalSpacing / (Children.Count - 1);
                        break;
                    case LinearArrangement.JustificationType.SpaceEvenly:
                        actualSpacing = totalSpacing / (Children.Count + 1);
                        break;
                }
            }

            //if true, the content cannot have its preferred size, but at least the minimum sizes can be respected
            bool needsForcedShrinking = containerDim < estimatedDim && containerDim >= minimumElementsDim;
            //represents the remaining size for the growing elements
            float remainingDimForGrowth = containerDim - allocatedMinDim;
            float growthSectionDim = remainingDimForGrowth / totalGrowthFactors;

            //the pass is "tainted" if an element declared a size, but after recalculation it has a different size;
            //this means a third pass is necessary
            bool isTainted = false;
            float deviation = 0;

            thisAbsolutePosition =
                CalculateStartingPosition(
                    thisAbsolutePosition,
                    containerDim,
                    estimatedDim,
                    actualSpacing);

            #region Second pass

            //second pass: constrain the elements to the available space and call this function on them (the actual
            //layout part)
            foreach (Element child in Children)
            {
                ElementLayout childLayout = child.Layout ?? new ElementLayout();
                if (!child.Enabled)
                {
                    continue;
                }

                //if it's growing
                if (child.ElementContainerSizing is RowContainerSizing ||
                    child.ElementContainerSizing is ColumnContainerSizing)
                {
                    float finalDim = ContainerOrientation switch
                    {
                        Orientation.Horizontal when
                            child.ElementContainerSizing is RowContainerSizing rowContainerSizing =>
                            Math.Max(
                                rowContainerSizing.GrowthFactor * growthSectionDim,
                                CalculateDimension(childLayout.MinWidth ?? Dimension.Unset, thisSize.Width)),
                        Orientation.Vertical when
                            child.ElementContainerSizing is ColumnContainerSizing columnContainerSizing =>
                            Math.Max(
                                columnContainerSizing.GrowthFactor * growthSectionDim,
                                CalculateDimension(childLayout.MinHeight ?? Dimension.Unset, thisSize.Height)),
                        _ => 0
                    };

                    Size actualSize = child.RecomputeLayout(
                        thisSize,
                        thisMaxSize,
                        Point2D.Zero,
                        ContainerOrientation == Orientation.Horizontal ? finalDim : null,
                        ContainerOrientation == Orientation.Vertical ? finalDim : null);

                    //if (!isTainted)
                    {
                        Size finalSize =
                            ContainerOrientation == Orientation.Horizontal
                                ? new Size(finalDim, actualSize.Height)
                                : new Size(actualSize.Width, finalDim);

                        // if (ContainerOrientation == Orientation.Horizontal)
                        // {
                        //     finalSize = new Size(
                        //         finalDim,
                        //         CalculateDimension(
                        //             child.Layout.GetSuggestedHeight() ?? Dimension.Unset,
                        //             thisSize.Height));
                        // }
                        // else
                        // {
                        //     finalSize = new Size(
                        //         CalculateDimension(
                        //             child.Layout.GetSuggestedWidth() ?? Dimension.Unset,
                        //             thisSize.Width),
                        //         finalDim);
                        // }

                        //works like an iterator
                        thisAbsolutePosition =
                            PositionChild(
                                child,
                                finalSize,
                                thisSize,
                                thisAbsolutePosition,
                                actualSpacing);
                        finalContainerDim +=
                            actualSpacing +
                            (ContainerOrientation == Orientation.Horizontal
                                ? finalSize.Width
                                : finalSize.Height);
                        finalOppositeDim =
                            Math.Max(
                                finalOppositeDim,
                                ContainerOrientation == Orientation.Horizontal
                                    ? finalSize.Height
                                    : finalSize.Width);
                    }
                }
                else
                {
                    float declaredWidth =
                        CalculateDimension(childLayout.GetSuggestedWidth() ?? Dimension.Unset, thisSize.Width);
                    float declaredHeight =
                        CalculateDimension(childLayout.GetSuggestedHeight() ?? Dimension.Unset, thisSize.Height);
                    Size actualSize;

                    if (canRespectPositioning || !needsForcedShrinking)
                    {
                        actualSize = child.RecomputeLayout(thisSize, thisMaxSize, Point2D.Zero);
                    }
                    else
                    {
                        bool isEnforced =
                            childLayout.WidthMode == ElementLayout.LayoutMode.Fixed &&
                            childLayout.HeightMode == ElementLayout.LayoutMode.Fixed;
                        Size enforcedSize = new(declaredWidth, declaredHeight);

                        actualSize = child.RecomputeLayout(
                            thisSize,
                            thisSize,
                            Point2D.Zero,
                            isEnforced ? enforcedSize.Width : null,
                            isEnforced ? enforcedSize.Height : null);

                        //if the element didn't obey the enforced size, give a warning and consider the given size, even if
                        //this might break the UI
                        if (isEnforced &&
                            (Math.Abs(actualSize.Width - declaredWidth) > 0.01 ||
                             Math.Abs(actualSize.Height - declaredHeight) > 0.01))
                        {
                            CatLogger.LogWarning(
                                "An element didn't obey the enforced size. Make sure you take the parentEnforcedSize into" +
                                "consideration when you override Element.RecomputeLayout. The layout might be corrupted.");
                        }
                    }

                    //TODO: we might need to consider the maximum size as well, not just the actual size
                    if (ContainerOrientation == Orientation.Horizontal)
                    {
                        deviation += actualSize.Width - declaredWidth;
                    }
                    else
                    {
                        deviation += actualSize.Height - declaredHeight;
                    }

                    if (Math.Abs(deviation) > 0.5)
                    {
                        estimatedDim += deviation;
                        deviation = 0;
                        isTainted = true;
                    }

                    //set the position
                    //if (!isTainted)
                    {
                        //works like an iterator
                        thisAbsolutePosition =
                            PositionChild(
                                child,
                                actualSize,
                                thisSize,
                                thisAbsolutePosition,
                                actualSpacing);
                        finalContainerDim +=
                            actualSpacing +
                            (ContainerOrientation == Orientation.Horizontal
                                ? actualSize.Width
                                : actualSize.Height);
                        finalOppositeDim =
                            Math.Max(
                                finalOppositeDim,
                                ContainerOrientation == Orientation.Horizontal
                                    ? actualSize.Height
                                    : actualSize.Width);
                    }
                }
            }

            #endregion

            //a third pass happens only if an element declared a size, but after recalculation it was a different
            //size
            if (isTainted)
            {
                //TODO
            }

            //if the layout mode for is not fixed, respect the min between the layout max and the actual computed size
            //(finalContainerDim), otherwise respect the layout fixed size
            bool respectsComputedFinalDim =
                ContainerOrientation == Orientation.Horizontal
                    ? layout.WidthMode != ElementLayout.LayoutMode.Fixed && !layout.PrefersMaxWidth
                    : layout.HeightMode != ElementLayout.LayoutMode.Fixed && !layout.PrefersMaxHeight;

            float finalMaxLayoutDim =
                ContainerOrientation == Orientation.Horizontal
                    ? thisMaxSize.Width
                    : thisMaxSize.Height;
            finalContainerDim =
                respectsComputedFinalDim
                    ? Math.Min(finalContainerDim, finalMaxLayoutDim)
                    //if fixed, max will be the same as the preferred size
                    : finalMaxLayoutDim;

            bool respectsComputedOppositeDim =
                ContainerOrientation == Orientation.Horizontal
                    ? layout.HeightMode != ElementLayout.LayoutMode.Fixed && !layout.PrefersMaxHeight
                    : layout.WidthMode != ElementLayout.LayoutMode.Fixed && !layout.PrefersMaxWidth;

            float oppositeMaxLayoutDim =
                ContainerOrientation == Orientation.Horizontal
                    ? thisMaxSize.Height
                    : thisMaxSize.Width;
            finalOppositeDim =
                respectsComputedOppositeDim
                    ? Math.Min(finalOppositeDim, oppositeMaxLayoutDim)
                    : oppositeMaxLayoutDim;

            Size size =
                ContainerOrientation == Orientation.Horizontal
                    ? new Size(finalContainerDim, finalOppositeDim)
                    : new Size(finalOppositeDim, finalContainerDim);

            Rect previousBounds = Bounds;
            Bounds = new Rect(initialAbsolutePosition.X, initialAbsolutePosition.Y, size.Width, size.Height);

            if (Math.Abs(previousBounds.X - Bounds.X) > 0.01 ||
                Math.Abs(previousBounds.Y - Bounds.Y) > 0.01 ||
                Math.Abs(previousBounds.Width - Bounds.Width) > 0.01 ||
                Math.Abs(previousBounds.Height - Bounds.Height) > 0.01)
            {
                //this causes StackOverflowException
                //MarkLayoutDirty();
            }

            return size;
        }

        protected override void OnChildLayoutChanged(object? sender, ChildLayoutChangedEventArgs e)
        {
            if (IsChildOfContainer)
            {
                MarkLayoutDirty();
                return;
            }

            base.OnChildLayoutChanged(sender, e);

            if (e.ChildIndex >= Children.Count)
            {
                return;
            }

            Element? parent = GetParent();
            if (parent != null)
            {
                GetParentLayoutMetrics(out Size parentSize, out Size parentMaxSize, out Point2D parentAbsolutePosition);
                RecomputeLayout(parentSize, parentMaxSize, parentAbsolutePosition);
            }
            else if (Document != null)
            {
                RecomputeLayout(Document.ViewportSize, Document.ViewportSize, new Point2D());
            }
        }

        private Dimension GetChildMinDimension(
            ElementLayout layout,
            Dimension childPrefDimension,
            out bool isConsideredGrowing,
            out bool canContainerRespectPositioning)
        {
            isConsideredGrowing = false;
            canContainerRespectPositioning = true;

            if (ContainerOrientation == Orientation.Horizontal)
            {
                switch (layout.WidthMode)
                {
                    default:
                    case ElementLayout.LayoutMode.Fixed:
                        return childPrefDimension;
                    case ElementLayout.LayoutMode.MinMax:
                        if (layout.PrefersMaxWidth)
                        {
                            //it considers the element as a growing element
                            isConsideredGrowing = true;

                            //if the max dimension is unset, it means the element will be able to stretch infinitely,
                            //so there's no point calculating the max dimension
                            if ((layout.MaxHeight ?? Dimension.Unset).IsUnset())
                            {
                                canContainerRespectPositioning = false;
                            }

                            return layout.MinWidth ?? Dimension.Unset;
                        }

                        return childPrefDimension;
                    case ElementLayout.LayoutMode.MinMaxAndPreferred:
                        return layout.MinWidth ?? Dimension.Unset;
                }
            }
            else
            {
                switch (layout.HeightMode)
                {
                    default:
                    case ElementLayout.LayoutMode.Fixed:
                        return childPrefDimension;
                    case ElementLayout.LayoutMode.MinMax:
                        if (layout.PrefersMaxHeight)
                        {
                            //it considers the element as a growing element
                            isConsideredGrowing = true;

                            //if the max dimension is unset, it means the element will be able to stretch infinitely,
                            //so there's no point calculating the max dimension
                            if ((layout.MaxHeight ?? Dimension.Unset).IsUnset())
                            {
                                canContainerRespectPositioning = false;
                            }

                            return layout.MinHeight ?? Dimension.Unset;
                        }

                        return childPrefDimension;
                    case ElementLayout.LayoutMode.MinMaxAndPreferred:
                        return layout.MinHeight ?? Dimension.Unset;
                }
            }
        }

        /// <summary>
        /// Positions the child element, then returns the position of the next element. The caller must set
        /// currentPosition directly for the first element's position, so it must compute that position taking the
        /// initial space into account.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="childFinalSize">The final size of the child in pixels.</param>
        /// <param name="containerSize">The size of the container in pixels.</param>
        /// <param name="currentAbsolutePosition">The current abs. position of this child.</param>
        /// <param name="constSpacing">
        /// The spacing between elements, in pixels. This should be constant, so calculate it once and pass it here.
        /// </param>
        /// <returns>The absolute position of the next element.</returns>
        private Point2D PositionChild(
            Element child,
            Size childFinalSize,
            Size containerSize,
            Point2D currentAbsolutePosition,
            float constSpacing)
        {
            float x = currentAbsolutePosition.X;
            float y = currentAbsolutePosition.Y;

            float xOffset = 0;
            float yOffset = 0;

            if (ContainerOrientation == Orientation.Horizontal)
            {
                VerticalAlignmentType align =
                    child.ElementContainerSizing is RowContainerSizing rowSizing
                        ? rowSizing.VerticalAlignment
                        : (VerticalAlignmentType)PreferredAlignment;

                switch (align)
                {
                    default:
                    case VerticalAlignmentType.Top:
                        yOffset = 0;
                        break;
                    case VerticalAlignmentType.Center:
                        yOffset = (containerSize.Height - childFinalSize.Height) / 2f;
                        break;
                    case VerticalAlignmentType.Bottom:
                        yOffset = containerSize.Height - childFinalSize.Height;
                        break;
                }
            }
            else
            {
                HorizontalAlignmentType align =
                    child.ElementContainerSizing is ColumnContainerSizing columnSizing
                        ? columnSizing.HorizontalAlignment
                        : (HorizontalAlignmentType)PreferredAlignment;

                switch (align)
                {
                    default:
                    case HorizontalAlignmentType.Left:
                        xOffset = 0;
                        break;
                    case HorizontalAlignmentType.Center:
                        xOffset = (containerSize.Width - childFinalSize.Width) / 2f;
                        break;
                    case HorizontalAlignmentType.Right:
                        xOffset = containerSize.Width - childFinalSize.Width;
                        break;
                }
            }

            child.Bounds = new Rect(x + xOffset, y + yOffset, childFinalSize.Width, childFinalSize.Height);
            UpdatePositionOfChildren(child, new Point2D(x + xOffset, y + yOffset));

            if (ContainerOrientation == Orientation.Horizontal)
            {
                x += childFinalSize.Width + constSpacing;
            }
            else
            {
                y += childFinalSize.Height + constSpacing;
            }

            return new Point2D(x, y);
        }

        private Point2D CalculateStartingPosition(
            Point2D containerStartPosition,
            float containerDimension,
            float contentDimension,
            float constSpacing)
        {
            //if only one child and one of the space-... types of justification, it behaves as Center
            if (
                Children.Count <= 1 &&
                (Arrangement.ContentJustification == LinearArrangement.JustificationType.SpaceAround ||
                 Arrangement.ContentJustification == LinearArrangement.JustificationType.SpaceBetween ||
                 Arrangement.ContentJustification == LinearArrangement.JustificationType.SpaceEvenly))
            {
                if (ContainerOrientation == Orientation.Horizontal)
                {
                    return new Point2D(
                        containerStartPosition.X + ((containerDimension - contentDimension) / 2f),
                        containerStartPosition.Y);
                }

                return new Point2D(
                    containerStartPosition.X,
                    containerStartPosition.Y + ((containerDimension - contentDimension) / 2f));
            }

            if (
                Arrangement.ContentJustification == LinearArrangement.JustificationType.Start ||
                Arrangement.ContentJustification == LinearArrangement.JustificationType.SpaceBetween)
            {
                return containerStartPosition;
            }

            float x = containerStartPosition.X;
            float y = containerStartPosition.Y;

            float delta = 0;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Arrangement.ContentJustification)
            {
                case LinearArrangement.JustificationType.Center:
                    delta = (containerDimension - contentDimension) / 2f;
                    break;
                case LinearArrangement.JustificationType.End:
                    delta = containerDimension - contentDimension;
                    break;
                case LinearArrangement.JustificationType.SpaceAround:
                    delta = constSpacing / 2f;
                    break;
                case LinearArrangement.JustificationType.SpaceEvenly:
                    delta = constSpacing;
                    break;
            }

            if (ContainerOrientation == Orientation.Horizontal)
            {
                x += delta;
            }
            else
            {
                y += delta;
            }

            return new Point2D(x, y);
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
