using CatUI.Data;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Shapes
{
    public class Rectangle : AbstractShape
    {
        public Rectangle(ThemeDefinition<ElementThemeData>? themeOverrides = null)
            : base(themeOverrides)
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
            Document?.Renderer.DrawRect(rect, FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawRectOutline(rect, OutlineBrush, OutlineParameters);
        }
    }
}
