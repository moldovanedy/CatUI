using System;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Containers
{
    public class VBoxContainer : BoxContainer
    {
        public override Orientation BoxOrientation => Orientation.Vertical;

        public VBoxContainer(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
        }

        internal override void RecalculateLayout()
        {
            if (IsChildOfContainer || !Enabled)
            {
                return;
            }

            float finalWidth = 0, finalHeight;

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
                parentWidth = GetParent()?.Bounds.Width ?? 0;
                parentHeight = GetParent()?.Bounds.Height ?? 0;
                parentXPos = GetParent()?.Bounds.StartPoint.X ?? 0;
                parentYPos = GetParent()?.Bounds.StartPoint.Y ?? 0;
            }

            //this is in order to recalculate the Bounds
            AbsoluteHeight = Math.Clamp(
                PreferredHeight.IsUnset() ? parentHeight : CalculateDimension(PreferredHeight, parentHeight),
                MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));

            Point2D finalPosition = Point2D.Zero;
            if (!Position.IsUnset())
            {
                finalPosition = new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight));
            }

            if (!PreferredWidth.IsUnset())
            {
                finalWidth = Math.Clamp(
                    CalculateDimension(PreferredWidth, parentWidth),
                    MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                    MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            }
            else if (!MinWidth.IsUnset())
            {
                finalWidth = CalculateDimension(MinWidth, parentWidth);
            }

            float minimumPreferredHeight = 0, minimumMinHeight = 0;
            float allocatedPreferredHeight = 0, totalGrowthFactors = 0;

            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                float minHeight = CalculateDimension(child.MinHeight, Bounds.Height);
                float maxHeight = CalculateDimension(child.MaxHeight, Bounds.Height);
                float prefHeight =
                    Math.Clamp(
                        CalculateDimension(child.PreferredHeight, Bounds.Height),
                        minHeight != 0 ? minHeight : float.MinValue,
                        maxHeight != 0 ? maxHeight : float.MaxValue);

                if (child.ElementContainerSizing == null ||
                    (child.ElementContainerSizing is VBoxContainerSizing boxContainerSizing &&
                     boxContainerSizing.VGrowthFactor == 0))
                {
                    minimumPreferredHeight += prefHeight;
                    allocatedPreferredHeight += prefHeight;
                }
                else
                {
                    if (child.ElementContainerSizing is VBoxContainerSizing containerSizing)
                    {
                        totalGrowthFactors += containerSizing.VGrowthFactor;
                    }

                    minimumPreferredHeight += prefHeight;
                }

                minimumMinHeight += minHeight;
            }

            //calculate the container's final Height
            bool elementsNeedShrinking = false;
            float containerPrefHeight =
                PreferredHeight.IsUnset() ? Bounds.Height : CalculateDimension(PreferredHeight, parentHeight);
            //it means that the container's preferred height is smaller that the minimum pref height of the content, so shrink the container
            if (minimumPreferredHeight > containerPrefHeight)
            {
                elementsNeedShrinking = true;
                float containerMaxHeight =
                    MaxHeight.IsUnset() ? Bounds.Height : CalculateDimension(MaxHeight, parentHeight);
                //it means that the container's max height is smaller that the minimum Height of the content, 
                //so set the value as the max stretch of the content
                finalHeight = minimumPreferredHeight > containerMaxHeight ? containerMaxHeight : minimumPreferredHeight;
            }
            else
            {
                finalHeight = containerPrefHeight;
            }

            float currentPosX = finalPosition.X, currentPosY = finalPosition.Y;
            float t = (finalHeight - minimumMinHeight) / (minimumPreferredHeight - minimumMinHeight);
            float growthSectionHeight = (finalHeight - allocatedPreferredHeight) / totalGrowthFactors;

            foreach (Element child in Children)
            {
                if (!child.Enabled)
                {
                    continue;
                }

                child.AbsolutePosition = new Point2D(currentPosX, currentPosY);

                //TODO: handle horizontal positioning
                child.AbsoluteWidth = finalWidth;

                float minHeight = CalculateDimension(child.MinHeight, Bounds.Height);
                float maxHeight = CalculateDimension(child.MaxHeight, Bounds.Height);
                float prefHeight =
                    Math.Clamp(
                        CalculateDimension(child.PreferredHeight, Bounds.Height),
                        minHeight != 0 ? minHeight : float.MinValue,
                        maxHeight != 0 ? maxHeight : float.MaxValue);

                if (elementsNeedShrinking)
                {
                    //if the elements need to be shrunk, but they CAN be shrunk because the minHeight
                    //was smaller than prefHeight for at least one element
                    if (minimumPreferredHeight > minimumMinHeight)
                    {
                        if (child.ElementContainerSizing is VBoxContainerSizing boxContainerSizing)
                        {
                            if (boxContainerSizing.VGrowthFactor > 0)
                            {
                                //make it proportional with the shrinking of the other elements
                                growthSectionHeight =
                                    (finalHeight - (allocatedPreferredHeight * t)) / totalGrowthFactors;
                                child.AbsoluteHeight = boxContainerSizing.VGrowthFactor * growthSectionHeight;

                                //if the result was smaller than the minHeight, set it to minHeight and update the allocated
                                //height accordingly (so that other elements can shrink correctly)
                                if (child.AbsoluteHeight < minHeight)
                                {
                                    allocatedPreferredHeight += minHeight - child.AbsoluteHeight;
                                    child.AbsoluteHeight = minHeight;
                                }
                            }
                            else
                            {
                                child.AbsoluteHeight = prefHeight;
                            }
                        }
                        else
                        {
                            //if the pref height wasn't set, only the min height, just set it as the min height
                            child.AbsoluteHeight = prefHeight <= minHeight
                                ? minHeight
                                : NumberUtils.Lerp(minHeight, prefHeight, t);
                        }
                    }
                    //when the elements can no longer be shrunk
                    else
                    {
                        child.AbsoluteHeight = minHeight;
                    }
                }
                else
                {
                    if (child.ElementContainerSizing is VBoxContainerSizing boxContainerSizing)
                    {
                        if (boxContainerSizing.VGrowthFactor > 0)
                        {
                            child.AbsoluteHeight = boxContainerSizing.VGrowthFactor * growthSectionHeight;

                            //if the result was smaller than the minHeight, set it to minHeight and update the allocated
                            //height accordingly (so that other elements can shrink correctly)
                            if (child.AbsoluteHeight < minHeight)
                            {
                                allocatedPreferredHeight += minHeight - child.AbsoluteHeight;
                                child.AbsoluteHeight = minHeight;
                            }
                        }
                        else
                        {
                            child.AbsoluteHeight = prefHeight;
                        }
                    }
                    else
                    {
                        child.AbsoluteHeight = prefHeight;
                    }
                }

                currentPosY += child.AbsoluteHeight;
                child.RecalculateLayout();
            }

            AbsoluteWidth = finalWidth;
            AbsoluteHeight = finalHeight;
            AbsolutePosition = finalPosition;
        }
    }
}
