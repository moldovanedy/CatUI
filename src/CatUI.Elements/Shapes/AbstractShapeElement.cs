using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Shapes
{
    /// <summary>
    /// The base class for any shapes that can be drawn directly, like <see cref="RectangleElement"/>, <see cref="EllipseElement"/>
    /// or <see cref="GeometricPathElement"/>.
    /// </summary>
    public abstract class AbstractShapeElement : Element
    {
        /// <summary>
        /// The "brush" used to fill the shape. A brush contains information like the color to use, if it can use an image
        /// as fill etc. The default value is a <see cref="ColorBrush"/> that has a completely transparent color,
        /// meaning it won't draw anything.
        /// </summary>
        public IBrush FillBrush
        {
            get => _fillBrush;
            set
            {
                if (value != _fillBrush)
                {
                    FillBrushProperty.Value = value;
                }
            }
        }

        private IBrush _fillBrush = new ColorBrush(Color.Default);
        public ObservableProperty<IBrush> FillBrushProperty { get; } = new(new ColorBrush(Color.Default));

        private void SetFillBrush(IBrush? value)
        {
            _fillBrush = value ?? new ColorBrush(Color.Default);
            SetLocalValue(nameof(FillBrush), value);
            RequestRedraw();
        }

        /// <summary>
        /// The "brush" used to make an outline (stroke) of the shape. A brush contains information like the color to use,
        /// if it can use an image as fill etc. The default value is a <see cref="ColorBrush"/> that has a completely
        /// transparent color, meaning it won't draw anything.
        /// </summary>
        public IBrush OutlineBrush
        {
            get => _outlineBrush;
            set
            {
                if (value != _outlineBrush)
                {
                    OutlineBrushProperty.Value = value;
                }
            }
        }

        private IBrush _outlineBrush = new ColorBrush(Color.Default);

        public ObservableProperty<IBrush> OutlineBrushProperty { get; } =
            new(new ColorBrush(Color.Default));

        private void SetOutlineBrush(IBrush? value)
        {
            _outlineBrush = value ?? new ColorBrush(Color.Default);
            SetLocalValue(nameof(OutlineBrush), value);
            RequestRedraw();
        }

        /// <summary>
        /// The parameters that describe the outline of the shape. It is irrelevant when <see cref="OutlineBrush"/> is
        /// not drawable (see <see cref="IBrush.IsSkippable"/>). The default value is a new <see cref="OutlineParams"/>
        /// object, consult its documentation to find out the default parameters' values.
        /// </summary>
        public OutlineParams OutlineParameters
        {
            get => _outlineParameters;
            set => OutlineParametersProperty.Value = value;
        }

        private OutlineParams _outlineParameters = new();

        public ObservableProperty<OutlineParams> OutlineParametersProperty { get; } =
            new(new OutlineParams());

        private void SetOutlineParameters(OutlineParams value)
        {
            _outlineParameters = value;
            SetLocalValue(nameof(OutlineParameters), value);
            MarkLayoutDirty();
        }

        public AbstractShapeElement(IBrush? fillBrush = null, IBrush? outlineBrush = null)
        {
            FillBrushProperty.ValueChangedEvent += SetFillBrush;
            OutlineBrushProperty.ValueChangedEvent += SetOutlineBrush;
            OutlineParametersProperty.ValueChangedEvent += SetOutlineParameters;

            if (fillBrush != null)
            {
                FillBrush = fillBrush;
            }

            if (outlineBrush != null)
            {
                OutlineBrush = outlineBrush;
            }
        }

        //~AbstractShapeElement()
        //{
        //    FillBrushProperty = null!;
        //    OutlineBrushProperty = null!;
        //    OutlineParametersProperty = null!;
        //}
    }
}
