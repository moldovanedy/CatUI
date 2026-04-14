using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Data.Theming;
using CatUI.Elements;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Input;
using CatUI.Elements.Media;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;

namespace CatUISample.UI.Views;

public class MainView : ColumnContainer
{
    public MainView()
    {
        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");
        Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Start, 20);
    }

    protected override void EnterDocument(object sender)
    {
        base.EnterDocument(sender);

        FontAsset? fontAsset =
            AssetsManager.LoadFromAssemblyAsync<FontAsset>("/Assets/Fonts/NotoSans-Bold.ttf").Result;
        FontAsset? cursiveFontAsset =
            AssetsManager.LoadFromAssemblyAsync<FontAsset>("/Assets/Fonts/Babbler-Regular.ttf").Result;
        ImageAsset? imageAsset =
            AssetsManager.LoadFromAssemblyAsync<ImageAsset>("/Assets/Images/cat_image.png").Result;

        if (fontAsset == null || cursiveFontAsset == null || imageAsset == null)
        {
            CatLogger.LogError("CatUISample: One or more assets could not be retrieved in MainView");
            return;
        }

        Children =
        [
            new Label("CatUI Sample", TextAlignmentType.Center)
            {
                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                FontSize = 32,
                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
            },
            new RowContainer
            {
                Arrangement = new LinearArrangement(LinearArrangement.JustificationType.SpaceEvenly, 0),
                VerticalAlignment = VerticalAlignmentType.Center,
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredWidth("100%", 0, "100%")
                        .SetFixedHeight(36),
                Children =
                [
                    new Button("Button 1", 16) { Layout = new ElementLayout().SetFixedWidth(120).SetFixedHeight(28) },
                    new Button("Button 2", 18) { Layout = new ElementLayout().SetFixedWidth(120).SetFixedHeight(32) },
                    new Button("Button 3", 22) { Layout = new ElementLayout().SetFixedWidth(120).SetFixedHeight(36) }
                ]
            },
            new RowContainer
            {
                Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Start, 10),
                VerticalAlignment = VerticalAlignmentType.Center,
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredWidth("100%", 0, "100%")
                        .SetFixedHeight(28),
                Children =
                [
                    new Element(),
                    new CheckBox(
                        CheckBox.CheckBoxState.Checked,
                        "Checkbox",
                        20,
                        new ColorBrush(CatTheme.Colors.OnSurface))
                    {
                        Layout = new ElementLayout().SetFixedWidth(160).SetFixedHeight(28),
                        VerticalAlignment = VerticalAlignmentType.Center
                    },
                    new RadioButton(
                        true,
                        "Radio button",
                        20,
                        new ColorBrush(CatTheme.Colors.OnSurface))
                    {
                        Layout = new ElementLayout().SetFixedWidth(160).SetFixedHeight(28),
                        VerticalAlignment = VerticalAlignmentType.Center
                    },
                    new SwitchButton(
                        true,
                        "Switch",
                        20,
                        new ColorBrush(CatTheme.Colors.OnSurface))
                    {
                        Layout = new ElementLayout().SetFixedWidth(160).SetFixedHeight(28),
                        VerticalAlignment = VerticalAlignmentType.Center
                    }
                ]
            },
            new RowContainer
            {
                Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Start, 10),
                VerticalAlignment = VerticalAlignmentType.Center,
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredWidth("100%", 0, "100%")
                        .SetFixedHeight(110),
                Children =
                [
                    new Element(),
                    new Label("The quick brown fox jumps over the lazy dog")
                    {
                        Layout =
                            new ElementLayout()
                                .SetFixedWidth(230)
                                .SetMinMaxAndPreferredHeight(20, 20, 200),
                        Background = new ColorBrush(new Color("#424242")),
                        FontSize = 22,
                        WordWrap = true
                    },
                    new Label("Text with manual hy\u00adphe\u00adna\u00adtion and large line height")
                    {
                        Layout =
                            new ElementLayout()
                                .SetFixedWidth(190)
                                .SetMinMaxAndPreferredHeight(20, 20, 200),
                        Background = new ColorBrush(new Color("#424242")),
                        FontSize = 18,
                        LineHeight = 2.25f,
                        WordWrap = true
                    },
                    new Label("Colorful!")
                    {
                        Layout = new ElementLayout().SetFixedWidth(120),
                        TextBrush = new ColorBrush(new Color("#ff4040")),
                        Font = fontAsset,
                        FontSize = 22
                    },
                    new Label("Font test")
                    {
                        Layout = new ElementLayout().SetFixedWidth(120), Font = cursiveFontAsset, FontSize = 24
                    }
                ]
            },
            new RowContainer
            {
                Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Start, 10),
                VerticalAlignment = VerticalAlignmentType.Center,
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredWidth("100%", 0, "100%")
                        .SetFixedHeight(250),
                Children =
                [
                    new Element(),
                    new EllipseElement(
                        new Point2D(50, 40), 40, 20,
                        new ColorBrush(new Color("#ff0000C0")),
                        new ColorBrush(new Color("#ffff0080")))
                    {
                        OutlineParameters = new OutlineParams(10f), ClipType = ClipApplicability.None
                    },
                    new GeometricPathElement(
                        "M0,0.054V20h21V0.054H0z M15.422,18.129l-5.264-2.768l-5.265,2.768l1.006-5.863L1.64,8.114l5.887-0.855l2.632-5.334l2.633,5.334l5.885,0.855l-4.258,4.152L15.422,18.129z",
                        new ColorBrush(new Color(0xff_98_00)),
                        new ColorBrush(new Color(0x21_96_f3)))
                    {
                        Position = "5 10",
                        Layout =
                            new ElementLayout().SetFixedWidth(80).SetFixedHeight(60),
                        Background = new ColorBrush(new Color(0xff_ff_ff)),
                        ShouldApplyScaling = true,
                        OutlineParameters = new OutlineParams(
                            2,
                            LineCapType.Round,
                            miterLimit: 5)
                    },
                    new ImageView(imageAsset)
                    {
                        Layout = new ElementLayout().SetFixedWidth(240).SetFixedHeight(240),
                        ShouldKeepAspectRatio = true
                    }
                ]
            },
            new RowContainer
            {
                Arrangement = new LinearArrangement(LinearArrangement.JustificationType.Start, 15),
                VerticalAlignment = VerticalAlignmentType.Center,
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredWidth("100%", 0, "100%")
                        .SetFixedHeight(40),
                Children =
                [
                    new Element(),
                    new Label("Text field:"),
                    new TextField
                    {
                        Layout =
                            new ElementLayout()
                                .SetMinMaxAndPreferredWidth(250, 250, Dimension.Unset)
                                .SetFixedHeight(32)
                    }
                ]
            }
        ];
    }
}
