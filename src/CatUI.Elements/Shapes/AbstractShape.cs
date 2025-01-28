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
    public abstract class AbstractShape : Element
    {
        public AbstractShape(
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                themeOverrides,
                preferredWidth,
                preferredHeight)
        {
            if (fillBrush != null)
            {
                FillBrush = fillBrush;
            }

            if (outlineBrush != null)
            {
                OutlineBrush = outlineBrush;
            }
        }

        public IBrush FillBrush { get; set; } = new ColorBrush(Color.Default);
        public IBrush OutlineBrush { get; set; } = new ColorBrush(Color.Default);
        public OutlineParams OutlineParameters { get; set; } = new();
    }
}
