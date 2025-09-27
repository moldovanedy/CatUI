using System;
using System.Numerics;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Shapes;
using CatUI.RenderingEngine.GraphicsCaching;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.Elements.Shapes
{
    /// <summary>
    /// Draws a path (see remarks), either filled, outlined or both. If the path is both filled and outlined, the filled
    /// area will have the size of the element and the outline will exceed the element bounds by half of the outline width
    /// on each size. The outline will also overlap with the filled area by half of the outline width on each side.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the most complex shape element: it can draw polygons, straight lines, curves (quadratic and cubic Bezier),
    /// etc. You can combine multiple paths, and it's not necessary for the path to be continuous. However, remember that
    /// this element can affect performance if overused, especially GPU usage. To set a path, you use the SVG's
    /// &lt;path&gt; syntax or <see cref="SetNewSkiaPath"/> with the methods of <see cref="SKPath"/>.
    /// See <see cref="SvgPath"/> for more info. This object uses Skia's <see cref="SKPath"/> internally.
    /// </para>
    /// <para>
    /// For the best results, try to avoid having lots of these elements visible on the screen at once, or better on the
    /// entire app's lifetime. Any modification to the path is similar to creating a new path, so avoid modifying paths
    /// as well. If you have small objects with a lot of details, consider using images, as images are generally faster
    /// to draw when they are small.
    /// </para>
    /// </remarks>
    public class GeometricPathElement : AbstractShapeElement
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<GeometricPathElement>? Ref
        {
            get => _ref;
            set
            {
                _ref = value;
                if (_ref != null)
                {
                    _ref.Value = this;
                }
            }
        }

        private ObjectRef<GeometricPathElement>? _ref;

        public override ClipShape CorrespondingClipShape => _clipShape;

        private readonly ClipShape _clipShape;

        private SKPath _skiaPath = new();

        /// <summary>
        /// If true, the path will be scaled to respect the element's width and height. If false, no scaling will be applied,
        /// but the path might exceed the element's bounds (width and height). The default value is false.
        /// </summary>
        public bool ShouldApplyScaling
        {
            get => _shouldApplyScaling;
            set => ShouldApplyScalingProperty.Value = value;
        }

        private bool _shouldApplyScaling;
        public ObservableProperty<bool> ShouldApplyScalingProperty { get; } = new(false);

        private void SetShouldApplyScaling(bool value)
        {
            _shouldApplyScaling = value;
            if (_clipShape is PathClipShape pathClipShape)
            {
                pathClipShape.ShouldApplyScaling = value;
            }

            SetLocalValue(nameof(ShouldApplyScaling), value);
            MarkLayoutDirty();
        }

        /// <summary>
        /// If true, all coordinates will be scaled (or rather transformed from pixels to dp) when the path is drawn.
        /// This is highly recommended, as otherwise the path will not respect <see cref="UiDocument.ContentScale"/>
        /// and will look too small on some devices (especially mobile). The default value is true.
        /// </summary>
        public bool ShouldRespectContentScaling
        {
            get => _shouldRespectContentScaling;
            set => ShouldRespectContentScalingProperty.Value = value;
        }

        private bool _shouldRespectContentScaling = true;
        public ObservableProperty<bool> ShouldRespectContentScalingProperty { get; } = new(true);

        private void SetShouldRespectContentScaling(bool value)
        {
            _shouldRespectContentScaling = value;
            SetLocalValue(nameof(ShouldRespectContentScaling), value);
            MarkLayoutDirty();
        }

        /// <summary>
        /// The path's string description in the Scalable Vector Graphics (SVG) format. The only relevant element from an
        /// SVG object is its &lt;path&gt; "d" attribute; you can use that here. All coordinates are relative to the top-left
        /// corner of the element bounds. The default value is an empty string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// While you can create the description manually, SVGs are generally created with vector-based editing software
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
                if (value != _svgPath)
                {
                    SvgPathProperty.Value = value;
                }
            }
        }

        private string _svgPath = "";
        public ObservableProperty<string> SvgPathProperty { get; } = new("");

        private void SetSvgPath(string? value)
        {
            _svgPath = value ?? string.Empty;
            if (!string.IsNullOrEmpty(_svgPath))
            {
                _skiaPath = SKPath.ParseSvgPathData(_svgPath);
                PathCache.CacheNewPath(_skiaPath);
            }

            if (_clipShape is PathClipShape pathClipShape)
            {
                pathClipShape.SvgPath = _svgPath;
            }

            SetLocalValue(nameof(SvgPath), value);
            MarkLayoutDirty();
        }

        public GeometricPathElement(string svgPath = "", IBrush? fillBrush = null, IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
            Init();
            SvgPath = svgPath;
            _clipShape = new PathClipShape();
        }

        public GeometricPathElement(GeometricPathElement other) : base(other)
        {
            Init();
            SvgPath = other.SvgPath;
            _clipShape = new PathClipShape();
        }

        private void Init()
        {
            ShouldApplyScalingProperty.ValueChangedEvent += SetShouldApplyScaling;
            ShouldRespectContentScalingProperty.ValueChangedEvent += SetShouldRespectContentScaling;
            SvgPathProperty.ValueChangedEvent += SetSvgPath;
        }

        /// <summary>
        /// Returns a clone of the internal SKPath object (unscaled even when <see cref="ShouldApplyScaling"/> is true).
        /// </summary>
        /// <returns>A clone of the internal SKPath object.</returns>
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
            PathCache.CacheNewPath(_skiaPath);

            if (_clipShape is PathClipShape pathClipShape)
            {
                pathClipShape.SetNewSkiaPath(path);
            }
        }

        /// <summary>
        /// Will reset the current path to the newly given SVG data from the SVG &lt;path&gt; element.
        /// </summary>
        /// <param name="svgPath">The SVG data from the SVG &lt;path&gt; element.</param>
        public void RecreateFromSvgPath(string svgPath)
        {
            PathCache.RemovePath(_skiaPath);
            _skiaPath = SKPath.ParseSvgPathData(svgPath);
            PathCache.CacheNewPath(_skiaPath);

            if (_clipShape is PathClipShape pathClipShape)
            {
                pathClipShape.SvgPath = svgPath;
            }
        }

        protected override void DrawBackground()
        {
            if (!IsCurrentlyVisible)
            {
                return;
            }

            base.DrawBackground();
            Point2D topLeftPoint = new(Bounds.X, Bounds.Y);
            SKMatrix transformMatrix;
            float contentScale =
                ShouldRespectContentScaling
                    ? Document?.ContentScale ?? 1f
                    : 1f;

            if (ShouldApplyScaling)
            {
                Vector2 scale = new(
                    Bounds.Width / _skiaPath.TightBounds.Width,
                    Bounds.Height / _skiaPath.TightBounds.Height);

                transformMatrix = SKMatrix.CreateScaleTranslation(
                    scale.X * contentScale, scale.Y * contentScale, topLeftPoint.X, topLeftPoint.Y);
            }
            else
            {
                transformMatrix = SKMatrix.CreateScaleTranslation(
                    contentScale, contentScale, topLeftPoint.X, topLeftPoint.Y);
            }

            int? stateCount = Document?.Renderer.SaveCanvasState();
            Document?.Renderer.Canvas?.SetMatrix(transformMatrix);

            Document?.Renderer.DrawPath(_skiaPath, FillBrush);
            Document?.Renderer.DrawPathOutline(_skiaPath, OutlineBrush, OutlineParameters);

            if (stateCount != null)
            {
                Document?.Renderer.RestoreCanvasState(stateCount.Value);
            }
        }

        public override Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            float? parentEnforcedWidth = null,
            float? parentEnforcedHeight = null)
        {
            Point2D absolutePosition = GetAbsolutePositionUtil(parentAbsolutePosition, parentSize);
            Size thisSize = GetDirectSizeUtil(parentSize, parentMaxSize);
            Size thisMaxSize = GetMaxSizeUtil(parentSize);

            //when scaling is applied, ignore the path bounds
            float width =
                parentEnforcedWidth ??
                Math.Max(
                    thisSize.Width,
                    ShouldApplyScaling ? float.NegativeInfinity : _skiaPath.TightBounds.Width);
            float height =
                parentEnforcedHeight ??
                Math.Max(
                    thisSize.Height,
                    ShouldApplyScaling ? float.NegativeInfinity : _skiaPath.TightBounds.Height);

            float maxWidth = parentEnforcedWidth ?? Math.Max(thisMaxSize.Width, width);
            float maxHeight = parentEnforcedHeight ?? Math.Max(thisMaxSize.Height, height);

            thisSize = new Size(width, height);
            thisMaxSize = new Size(maxWidth, maxHeight);
            RecomputeChildrenUtil(thisSize, thisMaxSize, absolutePosition);

            //IMPORTANT: When we consider all the children's bounds, it's important to know that if the bounds are not
            //the expected ones (from thisSize) because an element overflowed, we DON'T recalculate again, but leave
            //all elements as-is. The consequence is that the elements that have layouts in percentages might not really
            //be that exact percentage, so it's "not a bug, it's a feature" which should be stated clearly in the docs
            Bounds = GetFinalBoundsUtil(absolutePosition, thisSize);

            return thisSize;
        }

        public override GeometricPathElement Duplicate()
        {
            var el = new GeometricPathElement(this);
            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
