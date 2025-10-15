using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;

namespace CatUISample.UI.Pages;

public class MainPage : ColumnContainer
{
    public MainPage()
    {
        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");
    }

    protected override void EnterDocument(object sender)
    {
        base.EnterDocument(sender);

        Children =
        [
            new Label("CatUI Sample", TextAlignmentType.Center)
            {
                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                FontSize = 32,
                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
            },
            new RoundedRectangleElement(
                new ColorBrush(new Color(0xff_00_00)),
                new ColorBrush(new Color(0xff_ff_ff)))
            {
                Layout = new ElementLayout().SetFixedWidth("50%").SetFixedHeight("20%"),
                OutlineParameters = new OutlineParams(10f),
                RoundCornersDescriptor = new CornerInset
                {
                    TopLeftRadius = 10,
                    TopRightRadius = 15,
                    BottomRightEllipse = new Dimension2(20, 10),
                    BottomLeftEllipse = new Dimension2(25, 15)
                }
            }
        ];
    }
}
