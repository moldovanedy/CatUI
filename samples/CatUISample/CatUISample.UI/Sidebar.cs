using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Helpers.Navigation;
using CatUI.Elements.Utils;
using CatUI.Utils;

namespace CatUISample.UI;

public class Sidebar : ColumnContainer
{
    private readonly List<(string, string)> _entries =
    [
        ("Main page", "/"),
        ("Layout - RowContainer", "/Layout/RowContainer"),
        ("Layout - ScrollContainer", "/Layout/ScrollContainer"),
        ("UI Elements - Buttons", "/UiElements/Buttons"),
        ("UI Elements - Text fields", "/UiElements/Input/TextFields"),
        ("Native UI - File picking", "/NativeUi/FilePicking"),
        ("Misc - Cursors", "/Misc/Cursors"),
        ("Misc - Animation", "/Misc/Animation")
    ];

    public Sidebar(ObjectRef<Navigator> navigatorRef)
    {
        Layout = new ElementLayout().SetFixedWidth(250).SetFixedHeight("100%");
        Background = new ColorBrush(CatTheme.Colors.SurfaceContainer);

        foreach ((string, string) entry in _entries)
        {
            Children.Add(
                new Button(entry.Item1, 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(40),
                    StyleClass = "MenuButtons",
                    OnClick = (_, _) => navigatorRef.Value?.Navigate(entry.Item2),
                    Cursor = CursorIcon.CURSOR_POINTING_HAND
                });
            Children.Add(new HorizontalDivider(1, new ColorBrush(Color.Default)));
        }
    }
}
