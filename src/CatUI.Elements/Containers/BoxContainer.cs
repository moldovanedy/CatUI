using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Containers
{
    public abstract class BoxContainer : Container
    {
        /// <summary>
        /// Specifies the dimension of the space left between each of the elements in the container. By default, it's 0.
        /// </summary>
        public Dimension Spacing { get; set; } = Dimension.Unset;

        /// <summary>
        /// Specifies the orientation of this BoxContainer. Can be vertical or horizontal.
        /// </summary>
        public abstract Orientation BoxOrientation { get; }

        public BoxContainer(
            Dimension? spacing = null,
            //Element
            string name = "",
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,
            ContainerSizing? elementContainerSizing = null,
            bool visible = true,
            bool enabled = true,
            //Element actions
            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :

            //ReSharper disable ArgumentsStyleNamedExpression
            base(
                name: name,
                children: children,
                position: position,
                preferredWidth: preferredWidth,
                preferredHeight: preferredHeight,
                minHeight: minHeight,
                minWidth: minWidth,
                maxHeight: maxHeight,
                maxWidth: maxWidth,
                elementContainerSizing: elementContainerSizing,
                visible: visible,
                enabled: enabled,
                //
                onDraw: onDraw,
                onEnterDocument: onEnterDocument,
                onExitDocument: onExitDocument,
                onLoad: onLoad,
                onPointerEnter: onPointerEnter,
                onPointerLeave: onPointerLeave,
                onPointerMove: onPointerMove)
        {
            if (spacing != null)
            {
                Spacing = spacing;
            }
        }

        public enum Orientation
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}
