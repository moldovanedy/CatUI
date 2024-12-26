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
    public partial class Ellipse : AbstractShape
    {
        public Ellipse(
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,

            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            OutlineParams? outlineParameters = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,
            ContainerSizing? elementContainerSizing = null,
            bool visible = true,
            bool enabled = true,

            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :
            base(fillBrush: fillBrush,
                 outlineBrush: outlineBrush,
                 outlineParameters: outlineParameters,
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

                 onDraw: onDraw,
                 onEnterDocument: onEnterDocument,
                 onExitDocument: onExitDocument,
                 onLoad: onLoad,
                 onPointerEnter: onPointerEnter,
                 onPointerLeave: onPointerLeave,
                 onPointerMove: onPointerMove)
        {
            DrawEvent += PrivateDrawOutline;
        }

        ~Ellipse()
        {
            DrawEvent -= PrivateDrawOutline;
        }

        protected override void DrawBackground()
        {
            if (FillBrush.IsSkippable)
            {
                return;
            }

            Rect rect = Bounds.GetContentBox();
            Document?.Renderer.DrawEllipse(
                new Point2D(rect.CenterX, rect.CenterY),
                rect.Width / 2f,
                rect.Height / 2f,
                FillBrush);
        }

        private void PrivateDrawOutline()
        {
            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Rect rect = Bounds.GetContentBox();
            Document?.Renderer.DrawEllipseOutline(
                new Point2D(rect.CenterX, rect.CenterY),
                rect.Width / 2f,
                rect.Height / 2f,
                OutlineBrush,
                OutlineParameters);
        }
    }
}
