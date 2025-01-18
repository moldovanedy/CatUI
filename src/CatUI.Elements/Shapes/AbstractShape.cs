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
            OutlineParams? outlineParameters = null,
            //Element
            string name = "",
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,
            ContainerSizing? elementContainerSizing = null,
            bool visible = true,
            bool enabled = true,
            //Element actions
            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :

            //ReSharper disable ArgumentsStyleNamedExpression
            base(
                name: name,
                children: children,
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
                //
                onDraw: onDraw,
                onEnterDocument: onEnterDocument,
                onExitDocument: onExitDocument,
                onLoad: onLoad,
                onPointerEnter: onPointerEnter,
                onPointerLeave: onPointerLeave,
                onPointerMove: onPointerMove)
        //ReSharper enable ArgumentsStyleNamedExpression
        {
            FillBrush = fillBrush ?? new ColorBrush(Color.Default);
            OutlineBrush = outlineBrush ?? new ColorBrush(Color.Default);
            OutlineParameters = outlineParameters ?? new OutlineParams();

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }
        }

        public IBrush FillBrush
        {
            get
            {
                ElementThemeData? theme = GetElementThemeOverride(ElementThemeData.STYLE_NORMAL);
                if (theme == null)
                {
                    theme = new ElementThemeData(ElementThemeData.STYLE_NORMAL);
                    SetElementThemeOverride(ElementThemeData.STYLE_NORMAL, theme);
                }

                IBrush brush = theme.Background ?? new ColorBrush();
                return brush;
            }
            set
            {
                ElementThemeData? theme = GetElementThemeOverride(ElementThemeData.STYLE_NORMAL);
                if (theme == null)
                {
                    theme = new ElementThemeData(ElementThemeData.STYLE_NORMAL);
                    SetElementThemeOverride(ElementThemeData.STYLE_NORMAL, theme);
                }

                theme.Background = value;
            }
        }

        public IBrush OutlineBrush { get; set; }
        public OutlineParams OutlineParameters { get; set; }
    }
}
