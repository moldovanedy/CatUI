using System.Numerics;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Themes;
using CatUI.RenderingEngine.GraphicsCaching;
using SkiaSharp;

namespace CatUI.Elements.Shapes
{
    public class GeometricPath : AbstractShape
    {
        private SKPath _skiaPath = new();
        private SKPath _scaledCachedPath = new();

        /// <summary>
        /// If true, the path will be scaled to respect the element's width and height. If false, no scaling will be applied,
        /// but the path might exceed the element's bounds (width and height).
        /// </summary>
        public bool ShouldApplyScaling
        {
            get => _shouldApplyScaling;
            set
            {
                _shouldApplyScaling = value;
                ShouldApplyScalingProperty.Value = value;
            }
        }

        private bool _shouldApplyScaling;

        public ObservableProperty<bool> ShouldApplyScalingProperty { get; } = new();

        public string SvgPath
        {
            get => _svgPath;
            set
            {
                _svgPath = value;
                if (!string.IsNullOrEmpty(_svgPath))
                {
                    _skiaPath = SKPath.ParseSvgPathData(_svgPath);
                    _scaledCachedPath = new SKPath(_skiaPath);
                    PathCache.CacheNewPath(_scaledCachedPath);
                }

                SvgPathProperty.Value = value;
            }
        }

        private string _svgPath = "";
        public ObservableProperty<string> SvgPathProperty { get; } = new();

        private Vector2 _lastTopLeftPoint = Vector2.Zero;
        private Vector2 _lastScale = Vector2.One;

        public GeometricPath(ThemeDefinition<ElementThemeData>? themeOverrides = null)
            : base(themeOverrides)
        {
        }

        ~GeometricPath()
        {
            PathCache.RemovePath(_scaledCachedPath);
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

            PathCache.RemovePath(_scaledCachedPath);
            _skiaPath = path;
            _scaledCachedPath = new SKPath(_skiaPath);
            PathCache.CacheNewPath(_scaledCachedPath);
        }

        /// <summary>
        /// Will reset the current path to the newly given SVG data from the SVG <code>path</code> element.
        /// </summary>
        /// <param name="svgPath">The SVG data from the SVG <code>path</code> element.</param>
        public void RecreateFromSvgPath(string svgPath)
        {
            PathCache.RemovePath(_scaledCachedPath);
            _skiaPath = SKPath.ParseSvgPathData(svgPath);
            _scaledCachedPath = new SKPath(_skiaPath);
            PathCache.CacheNewPath(_scaledCachedPath);
        }

        protected override void DrawBackground()
        {
            if (!Visible)
            {
                return;
            }

            ElementThemeData currentTheme =
                GetElementFinalThemeData<ElementThemeData>(ElementThemeData.STYLE_NORMAL) ??
                new ElementThemeData().GetDefaultData(ElementThemeData.STYLE_NORMAL);

            IBrush? bgBrush = currentTheme.Background;
            if (bgBrush == null)
            {
                return;
            }

            if (!bgBrush.IsSkippable)
            {
                Document?.Renderer.DrawRect(Bounds.GetPaddingBox(), bgBrush);
            }

            var startPoint = new Vector2(_skiaPath.TightBounds.Left, _skiaPath.TightBounds.Top);
            var scale = new Vector2(1, 1);

            //TODO: use caching as much as possible, and investigate whether saving the canvas state,
            // transforming the whole canvas, drawing and then restoring the canvas is a better choice when there are many paths
            _scaledCachedPath = new SKPath(_skiaPath);

            if (ShouldApplyScaling)
            {
                scale = new Vector2(
                    Bounds.Width / _skiaPath.TightBounds.Width,
                    Bounds.Height / _skiaPath.TightBounds.Height);
            }

            var thisTopLeftPoint = new Vector2(
                Bounds.StartPoint.X - (startPoint.X * scale.X),
                Bounds.StartPoint.Y - (startPoint.Y * scale.Y));

            _scaledCachedPath.Transform(SKMatrix.CreateScaleTranslation(
                scale.X, scale.Y, thisTopLeftPoint.X, thisTopLeftPoint.Y));
            Document?.Renderer.DrawPath(_scaledCachedPath, FillBrush, OutlineBrush, OutlineParameters);

            _lastScale = scale;
        }
    }
}
