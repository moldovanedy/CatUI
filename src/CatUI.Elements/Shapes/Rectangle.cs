using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public class Rectangle : AbstractShape
    {
        public Rectangle(
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                fillBrush,
                outlineBrush,
                themeOverrides,
                preferredWidth,
                preferredHeight)
        {
        }

        public Rectangle(
            Rect rectDescriptor,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null)
        {
            if (fillBrush != null)
            {
                FillBrush = fillBrush;
            }

            if (outlineBrush != null)
            {
                OutlineBrush = outlineBrush;
            }

            if (rectDescriptor.X != 0)
            {
                Position = new Dimension2(rectDescriptor.X, Position.Y);
            }

            if (rectDescriptor.Y != 0)
            {
                Position = new Dimension2(Position.X, rectDescriptor.Y);
            }

            if (rectDescriptor.Width != 0)
            {
                PreferredWidth = new Dimension(rectDescriptor.Width);
            }

            if (rectDescriptor.Height != 0)
            {
                PreferredHeight = new Dimension(rectDescriptor.Height);
            }
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
            Document?.Renderer.DrawRect(rect, FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawRectOutline(rect, OutlineBrush, OutlineParameters);
        }
    }
}
