using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public partial class Rectangle : AbstractShape
    {
        public Rectangle(
            //AbstractShape
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            OutlineParams? outlineParameters = null,
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
                fillBrush: fillBrush,
                outlineBrush: outlineBrush,
                outlineParameters: outlineParameters,
                //
                name: name,
                children: children,
                themeOverrides: themeOverrides,
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
        //ReSharper enable ArgumentsStyleNamedExpression
        {
        }
    }
}
