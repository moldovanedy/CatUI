using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
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
            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(fillBrush: fillBrush,
                 outlineBrush: outlineBrush,
                 outlineParameters: outlineParameters,
                 doc: doc,
                 children: children,
                 themeOverrides: themeOverrides,
                 position: position,
                 preferredWidth: preferredWidth,
                 preferredHeight: preferredHeight,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth)
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
