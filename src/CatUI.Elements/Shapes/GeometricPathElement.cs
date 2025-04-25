using System;
using System.Numerics;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
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
    /// This is the most complex shape element: it can draw polygons, straight lines, curves (quadratic and cubic Bezier)
    /// etc. You can combine multiple paths, and it's not necessary for the path to be continuous. However, remember that
    /// this element can affect performance if overused, especially GPU usage. To set a path, you use the SVG's
    /// &lt;path&gt; syntax or <see cref="SetNewSkiaPath"/> with the methods of <see cref="SKPath"/>.
    /// See <see cref="SvgPath"/> for more info. This object uses Skia's <see cref="SKPath"/> internally.
    /// </para>
    /// <para>
    /// For the best results, try to avoid having lots of these elements visible on the screen at once, or better on the
    /// entire app's lifetime. Any modification to the path is similar to creating a new path, so avoid modifying paths
    /// as well. If you have small objects with a lot of details, consider using images, as images are generally faster
    /// to draw when the images are small.
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
                SetShouldApplyScaling(value);
                ShouldApplyScalingProperty.Value = value;
            }
        }

        private bool _shouldApplyScaling;

        public ObservableProperty<bool> ShouldApplyScalingProperty { get; private set; } = new(false);

        private void SetShouldApplyScaling(bool value)
        {
            _shouldApplyScaling = value;
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
                SetSvgPath(value);
                SvgPathProperty.Value = value;
            }
        }

        private string _svgPath = "";
        public ObservableProperty<string> SvgPathProperty { get; private set; } = new("");

        private void SetSvgPath(string? value)
        {
            _svgPath = value ?? string.Empty;
            if (!string.IsNullOrEmpty(_svgPath))
            {
                _skiaPath = SKPath.ParseSvgPathData(_svgPath);
                _scaledCachedPath = new SKPath(_skiaPath);
                PathCache.CacheNewPath(_scaledCachedPath);
            }

            MarkLayoutDirty();
        }

        private Vector2 _lastTopLeftPoint = Vector2.Zero;
        private SKMatrix _lastTransformMatrix = SKMatrix.Identity;

        public GeometricPathElement(string svgPath = "", IBrush? fillBrush = null, IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
            SvgPath = svgPath;

            ShouldApplyScalingProperty.ValueChangedEvent += SetShouldApplyScaling;
            SvgPathProperty.ValueChangedEvent += SetSvgPath;
        }

        ~GeometricPathElement()
        {
            //this causes crashes
            //PathCache.RemovePath(_scaledCachedPath);

            ShouldApplyScalingProperty = null!;
            SvgPathProperty = null!;
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

            //TODO: investigate whether saving the canvas state, transforming the whole canvas, drawing and then
            // restoring the canvas is a better choice when there are many paths instead of the inverse matrix technique;
            // also look for ways to minimize the transformations, as it might invalidate Skia's caches, leading to poor
            // performance; it's especially important for the case where scaling is not applied, as that's only a translation
            //
            _scaledCachedPath.Transform(_lastTransformMatrix.Invert());
            //Vector2 startPoint = new(_skiaPath.TightBounds.Left, _skiaPath.TightBounds.Top);

            if (ShouldApplyScaling)
            {
                Vector2 scale = new(
                    Bounds.Width / _skiaPath.TightBounds.Width,
                    Bounds.Height / _skiaPath.TightBounds.Height);

                // _lastTopLeftPoint = new Vector2(
                //     Bounds.X - (startPoint.X * scale.X),
                //     Bounds.Y - (startPoint.Y * scale.Y));
                _lastTopLeftPoint = new Vector2(Bounds.X, Bounds.Y);

                _lastTransformMatrix = SKMatrix.CreateScaleTranslation(
                    scale.X, scale.Y, _lastTopLeftPoint.X, _lastTopLeftPoint.Y);
            }
            else
            {
                //_lastTopLeftPoint = new Vector2(Bounds.X - startPoint.X, Bounds.Y - startPoint.Y);
                _lastTopLeftPoint = new Vector2(Bounds.X, Bounds.Y);
                _lastTransformMatrix = SKMatrix.CreateTranslation(_lastTopLeftPoint.X, _lastTopLeftPoint.Y);
            }

            _scaledCachedPath.Transform(_lastTransformMatrix);
            Document?.Renderer.DrawPath(_scaledCachedPath, FillBrush);
            Document?.Renderer.DrawPathOutline(_scaledCachedPath, OutlineBrush, OutlineParameters);
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
                    ShouldApplyScaling ? float.NegativeInfinity : _scaledCachedPath.TightBounds.Width);
            float height =
                parentEnforcedHeight ??
                Math.Max(
                    thisSize.Height,
                    ShouldApplyScaling ? float.NegativeInfinity : _scaledCachedPath.TightBounds.Height);

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
            return new GeometricPathElement
            {
                SvgPath = _svgPath,
                ShouldApplyScaling = _shouldApplyScaling,
                //AbstractShapeElement
                FillBrush = FillBrush.Duplicate(),
                OutlineBrush = OutlineBrush.Duplicate(),
                OutlineParameters = OutlineParameters,
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };
        }
    }
}
