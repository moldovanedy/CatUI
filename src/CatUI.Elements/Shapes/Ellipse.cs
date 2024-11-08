using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public class Ellipse : Element
    {
        public IBrush Brush
        {
            get
            {
                ElementThemeData? theme = base.GetElementThemeOverride(Element.STYLE_NORMAL);
                if (theme == null)
                {
                    theme = new ElementThemeData(Element.STYLE_NORMAL);
                    base.SetElementThemeOverride(Element.STYLE_NORMAL, theme);
                }

                IBrush? brush = theme.Background;
                if (brush == null)
                {
                    brush = new ColorBrush();
                    theme.Background = brush;
                }

                return brush;
            }
            set
            {
                ElementThemeData? theme = base.GetElementThemeOverride(Element.STYLE_NORMAL);
                if (theme == null)
                {
                    theme = new ElementThemeData(Element.STYLE_NORMAL);
                    base.SetElementThemeOverride(Element.STYLE_NORMAL, theme);
                }

                theme.Background = value;
            }
        }

        public Ellipse()
        {
            base.DrawEvent += PrivateDraw;
        }

        public Ellipse(
            IBrush rectBrush,
            Dimension2 position,
            Dimension width,
            Dimension height,
            UIDocument? doc = null,
            List<Element>? children = null,
            Dictionary<string, ElementThemeData>? themeOverrides = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(doc: doc,
                 children: children,
                 themeOverrides: themeOverrides,
                 position: position,
                 width: width,
                 height: height,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth)
        {
            Brush = rectBrush;
            base.DrawEvent += PrivateDraw;
        }

        ~Ellipse()
        {
            base.DrawEvent -= PrivateDraw;
        }

        public Ellipse SetInitialBrush(IBrush brush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            Brush = brush;
            return this;
        }

        private void PrivateDraw()
        {
            Rect rect = this.Bounds.GetContentBox();
            this.Document?.Renderer?.DrawEllipse(
                new Point2D(rect.CenterX, rect.CenterY),
                rect.Width / 2f,
                rect.Height / 2f,
                Brush);
        }
    }
}
