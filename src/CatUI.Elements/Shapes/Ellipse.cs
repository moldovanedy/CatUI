using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Shapes
{
    public class Ellipse : AbstractShape
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<Ellipse>? Ref
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

        private ObjectRef<Ellipse>? _ref;

        public Ellipse(IBrush? fillBrush = null, IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
        }

        protected override void DrawBackground()
        {
            if (!Visible)
            {
                return;
            }

            if (FillBrush.IsSkippable)
            {
                return;
            }

            Document?.Renderer.DrawEllipse(
                new Point2D(Bounds.CenterX, Bounds.CenterY),
                Bounds.Width / 2f,
                Bounds.Height / 2f,
                FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawEllipseOutline(
                new Point2D(Bounds.CenterX, Bounds.CenterY),
                Bounds.Width / 2f,
                Bounds.Height / 2f,
                OutlineBrush,
                OutlineParameters);
        }

        public override Ellipse Duplicate()
        {
            return new Ellipse
            {
                FillBrush = FillBrush.Duplicate(),
                OutlineBrush = OutlineBrush.Duplicate(),
                OutlineParameters = OutlineParameters,
                //
                Position = Position,
                Margin = Margin,
                Background = Background.Duplicate(),
                CornerRadius = CornerRadius,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate()
            };
        }
    }
}
