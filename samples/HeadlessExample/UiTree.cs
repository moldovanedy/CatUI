using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Elements;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Text;

namespace HeadlessExample;

public class UiTree
{
    private readonly ObservableProperty<string> _snapshotText = new();

    public void SetText(string text)
    {
        _snapshotText.Value = text;
    }

    public ColumnContainer GetRoot()
    {
        return new ColumnContainer
        {
            HorizontalAlignment = HorizontalAlignmentType.Center,
            Children =
            [
                new RowContainer
                {
                    Layout = new ElementLayout().SetFixedWidth(120).SetFixedHeight(50),
                    VerticalAlignment = VerticalAlignmentType.Center,
                    Children =
                    [
                        new Element
                        {
                            Background = new ColorBrush(new Color(0x00_40_80)),
                            Children =
                            [
                                new Label("PNG test", TextAlignmentType.Center)
                                {
                                    Layout = new ElementLayout().SetFixedWidth(120),
                                    FontSize = 20,
                                    TextBrush = new ColorBrush(new Color(0xff_ff_ff))
                                }
                            ]
                        }
                    ]
                },
                new RowContainer
                {
                    Layout = new ElementLayout().SetFixedWidth(500).SetFixedHeight(200),
                    VerticalAlignment = VerticalAlignmentType.Center,
                    Children =
                    [
                        new Element
                        {
                            Children =
                            [
                                new Label("PNG test", TextAlignmentType.Center)
                                {
                                    InitializationFunction = e =>
                                    {
                                        if (e is Label el)
                                        {
                                            el.TextProperty.BindUnidirectional(_snapshotText);
                                        }
                                    },
                                    Layout = new ElementLayout().SetFixedWidth(500),
                                    FontSize = 60,
                                    TextBrush = new ColorBrush(new Color(0))
                                }
                            ]
                        }
                    ]
                }
            ]
        };
    }
}
