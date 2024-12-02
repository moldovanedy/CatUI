using System;
using System.Collections.Generic;

using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;
using CatUI.Shared;

namespace CatUI.Elements.Containers
{
    public class HBoxContainer : BoxContainer
    {
        public HBoxContainer(
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,

            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :
            base(children: children,
                 themeOverrides: themeOverrides,
                 position: position,
                 preferredWidth: preferredWidth,
                 preferredHeight: preferredHeight,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth,

                 onDraw: onDraw,
                 onEnterDocument: onEnterDocument,
                 onExitDocument: onExitDocument,
                 onLoad: onLoad,
                 onPointerEnter: onPointerEnter,
                 onPointerLeave: onPointerLeave,
                 onPointerMove: onPointerMove)
        { }

        internal override void RecalculateLayout()
        {
            if (IsChildOfContainer == true)
            {
                return;
            }

            List<Element> children = GetChildren(true);
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
                parentWidth = GetParent()?.Bounds.Width ?? 0;
                parentHeight = GetParent()?.Bounds.Height ?? 0;
                parentXPos = GetParent()?.Bounds.StartPoint.X ?? 0;
                parentYPos = GetParent()?.Bounds.StartPoint.Y ?? 0;
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
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentHeight));
            }
            else if (!MinHeight.IsUnset())
            {
                finalHeight = CalculateDimension(MinHeight, parentHeight);
            }

            float minimumPreferredWidth = 0, minimumMinWidth = 0;
            float allocatedPreferredWidth = 0, totalGrowthFactors = 0;

            foreach (Element child in children)
            {
                float prefWidth = CalculateDimension(child.PreferredWidth, Bounds.Width);
                float minWidth = CalculateDimension(child.MinWidth, Bounds.Width);

                if (child.ElementContainerSizing == null ||
                    (
                        (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing) &&
                        boxContainerSizing.HGrowthFactor == 0))
                {
                    minimumPreferredWidth += prefWidth;
                    minimumMinWidth += minWidth;
                    allocatedPreferredWidth += prefWidth;
                }
                else
                {
                    if (child.ElementContainerSizing is HBoxContainerSizing containerSizing)
                    {
                        totalGrowthFactors += containerSizing.HGrowthFactor;
                    }
                    minimumPreferredWidth += minWidth;
                }
            }

            //calculate the container's final width
            bool elementsNeedShrinking = false;
            float containerPrefWidth =
                PreferredWidth.IsUnset() ? Bounds.Width : CalculateDimension(PreferredWidth, parentWidth);
            //it means that the container's preferred width is smaller that the minimum width of the content, so expand the container
            if (minimumPreferredWidth > containerPrefWidth)
            {
                elementsNeedShrinking = true;
                float containerMaxWidth =
                    MaxWidth.IsUnset() ? Bounds.Width : CalculateDimension(MaxWidth, parentHeight);
                //it means that the container's max width is smaller that the minimum width of the content, 
                //so set the the value as the max stretch of the content
                if (minimumPreferredWidth > containerMaxWidth)
                {
                    finalWidth = containerMaxWidth;
                }
                else
                {
                    finalWidth = minimumPreferredWidth;
                }
            }
            else
            {
                finalWidth = containerPrefWidth;
            }

            float currentPosX = finalPosition.X, currentPosY = finalPosition.Y;
            foreach (Element child in children)
            {
                child.AbsolutePosition.X = currentPosX;
                child.AbsolutePosition.Y = currentPosY;

                //TODO: handle vertical positioning
                child.AbsoluteHeight = finalHeight;

                if (elementsNeedShrinking)
                {
                    float minWidth =
                        child.MinWidth.IsUnset() ? 0 : CalculateDimension(child.MinWidth, Bounds.Width);

                    if (minimumPreferredWidth > minimumMinWidth)
                    {
                        float t = (finalWidth - minimumMinWidth) / (minimumPreferredWidth - minimumMinWidth);
                        float prefWidth =
                            child.PreferredWidth.IsUnset() ? 0 : CalculateDimension(child.PreferredWidth, Bounds.Width);

                        child.AbsoluteWidth = NumberUtils.Lerp(minWidth, prefWidth, t);
                    }
                    else
                    {
                        child.AbsoluteWidth = minWidth;
                    }
                }
                else
                {
                    float unallocatedWidth = finalWidth - allocatedPreferredWidth;
                    float growthSectionWidth = unallocatedWidth / totalGrowthFactors;

                    if (child.ElementContainerSizing is HBoxContainerSizing boxContainerSizing)
                    {
                        if (boxContainerSizing.HGrowthFactor != 0)
                        {
                            child.AbsoluteWidth = boxContainerSizing.HGrowthFactor * growthSectionWidth;
                        }
                        else
                        {
                            child.AbsoluteWidth = 0;
                        }
                    }
                    else
                    {
                        if (child.PreferredWidth.IsUnset())
                        {
                            if (!child.MinWidth.IsUnset())
                            {
                                child.AbsoluteWidth = CalculateDimension(child.PreferredWidth, Bounds.Width);
                            }
                        }
                        else
                        {
                            child.AbsoluteWidth = CalculateDimension(child.PreferredWidth, Bounds.Width);
                        }
                    }
                }

                currentPosX += child.AbsoluteWidth;
                child.RecalculateLayout();
            }

            AbsoluteWidth = finalWidth;
            AbsoluteHeight = finalHeight;
            AbsolutePosition = finalPosition;
        }
    }
}