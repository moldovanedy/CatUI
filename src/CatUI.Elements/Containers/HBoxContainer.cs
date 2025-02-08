using System;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Elements.Behaviors;
using CatUI.Utils;

namespace CatUI.Elements.Containers
{
    public class HBoxContainer : BoxContainer
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<HBoxContainer>? Ref
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

        private ObjectRef<HBoxContainer>? _ref;

        public override Orientation BoxOrientation => Orientation.Horizontal;

        public HBoxContainer(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
        }

        protected override void RecalculateLayout()
        {
            float finalWidth, finalHeight = 0;
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

            //this is in order to recalculate the Bounds
            AbsoluteWidth = Math.Clamp(
                PreferredWidth.IsUnset() ? parentWidth : CalculateDimension(PreferredWidth, parentWidth),
                MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));

            Point2D finalPosition = Point2D.Zero;
            if (!Position.IsUnset())
            {
                finalPosition = new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight));
            }

            if (!PreferredHeight.IsUnset())
            {
                finalHeight = Math.Clamp(
                    CalculateDimension(PreferredHeight, parentHeight),
                    MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));
            }
            else if (!MinHeight.IsUnset())
            {
                finalHeight = CalculateDimension(MinHeight, parentHeight);
            }

            float minimumPreferredWidth = 0, minimumMinWidth = 0;
            float allocatedPreferredWidth = 0, totalGrowthFactors = 0;

            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                //it's allowed to expand or shrink if it is IExpandable
                if (child is IExpandable expandable && expandable.CanExpandHorizontally)
                {
                    child.MarkLayoutDirty();
                }

                float minWidth = CalculateDimension(child.MinWidth, Bounds.BoundingRect.Width);
                float maxWidth = CalculateDimension(child.MaxWidth, Bounds.BoundingRect.Width);
                float prefWidth =
                    Math.Clamp(
                        CalculateDimension(child.PreferredWidth, Bounds.BoundingRect.Width),
                        minWidth != 0 ? minWidth : float.MinValue,
                        maxWidth != 0 ? maxWidth : float.MaxValue);

                if (child.ElementContainerSizing == null ||
                    (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing &&
                     boxContainerSizing.HGrowthFactor == 0))
                {
                    minimumPreferredWidth += prefWidth;
                    allocatedPreferredWidth += prefWidth;
                }
                else
                {
                    if (child.ElementContainerSizing is HBoxContainerSizing containerSizing)
                    {
                        totalGrowthFactors += containerSizing.HGrowthFactor;
                    }

                    minimumPreferredWidth += prefWidth;
                }

                minimumMinWidth += minWidth;
            }

            //calculate the container's final width
            bool elementsNeedShrinking = false;
            float containerPrefWidth =
                PreferredWidth.IsUnset() ? Bounds.BoundingRect.Width : CalculateDimension(PreferredWidth, parentWidth);
            //it means that the container's preferred width is smaller that the minimum pref width of the content, so shrink the container
            if (minimumPreferredWidth > containerPrefWidth)
            {
                elementsNeedShrinking = true;
                float containerMaxWidth =
                    MaxWidth.IsUnset() ? Bounds.BoundingRect.Width : CalculateDimension(MaxWidth, parentWidth);
                //it means that the container's max width is smaller that the minimum width of the content, 
                //so set the value as the max stretch of the content
                finalWidth = minimumPreferredWidth > containerMaxWidth ? containerMaxWidth : minimumPreferredWidth;
            }
            else
            {
                finalWidth = containerPrefWidth;
            }

            float currentPosX = finalPosition.X, currentPosY = finalPosition.Y;
            float t = (finalWidth - minimumMinWidth) / (minimumPreferredWidth - minimumMinWidth);
            float growthSectionWidth = (finalWidth - allocatedPreferredWidth) / totalGrowthFactors;

            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                child.AbsolutePosition = new Point2D(currentPosX, currentPosY);

                //TODO: handle vertical positioning
                child.AbsoluteHeight = finalHeight;

                float minWidth = CalculateDimension(child.MinWidth, Bounds.BoundingRect.Width);
                float maxWidth = CalculateDimension(child.MaxWidth, Bounds.BoundingRect.Width);
                float prefWidth =
                    Math.Clamp(
                        CalculateDimension(child.PreferredWidth, Bounds.BoundingRect.Width),
                        minWidth != 0 ? minWidth : float.MinValue,
                        maxWidth != 0 ? maxWidth : float.MaxValue);

                if (elementsNeedShrinking)
                {
                    //if the elements need to be shrunk, but they CAN be shrunk because the minWidth
                    //was smaller than prefWidth for at least one element
                    if (minimumPreferredWidth > minimumMinWidth)
                    {
                        if (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing)
                        {
                            if (boxContainerSizing.HGrowthFactor > 0)
                            {
                                //make it proportional with the shrinking of the other elements
                                growthSectionWidth = (finalWidth - (allocatedPreferredWidth * t)) / totalGrowthFactors;
                                child.AbsoluteWidth = boxContainerSizing.HGrowthFactor * growthSectionWidth;

                                //if the result was smaller than the minWidth, set it to minWidth and update the allocated
                                //width accordingly (so that other elements can shrink correctly)
                                if (child.AbsoluteWidth < minWidth)
                                {
                                    allocatedPreferredWidth += minWidth - child.AbsoluteWidth;
                                    child.AbsoluteWidth = minWidth;
                                }
                            }
                            else
                            {
                                child.AbsoluteWidth = prefWidth;
                            }
                        }
                        else
                        {
                            //if the pref width wasn't set, only the min width, just set it as the min width
                            child.AbsoluteWidth = prefWidth <= minWidth
                                ? minWidth
                                : NumberUtils.Lerp(minWidth, prefWidth, t);
                        }
                    }
                    //when the elements can no longer be shrunk
                    else
                    {
                        child.AbsoluteWidth = minWidth;
                    }
                }
                else
                {
                    if (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing)
                    {
                        if (boxContainerSizing.HGrowthFactor > 0)
                        {
                            child.AbsoluteWidth = boxContainerSizing.HGrowthFactor * growthSectionWidth;

                            //if the result was smaller than the minWidth, set it to minWidth and update the allocated
                            //width accordingly (so that other elements can shrink correctly)
                            if (child.AbsoluteWidth < minWidth)
                            {
                                allocatedPreferredWidth += minWidth - child.AbsoluteWidth;
                                child.AbsoluteWidth = minWidth;
                            }
                        }
                        else
                        {
                            child.AbsoluteWidth = prefWidth;
                        }
                    }
                    else
                    {
                        child.AbsoluteWidth = prefWidth;
                    }
                }

                currentPosX += child.AbsoluteWidth;
            }

            AbsoluteWidth = finalWidth;
            AbsoluteHeight = finalHeight;
            AbsolutePosition = finalPosition;
        }

        public override HBoxContainer Duplicate()
        {
            return new HBoxContainer
            {
                Spacing = Spacing,
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
