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
        public AbstractShape(ThemeDefinition<ElementThemeData>? themeOverrides = null)
            : base(themeOverrides)
        {
        }

        public IBrush FillBrush { get; set; } = new ColorBrush(Color.Default);
        public IBrush OutlineBrush { get; set; } = new ColorBrush(Color.Default);
        public OutlineParams OutlineParameters { get; set; } = new();
    }
}
