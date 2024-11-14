using System;
using System.Collections.Generic;
using System.Diagnostics;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Themes;
using CatUI.Data.Managers;
using SkiaSharp;

namespace CatUI.Elements.Shapes
{
    public class GeometricPath : Element, IGeometricShape
    {
        private SKPath _skiaPath = new SKPath();

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
        /// <summary>
        /// If true, the path will be scaled to respect the element's width and height. If false, no scaling will be applied,
        /// but the path might exceed the element's bounds (width and height).
        /// </summary>
        public bool ShouldApplyScaling { get; set; }

        private Point2D _lastStartPoint = Point2D.Zero;
        private Point2D _lastAppliedScale = new Point2D(1, 1);

        public GeometricPath(
            Dimension2 position,
            Dimension width,
            Dimension height,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            OutlineParams? outlineParameters = null,
            string svgPath = "",
            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
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
            OutlineParameters = outlineParameters ?? new OutlineParams();

            if (!string.IsNullOrEmpty(svgPath))
            {
                _skiaPath = SKPath.ParseSvgPathData(svgPath);
            }

            _skiaPath.MoveTo(25, 35);
            _skiaPath.LineTo(new SKPoint(45, 60));
            _skiaPath.AddCircle(100, 120, 20);
            _skiaPath.ArcTo(40, 40, 29, SKPathArcSize.Large, SKPathDirection.Clockwise, 25, 25);
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

        #region Builder
        public GeometricPath SetInitialFillBrush(IBrush fillBrush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            FillBrush = fillBrush;
            return this;
        }

        public GeometricPath SetInitialOutlineBrush(IBrush outlineBrush)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            OutlineBrush = outlineBrush;
            return this;
        }

        public GeometricPath SetInitialOutlineParameters(OutlineParams outlineParameters)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            OutlineParameters = outlineParameters;
            return this;
        }

        public GeometricPath SetInitialSvgPath(string svgPath)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _skiaPath = SKPath.ParseSvgPathData(svgPath);
            return this;
        }

        public GeometricPath SetInitialShouldApplyScaling(bool shouldApplyScaling)
        {
            if (base.IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            ShouldApplyScaling = shouldApplyScaling;
            return this;
        }
        #endregion //Builder

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