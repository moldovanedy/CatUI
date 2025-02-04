using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Shapes
{
    public class Ellipse : AbstractShape
    {
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
    }
}
