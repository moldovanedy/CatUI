using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public partial class Ellipse : AbstractShape
    {
        public Ellipse(
            Dimension2 position,
            Dimension preferredWidth,
            Dimension preferredHeight,

            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            OutlineParams? outlineParameters = null,
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

                 onDraw: onDraw,
                 onEnterDocument: onEnterDocument,
                 onExitDocument: onExitDocument,
                 onLoad: onLoad,
                 onPointerEnter: onPointerEnter,
                 onPointerLeave: onPointerLeave,
                 onPointerMove: onPointerMove)
        {
            base.DrawEvent += PrivateDrawOutline;
        }

        ~Ellipse()
        {
            base.DrawEvent -= PrivateDrawOutline;
        }

        protected override void DrawBackground()
        {
            if (FillBrush.IsSkippable)
            {
                return;
            }

            Rect rect = this.Bounds.GetContentBox();
            this.Document?.Renderer?.DrawEllipse(
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

            Rect rect = this.Bounds.GetContentBox();
            this.Document?.Renderer?.DrawEllipseOutline(
                new Point2D(rect.CenterX, rect.CenterY),
                rect.Width / 2f,
                rect.Height / 2f,
                OutlineBrush,
                OutlineParameters);
        }
    }
}
