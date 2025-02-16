using System;
using System.Numerics;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
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

        public override void RecalculateContainerChildren()
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
            Rect bounds = Bounds.BoundingRect;

            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                float minWidth = CalculateDimension(child.MinWidth, bounds.Width);
                float maxWidth = CalculateDimension(child.MaxWidth, bounds.Width);
                float prefWidth;

                //it's allowed to expand or shrink if it is IExpandable
                if (child is IExpandable expandable && expandable.CanExpandHorizontally)
                {
                    Size size = expandable.ComputeSizeInContainer();
                    prefWidth = CalculateDimension(size.Width, bounds.Width);
                }
                else
                {
                    prefWidth = Math.Clamp(
                        CalculateDimension(child.PreferredWidth, bounds.Width),
                        minWidth != 0 ? minWidth : float.MinValue,
                        maxWidth != 0 ? maxWidth : float.MaxValue);
                }

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
                PreferredWidth.IsUnset() ? bounds.Width : CalculateDimension(PreferredWidth, parentWidth);
            //it means that the container's preferred width is smaller that the minimum pref width of the content, so shrink the container
            if (minimumPreferredWidth > containerPrefWidth)
            {
                elementsNeedShrinking = true;
                float containerMaxWidth =
                    MaxWidth.IsUnset() ? bounds.Width : CalculateDimension(MaxWidth, parentWidth);
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

                var childPosition = new Point2D(currentPosX, currentPosY);

                //TODO: handle vertical positioning
                float childWidth;

                float minWidth = CalculateDimension(child.MinWidth, bounds.Width);
                float maxWidth = CalculateDimension(child.MaxWidth, bounds.Width);
                float prefWidth =
                    Math.Clamp(
                        CalculateDimension(child.PreferredWidth, bounds.Width),
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
                                childWidth = boxContainerSizing.HGrowthFactor * growthSectionWidth;

                                //if the result was smaller than the minWidth, set it to minWidth and update the allocated
                                //width accordingly (so that other elements can shrink correctly)
                                if (childWidth < minWidth)
                                {
                                    allocatedPreferredWidth += minWidth - childWidth;
                                    childWidth = minWidth;
                                }
                            }
                            else
                            {
                                childWidth = prefWidth;
                            }
                        }
                        else
                        {
                            //if the pref width wasn't set, only the min width, just set it as the min width
                            childWidth = prefWidth <= minWidth
                                ? minWidth
                                : NumberUtils.Lerp(minWidth, prefWidth, t);
                        }
                    }
                    //when the elements can no longer be shrunk
                    else
                    {
                        childWidth = minWidth;
                    }
                }
                else
                {
                    if (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing)
                    {
                        if (boxContainerSizing.HGrowthFactor > 0)
                        {
                            childWidth = boxContainerSizing.HGrowthFactor * growthSectionWidth;

                            //if the result was smaller than the minWidth, set it to minWidth and update the allocated
                            //width accordingly (so that other elements can shrink correctly)
                            if (childWidth < minWidth)
                            {
                                allocatedPreferredWidth += minWidth - childWidth;
                                childWidth = minWidth;
                            }
                        }
                        else
                        {
                            childWidth = prefWidth;
                        }
                    }
                    else
                    {
                        childWidth = prefWidth;
                    }
                }

                //we already performed the necessary calculations for this one 
                if (child is IExpandable)
                {
                    childWidth = child.Bounds.BoundingRect.Width;
                }
                else
                {
                    child.MarkLayoutDirty();
                }

                child.Bounds = new ElementBounds(
                    new Rect(childPosition.X, childPosition.Y, childWidth, finalHeight),
                    new Vector4());
                currentPosX += childWidth;
            }

            Bounds = new ElementBounds(
                new Rect(finalPosition.X, finalPosition.Y, finalWidth, finalHeight),
                new Vector4());
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
