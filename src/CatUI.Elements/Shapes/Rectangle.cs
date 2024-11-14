using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public class Rectangle : Element, IGeometricShape
    {
        public IBrush FillBrush
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

        public IBrush OutlineBrush { get; set; }
        public OutlineParams OutlineParameters { get; set; } = new OutlineParams();

        public Rectangle(
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
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
            FillBrush = fillBrush ?? new ColorBrush(new Color(0));
            OutlineBrush = outlineBrush ?? new ColorBrush(new Color(0));
        }

        #region Builder
        public Rectangle SetInitialFillBrush(IBrush fillBrush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            FillBrush = fillBrush;
            return this;
        }

        public Rectangle SetInitialOutlineBrush(IBrush outlineBrush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            OutlineBrush = outlineBrush;
            return this;
        }

        public Rectangle SetInitialOutlineParameters(OutlineParams outlineParameters)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            OutlineParameters = outlineParameters;
            return this;
        }
        #endregion //Builder
    }
}
