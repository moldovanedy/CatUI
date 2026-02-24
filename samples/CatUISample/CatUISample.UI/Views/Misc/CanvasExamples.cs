using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Media;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.RenderingEngine;

namespace CatUISample.UI.Views.Misc;

public class CanvasExamples : ScrollContainer
{
    private float _degrees;

    public CanvasExamples()
    {
        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");

        Content = new PaddingElement(new EdgeInset(0, 5))
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
            Children =
            [
                new ColumnContainer
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                    Arrangement = LinearArrangement.SpacedBy(5),
                    Children =
                    [
                        new Label("Canvas examples", TextAlignmentType.Center)
                        {
                            Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                            FontSize = 32,
                            TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                        },
                        new Canvas
                        {
                            Layout = new ElementLayout().SetFixedWidth(300).SetFixedHeight(300),
                            Background = new ColorBrush(CatTheme.Colors.SurfaceContainer),
                            DrawFunction = OnCanvasDraw
                        }
                    ]
                }
            ]
        };
    }

    private void OnCanvasDraw(CanvasPen pen)
    {
        var center = new Point2D(Bounds.X + 100, Bounds.Y + 150);

        pen.SkiaCanvas?.RotateDegrees(_degrees, center.X, center.Y);
        pen.CanvasRenderer.DrawEllipse(center, 40, 70, new ColorBrush(new Color(0x21_af_d2)));
        pen.SkiaCanvas?.ResetMatrix();

        pen.IsDirty = true;
        _degrees += 2;
    }
}
