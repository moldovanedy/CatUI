using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Brushes;
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
            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(fillBrush: fillBrush,
                 outlineBrush: outlineBrush,
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
        { }
    }
}
