using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public abstract class AbstractShape : Element
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

        public IBrush OutlineBrush { get; set; } = new ColorBrush(Color.Default);
        public OutlineParams OutlineParameters { get; set; } = new OutlineParams();

        public AbstractShape(
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            OutlineParams? outlineParameters = null,

            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
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
            base(children: children,
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
            FillBrush = fillBrush ?? new ColorBrush(Color.Default);
            OutlineBrush = outlineBrush ?? new ColorBrush(Color.Default);
            OutlineParameters = outlineParameters ?? new OutlineParams();

            if (themeOverrides != null)
            {
                base.SetElementThemeOverrides(themeOverrides);
            }
        }

        #region Builder
        public AbstractShape SetInitialFillBrush(IBrush fillBrush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            FillBrush = fillBrush;
            return this;
        }

        public AbstractShape SetInitialOutlineBrush(IBrush outlineBrush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            OutlineBrush = outlineBrush;
            return this;
        }

        public AbstractShape SetInitialOutlineParameters(OutlineParams outlineParameters)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            OutlineParameters = outlineParameters;
            return this;
        }
        #endregion
    }
}