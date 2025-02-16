using System;
using System.Numerics;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;

namespace CatUI.Elements.Containers
{
    public abstract class BoxContainer : Container
    {
        /// <summary>
        /// Specifies the dimension of the space left between each of the elements in the container. By default, it's 0.
        /// </summary>
        public Dimension Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                SpacingProperty.Value = _spacing;
            }
        }

        private Dimension _spacing = new(0);
        public ObservableProperty<Dimension> SpacingProperty { get; private set; } = new(new Dimension(0));

        /// <summary>
        /// Specifies the orientation of this BoxContainer. Can be vertical or horizontal.
        /// </summary>
        public abstract Orientation BoxOrientation { get; }

        public BoxContainer(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
            SpacingProperty.ValueChangedEvent += SetSpacing;
        }

        ~BoxContainer()
        {
            SpacingProperty = null!;
        }

        private void SetSpacing(Dimension value)
        {
            _spacing = value;
        }

        public override Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            Size? parentEnforcedSize = null)
        {
            Size thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
            Point2D thisAbsolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisMaxSize = GetMaxSizeUtil(parentSize);

            float estimatedDim = 0, minimumElementsDim = 0;
            float allocatedMinDim = 0, totalGrowthFactors = 0;
            bool canRespectPositioning = true;

            float finalContainerDim = 0;
            Point2D initialAbsolutePosition = thisAbsolutePosition;

            //first pass: calculate the min and max sizes and estimate the final size of the content
            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                if (BoxOrientation == Orientation.Horizontal)
                {
                    if (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing &&
                        boxContainerSizing.HGrowthFactor > 0)
                    {
                        totalGrowthFactors += boxContainerSizing.HGrowthFactor;

                        float min = CalculateDimension(child.Layout.MinWidth ?? Dimension.Unset, thisMaxSize.Width);
                        minimumElementsDim += min;
                        //TODO: here it should somehow cache the element's last width, not always assume it's the min width
                        estimatedDim += min;

                        //if the max width is not set, it means it can expand infinitely
                        if ((child.Layout.MaxWidth ?? Dimension.Unset).IsUnset())
                        {
                            canRespectPositioning = false;
                        }

                        continue;
                    }
                }
                else
                {
                    if (child.ElementContainerSizing is VBoxContainerSizing boxContainerSizing &&
                        boxContainerSizing.VGrowthFactor > 0)
                    {
                        totalGrowthFactors += boxContainerSizing.VGrowthFactor;

                        float min = CalculateDimension(child.Layout.MinHeight ?? Dimension.Unset, thisMaxSize.Height);
                        minimumElementsDim += min;
                        //TODO: here it should somehow cache the element's last height, not always assume it's the min height
                        estimatedDim += min;

                        //if the max height is not set, it means it can expand infinitely
                        if ((child.Layout.MaxHeight ?? Dimension.Unset).IsUnset())
                        {
                            canRespectPositioning = false;
                        }

                        continue;
                    }
                }

                Dimension childPrefDimension =
                    (BoxOrientation == Orientation.Horizontal
                        ? child.Layout.GetSuggestedWidth()
                        : child.Layout.GetSuggestedHeight()) ?? Dimension.Unset;
                Dimension childMinDimension;

                if (BoxOrientation == Orientation.Horizontal)
                {
                    switch (child.Layout.WidthMode)
                    {
                        default:
                        case ElementLayout.LayoutMode.Fixed:
                            childMinDimension = childPrefDimension;
                            break;
                        case ElementLayout.LayoutMode.MinMax:
                            if (child.Layout.PrefersMaxWidth)
                            {
                                childMinDimension = child.Layout.MinWidth ?? Dimension.Unset;

                                //it considers the element as a growing element
                                totalGrowthFactors++;

                                //if the max dimension is unset, it means the element will be able to stretch infinitely,
                                //so there's no point calculating the max dimension
                                if ((child.Layout.MaxHeight ?? Dimension.Unset).IsUnset())
                                {
                                    canRespectPositioning = false;
                                    continue;
                                }
                            }
                            else
                            {
                                childMinDimension = childPrefDimension;
                            }

                            break;
                        case ElementLayout.LayoutMode.MinMaxAndPreferred:
                            childMinDimension = child.Layout.MinWidth ?? Dimension.Unset;
                            break;
                    }
                }
                else
                {
                    switch (child.Layout.HeightMode)
                    {
                        default:
                        case ElementLayout.LayoutMode.Fixed:
                            childMinDimension = childPrefDimension;
                            break;
                        case ElementLayout.LayoutMode.MinMax:
                            if (child.Layout.PrefersMaxHeight)
                            {
                                childMinDimension = child.Layout.MinHeight ?? Dimension.Unset;

                                //it considers the element as a growing element
                                totalGrowthFactors++;

                                //if the max dimension is unset, it means the element will be able to stretch infinitely,
                                //so there's no point calculating the max dimension
                                if ((child.Layout.MaxHeight ?? Dimension.Unset).IsUnset())
                                {
                                    canRespectPositioning = false;
                                    continue;
                                }
                            }
                            else
                            {
                                childMinDimension = childPrefDimension;
                            }

                            break;
                        case ElementLayout.LayoutMode.MinMaxAndPreferred:
                            childMinDimension = child.Layout.MinHeight ?? Dimension.Unset;
                            break;
                    }
                }

                float directMinDim = CalculateDimension(childMinDimension, thisSize.Height);

                minimumElementsDim += directMinDim;
                allocatedMinDim += directMinDim;

                //if the estimated dimension is infinity, it means that some elements that don't have a growth factor
                //want to stretch infinitely (ElementLayout.MaxHeight is unset and ElementLayout.PrefersMaxHeight is true
                //(Width or Height))
                if (!float.IsPositiveInfinity(estimatedDim))
                {
                    estimatedDim +=
                        CalculateDimension(
                            childPrefDimension,
                            BoxOrientation == Orientation.Horizontal ? thisSize.Width : thisSize.Height);
                }
            }

            float containerDim = BoxOrientation == Orientation.Horizontal ? thisSize.Width : thisSize.Height;

            //when it does NOT enter if, the whole content can fit and there's still space left, making it easy
            //to respect the position of the content
            if (canRespectPositioning && containerDim < estimatedDim)
            {
                canRespectPositioning = false;
            }

            //if true, the content cannot have its preferred size, but at least the minimum sizes can be respected
            bool needsForcedShrinking = containerDim >= minimumElementsDim;
            //represents the remaining size for the growing elements
            float remainingDimForGrowth = containerDim - allocatedMinDim;
            float growthSectionDim = remainingDimForGrowth / totalGrowthFactors;

            //the pass is "tainted" if an element declared a size, but after recalculation it has a different size
            //this means a third pass is necessary
            bool isTainted = false;

            //second pass: constrain the elements to the available space and call this function on them (the actual
            //layout part)
            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                //if it's growing
                if (child.ElementContainerSizing is HBoxContainerSizing ||
                    child.ElementContainerSizing is VBoxContainerSizing)
                {
                    float finalDim = BoxOrientation switch
                    {
                        Orientation.Horizontal when
                            child.ElementContainerSizing is HBoxContainerSizing hBoxContainerSizing =>
                            hBoxContainerSizing.HGrowthFactor * growthSectionDim,
                        Orientation.Vertical when
                            child.ElementContainerSizing is VBoxContainerSizing vBoxContainerSizing =>
                            vBoxContainerSizing.VGrowthFactor * growthSectionDim,
                        _ => 0
                    };

                    child.RecomputeLayout(thisSize, thisMaxSize, Point2D.Zero);

                    if (!isTainted)
                    {
                        Size finalSize = new(
                            Math.Max(
                                CalculateDimension(child.Layout.GetSuggestedWidth() ?? Dimension.Unset, thisSize.Width),
                                finalDim),
                            Math.Max(
                                CalculateDimension(child.Layout.GetSuggestedHeight() ?? Dimension.Unset,
                                    thisSize.Height),
                                finalDim));

                        //TODO: handle positioning on the different axis that the one from BoxOrientation
                        float x = thisAbsolutePosition.X;
                        float y = thisAbsolutePosition.Y;

                        child.Bounds = new ElementBounds(
                            new Rect(x, y, finalSize.Width, finalSize.Height),
                            child.Bounds.Margins);

                        finalContainerDim +=
                            BoxOrientation == Orientation.Horizontal
                                ? finalSize.Width
                                : finalSize.Height;

                        if (BoxOrientation == Orientation.Horizontal)
                        {
                            x += finalSize.Width;
                        }
                        else
                        {
                            y += finalSize.Height;
                        }

                        thisAbsolutePosition = new Point2D(x, y);
                    }
                }
                else
                {
                    float declaredWidth =
                        CalculateDimension(child.Layout.GetSuggestedWidth() ?? Dimension.Unset, thisSize.Width);
                    float declaredHeight =
                        CalculateDimension(child.Layout.GetSuggestedHeight() ?? Dimension.Unset, thisSize.Height);
                    Size actualSize;

                    if (canRespectPositioning || needsForcedShrinking)
                    {
                        actualSize = child.RecomputeLayout(thisSize, thisMaxSize, Point2D.Zero);
                    }
                    else
                    {
                        Size enforcedSize = new(declaredWidth, declaredHeight);
                        actualSize = child.RecomputeLayout(thisSize, thisSize, Point2D.Zero, enforcedSize);

                        //if the element didn't obey the enforced size, give a warning and consider the given size, even if
                        //this might break the UI
                        if (Math.Abs(actualSize.Width - declaredWidth) > 0.01 ||
                            Math.Abs(actualSize.Height - declaredHeight) > 0.01)
                        {
                            CatLogger.LogWarning(
                                "An element didn't obey the enforced size. Make sure you take the parentEnforcedSize into" +
                                "consideration when you override Element.RecomputeLayout. The layout might be corrupted.");
                        }
                    }

                    float difference;
                    if (BoxOrientation == Orientation.Horizontal)
                    {
                        difference = actualSize.Width - declaredWidth;
                    }
                    else
                    {
                        difference = actualSize.Height - declaredHeight;
                    }

                    if (Math.Abs(difference) > 0.01)
                    {
                        estimatedDim += difference;
                        isTainted = true;
                    }

                    //set the position
                    if (!isTainted)
                    {
                        //TODO: handle positioning on the different axis that the one from BoxOrientation
                        float x = thisAbsolutePosition.X;
                        float y = thisAbsolutePosition.Y;

                        child.Bounds = new ElementBounds(
                            new Rect(x, y, actualSize.Width, actualSize.Height),
                            child.Bounds.Margins);

                        finalContainerDim +=
                            BoxOrientation == Orientation.Horizontal
                                ? actualSize.Width
                                : actualSize.Height;

                        if (BoxOrientation == Orientation.Horizontal)
                        {
                            x += actualSize.Width;
                        }
                        else
                        {
                            y += actualSize.Height;
                        }

                        thisAbsolutePosition = new Point2D(x, y);
                    }
                }
            }

            Size size =
                BoxOrientation == Orientation.Horizontal
                    ? new Size(finalContainerDim, thisSize.Height)
                    : new Size(thisSize.Width, finalContainerDim);

            Bounds = new ElementBounds(
                new Rect(initialAbsolutePosition.X, initialAbsolutePosition.Y, size.Width, size.Height),
                new Vector4());

            return thisSize;
        }

        public enum Orientation
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}
