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

        public Ellipse(
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

            Rect rect = Bounds.GetContentBox();
            Document?.Renderer.DrawEllipse(
                new Point2D(rect.CenterX, rect.CenterY),
                rect.Width / 2f,
                rect.Height / 2f,
                FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawEllipseOutline(
                new Point2D(rect.CenterX, rect.CenterY),
                rect.Width / 2f,
                rect.Height / 2f,
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
                PreferredWidth = PreferredWidth,
                PreferredHeight = PreferredHeight,
                MinWidth = MinWidth,
                MinHeight = MinHeight,
                MaxWidth = MaxWidth,
                MaxHeight = MaxHeight,
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
