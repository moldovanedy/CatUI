using System.Numerics;
using CatUI.Data;
using CatUI.Data.Brushes;
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
        /// but the path might exceed the element's bounds (width and height). The default value is false.
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

        public ObservableProperty<bool> ShouldApplyScalingProperty { get; } = new(false);

        /// <summary>
        /// The path's string description in the Scalable Vector Graphics (SVG) format. The only relevant element from an
        /// SVG object is its &lt;path&gt; "d" attribute; you can use that here. The default value is an empty string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// While you can create the description manually, SVGs are generally created with a vector-based editing software
        /// like Inkscape. Remember you can only use the data from the "d" attribute of the &lt;path&gt; element.
        /// For full-fledged SVG support, consult other packages that create SkiaSharp objects from SVG objects.
        /// </para>
        /// <para>
        /// If you prefer a more straightforward approach when you create paths, see <see cref="SetNewSkiaPath"/>, that accepts
        /// a <see cref="SKPath"/> object where you can use methods like <see cref="SKPath.AddPoly"/> or
        /// <see cref="SKPath.AddPath(SkiaSharp.SKPath,float,float,SkiaSharp.SKPathAddMode)"/> to create a path in a
        /// more readable way than using this string format.
        /// </para>
        /// </remarks>
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
        public ObservableProperty<string> SvgPathProperty { get; } = new("");

        private Vector2 _lastTopLeftPoint = Vector2.Zero;
        private Vector2 _lastScale = Vector2.One;

        public GeometricPath(
            string svgPath = "",
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                fillBrush,
                outlineBrush,
                preferredWidth,
                preferredHeight)
        {
            SvgPath = svgPath;
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
        /// Will reset the current path to the newly given SVG data from the SVG &lt;path&gt; element.
        /// </summary>
        /// <param name="svgPath">The SVG data from the SVG &lt;path&gt; element.</param>
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

            base.DrawBackground();
            var startPoint = new Vector2(_skiaPath.TightBounds.Left, _skiaPath.TightBounds.Top);
            var scale = new Vector2(1, 1);

            //TODO: use caching as much as possible, and investigate whether saving the canvas state,
            // transforming the whole canvas, drawing and then restoring the canvas is a better choice when there are many paths
            // another strategy would be to apply the inverse of the matrix to the drawn path
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
