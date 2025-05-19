using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;

namespace CatUI.Elements.Utils
{
    /// <summary>
    /// An element that displays a line that visually separates other elements, generally used in menus and containers.
    /// It is similar with <see cref="Spacer"/>, but this element also displays a line for separation.
    /// Don't set the <see cref="Element.Layout"/> on the opposite axis of you want to use the divider (i.e. don't set
    /// the height in <see cref="HorizontalDivider"/> and the width in <see cref="VerticalDivider"/>) and instead use
    /// <see cref="LineThickness"/> and the spacing properties.
    /// </summary>
    /// <remarks>By default, this sets the <see cref="Element.Layout"/> on the axis of orientation (i.e. width for
    /// <see cref="HorizontalDivider"/> and height for <see cref="VerticalDivider"/>) as "100%", but you can override
    /// this by resetting the corresponding property on the <see cref="Element.Layout"/>.
    /// </remarks>
    public abstract class Divider : Element
    {
        /// <summary>
        /// The orientation of the line (horizontal or vertical).
        /// </summary>
        public Orientation LineOrientation { get; protected set; }

        /// <summary>
        /// The line thickness in <see cref="Unit.Dp"/>. The default value is 2.
        /// </summary>
        public float LineThickness
        {
            get => _lineThickness;
            set => LineThicknessProperty.Value = value;
        }

        private float _lineThickness = 2;

        public ObservableProperty<float> LineThicknessProperty { get; } = new(2);

        private void SetLineThickness(float value)
        {
            _lineThickness = value;
            SetLocalValue(nameof(LineThickness), value);
            ResetLayout();
        }

        /// <summary>
        /// Sets the brush used to paint the line. The default value is a <see cref="ColorBrush"/> with a black color.
        /// </summary>
        public IBrush LineBrush
        {
            get => _lineBrush;
            set
            {
                if (value != _lineBrush)
                {
                    LineBrushProperty.Value = value;
                }
            }
        }

        private IBrush _lineBrush = new ColorBrush(new Color(0));

        public ObservableProperty<IBrush> LineBrushProperty { get; } = new(new ColorBrush(new Color(0)));

        private void SetLineBrush(IBrush? value)
        {
            _lineBrush = value ?? new ColorBrush(new Color(0));
            SetLocalValue(nameof(LineBrush), value);
            Document?.MarkVisualDirty();
        }

        /// <summary>
        /// Sets the line cap (i.e. how are the line ends drawn). The default value is <see cref="LineCapType.Butt"/>.
        /// </summary>
        public LineCapType LineCap
        {
            get => _lineCap;
            set
            {
                if (value != _lineCap)
                {
                    LineCapProperty.Value = value;
                }
            }
        }

        private LineCapType _lineCap = LineCapType.Butt;

        public ObservableProperty<LineCapType> LineCapProperty { get; } = new(LineCapType.Butt);

        private void SetLineCap(LineCapType value)
        {
            _lineCap = value;
            SetLocalValue(nameof(LineCap), value);
            Document?.MarkVisualDirty();
        }

        /// <summary>
        /// Represents the spacing on the opposite axis of orientation. The first value is top or left, the second value is
        /// bottom or right. The values are in <see cref="Unit.Dp"/>.
        /// </summary>
        protected ValueTuple<float, float> Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                ResetLayout();
            }
        }

        private ValueTuple<float, float> _spacing = (0, 0);

        /// <summary>
        /// Represents the spacing on the axis of orientation (i.e. the spacing between the line end and the
        /// element end). The first value is left or top, the second value is right or bottom.
        /// </summary>
        protected ValueTuple<Dimension, Dimension> LinePadding
        {
            get => _linePadding;
            set
            {
                _linePadding = value;
                Document?.MarkVisualDirty();
            }
        }

        private ValueTuple<Dimension, Dimension> _linePadding = (0, 0);

        protected Divider(Orientation orientation)
        {
            LineThicknessProperty.ValueChangedEvent += SetLineThickness;
            LineBrushProperty.ValueChangedEvent += SetLineBrush;
            LineCapProperty.ValueChangedEvent += SetLineCap;

            LineOrientation = orientation;

            Layout ??= new ElementLayout();
            if (orientation == Orientation.Horizontal)
            {
                Layout.SetFixedWidth("100%");
            }
            else
            {
                Layout.SetFixedHeight("100%");
            }
        }

        //~Divider()
        //{
        //    LineThicknessProperty = null!;
        //    LineBrushProperty = null!;
        //    LineCapProperty = null!;
        //}

        protected override void DrawBackground()
        {
            if (!Visible)
            {
                return;
            }

            base.DrawBackground();

            float actualThickness = CalculateDimension(_lineThickness);
            OutlineParams outlineParams = new(actualThickness, _lineCap);
            float x = Bounds.X, y = Bounds.Y, size;

            if (LineOrientation == Orientation.Horizontal)
            {
                x += CalculateDimension(_linePadding.Item1, Bounds.Width);
                y += CalculateDimension(_spacing.Item1) + (actualThickness / 2f);
                size = Bounds.Width - x - CalculateDimension(_linePadding.Item2, Bounds.Width);

                if (size < 0)
                {
                    return;
                }

                Document?.Renderer.DrawLine(
                    new Point2D(x, y),
                    new Point2D(x + size, y),
                    _lineBrush,
                    outlineParams);
            }
            else
            {
                y += CalculateDimension(_linePadding.Item1, Bounds.Height);
                x += CalculateDimension(_spacing.Item1) + (actualThickness / 2f);
                size = Bounds.Height - y - CalculateDimension(_linePadding.Item2, Bounds.Height);

                if (size < 0)
                {
                    return;
                }

                Document?.Renderer.DrawLine(
                    new Point2D(x, y),
                    new Point2D(x, y + size),
                    _lineBrush,
                    outlineParams);
            }
        }

        private void ResetLayout()
        {
            Layout ??= new ElementLayout();
            if (LineOrientation == Orientation.Horizontal)
            {
                Layout.SetFixedHeight(_spacing.Item1 + _spacing.Item2 + _lineThickness);
            }
            else
            {
                Layout.SetFixedWidth(_spacing.Item1 + _spacing.Item2 + _lineThickness);
            }
        }
    }
}
