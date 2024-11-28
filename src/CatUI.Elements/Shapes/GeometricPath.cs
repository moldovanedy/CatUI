using System;
using System.Collections.Generic;
using System.Diagnostics;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Themes;
using CatUI.Data.Managers;
using SkiaSharp;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;

namespace CatUI.Elements.Shapes
{
    public partial class GeometricPath : AbstractShape
    {
        private SKPath _skiaPath = new SKPath();

        /// <summary>
        /// If true, the path will be scaled to respect the element's width and height. If false, no scaling will be applied,
        /// but the path might exceed the element's bounds (width and height).
        /// </summary>
        public bool ShouldApplyScaling { get; set; }

        private Point2D _lastStartPoint = Point2D.Zero;
        private Point2D _lastAppliedScale = new Point2D(1, 1);

        public GeometricPath(
            Dimension2 position,
            Dimension preferredWidth,
            Dimension preferredHeight,

            bool shouldApplyScaling = false,

            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            OutlineParams? outlineParameters = null,
            string svgPath = "",
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
            ShouldApplyScaling = shouldApplyScaling;

            if (!string.IsNullOrEmpty(svgPath))
            {
                _skiaPath = SKPath.ParseSvgPathData(svgPath);
            }
        }

        ~GeometricPath()
        {
            PathCache.RemovePath(_skiaPath);
        }

        public SKPath GetSkiaPathClone()
        {
            return new SKPath(_skiaPath);
        }

        public void SetNewSkiaPath(SKPath path)
        {
            if (path == _skiaPath)
            {
                return;
            }

            PathCache.RemovePath(_skiaPath);
            _skiaPath = path;
        }

        /// <summary>
        /// Will reset the current path to the newly given SVG data from the SVG's <code>path</code> element.
        /// </summary>
        /// <param name="svgPath">The SVG data from the SVG's <code>path</code> element.</param>
        public void RecreateFromSvgPath(string svgPath)
        {
            PathCache.RemovePath(_skiaPath);
            _skiaPath = SKPath.ParseSvgPathData(svgPath);
        }

        protected override void DrawBackground()
        {
            _lastStartPoint = new Point2D(_skiaPath.TightBounds.Left, _skiaPath.TightBounds.Top);

            if (ShouldApplyScaling)
            {
                _lastAppliedScale = new Point2D(
                    this.Bounds.Width / _skiaPath.TightBounds.Width,
                    this.Bounds.Height / _skiaPath.TightBounds.Height);
            }
            else
            {
                _lastAppliedScale = new Point2D(1, 1);
            }

            _skiaPath.Transform(SKMatrix.CreateScaleTranslation(
                _lastAppliedScale.X,
                _lastAppliedScale.Y,
                base.Bounds.StartPoint.X - _lastStartPoint.X,
                base.Bounds.StartPoint.Y - _lastStartPoint.Y));

            Document?.Renderer?.DrawPath(_skiaPath, FillBrush, OutlineBrush, OutlineParameters);
        }
    }
}