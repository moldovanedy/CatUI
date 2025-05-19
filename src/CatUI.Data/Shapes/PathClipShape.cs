using System.Numerics;
using SkiaSharp;

namespace CatUI.Data.Shapes
{
    public class PathClipShape : ClipShape
    {
        /// <summary>
        /// If true, the path will be scaled to respect the element's width and height. If false, no scaling will be applied,
        /// but the path might exceed the element's bounds (width and height). The default value is false.
        /// </summary>
        public bool ShouldApplyScaling { get; set; }

        /// <summary>
        /// The path's string description in the Scalable Vector Graphics (SVG) format. The only relevant element from an
        /// SVG object is its &lt;path&gt; "d" attribute; you can use that here. All coordinates are relative to the top-left
        /// corner of the element bounds. The default value is an empty string.
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
                    SkiaPath = SKPath.ParseSvgPathData(_svgPath);
                    _scaledPath = new SKPath(SkiaPath);
                }
            }
        }

        private string _svgPath = "";

        public SKPath SkiaPath { get; private set; } = new();

        private SKPath _scaledPath = new();

        private Vector2 _lastTopLeftPoint = Vector2.Zero;
        private SKMatrix _lastTransformMatrix = SKMatrix.Identity;

        public void SetNewSkiaPath(SKPath path)
        {
            if (path == SkiaPath)
            {
                return;
            }

            SkiaPath = path;
            _scaledPath = new SKPath(SkiaPath);
        }

        public override bool IsPointInside(Point2D point, Rect bounds, float contentScale, Size viewportSize)
        {
            //we can be confident that if scaling is applied, if the point isn't inside the bounds, it's definitely not
            // inside the path; otherwise we can't assume that because the path might exceed the bounds
            if (ShouldApplyScaling && !Rect.IsPointInside(ref bounds, point))
            {
                return false;
            }

            _scaledPath.Transform(_lastTransformMatrix.Invert());
            var startPoint = new Vector2(SkiaPath.TightBounds.Left, SkiaPath.TightBounds.Top);

            if (ShouldApplyScaling)
            {
                var scale = new Vector2(
                    bounds.Width / SkiaPath.TightBounds.Width,
                    bounds.Height / SkiaPath.TightBounds.Height);

                _lastTopLeftPoint = new Vector2(
                    bounds.X - (startPoint.X * scale.X),
                    bounds.Y - (startPoint.Y * scale.Y));

                _lastTransformMatrix = SKMatrix.CreateScaleTranslation(
                    scale.X, scale.Y, _lastTopLeftPoint.X, _lastTopLeftPoint.Y);
            }
            else
            {
                _lastTopLeftPoint = new Vector2(bounds.X - startPoint.X, bounds.Y - startPoint.Y);
                _lastTransformMatrix = SKMatrix.CreateTranslation(_lastTopLeftPoint.X, _lastTopLeftPoint.Y);
            }

            _scaledPath.Transform(_lastTransformMatrix);
            return _scaledPath.Contains(point.X - bounds.X, point.Y - bounds.Y);
        }

        public override SKPath GetSkiaClipPath(Rect bounds, float contentScale, Size viewportSize)
        {
            return _scaledPath;
        }

        public override PathClipShape Duplicate()
        {
            return new PathClipShape { ShouldApplyScaling = ShouldApplyScaling, SvgPath = SvgPath };
        }
    }
}
