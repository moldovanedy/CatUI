using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public partial class Rectangle : AbstractShape
    {
        public Rectangle(
            Dimension2? position,
            Dimension? preferredWidth,
            Dimension? preferredHeight,

            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
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
            base(fillBrush: fillBrush,
                 outlineBrush: outlineBrush,
                 children: children,
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
    }
}
