using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Shapes
{
    public abstract class AbstractShape : Element
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
                _fillBrush = value;
                FillBrushProperty.Value = _fillBrush;
            }
        }

        private IBrush _fillBrush = new ColorBrush(Color.Default);
        public ObservableProperty<IBrush> FillBrushProperty { get; private set; } = new(new ColorBrush(Color.Default));

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
                _outlineBrush = value;
                OutlineBrushProperty.Value = _outlineBrush;
            }
        }

        private IBrush _outlineBrush = new ColorBrush(Color.Default);

        public ObservableProperty<IBrush> OutlineBrushProperty { get; private set; } =
            new(new ColorBrush(Color.Default));

        /// <summary>
        /// The parameters that describe the outline of the shape. It is irrelevant when <see cref="OutlineBrush"/> is
        /// not drawable (see <see cref="IBrush.IsSkippable"/>). The default value is a new <see cref="OutlineParams"/>
        /// object, consult its documentation to find out the default parameters' values.
        /// </summary>
        public OutlineParams OutlineParameters
        {
            get => _outlineParameters;
            set
            {
                _outlineParameters = value;
                OutlineParametersProperty.Value = _outlineParameters;
            }
        }

        private OutlineParams _outlineParameters = new();

        public ObservableProperty<OutlineParams> OutlineParametersProperty { get; private set; } =
            new(new OutlineParams());

        public AbstractShape(
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
            if (fillBrush != null)
            {
                FillBrush = fillBrush;
            }

            if (outlineBrush != null)
            {
                OutlineBrush = outlineBrush;
            }

            FillBrushProperty.ValueChangedEvent += SetFillBrush;
            OutlineBrushProperty.ValueChangedEvent += SetOutlineBrush;
            OutlineParametersProperty.ValueChangedEvent += SetOutlineParameters;
        }

        ~AbstractShape()
        {
            FillBrushProperty = null!;
            OutlineBrushProperty = null!;
            OutlineParametersProperty = null!;
        }

        private void SetFillBrush(IBrush? brush)
        {
            _fillBrush = brush ?? new ColorBrush(Color.Default);
        }

        private void SetOutlineBrush(IBrush? brush)
        {
            _outlineBrush = brush ?? new ColorBrush(Color.Default);
        }

        private void SetOutlineParameters(OutlineParams outlineParameters)
        {
            _outlineParameters = outlineParameters;
        }
    }
}
