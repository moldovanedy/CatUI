using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;

namespace CatUISample.UI.Pages.Misc
{
    public class CursorExamples : ScrollContainer
    {
        private readonly List<int> _cursors =
        [
            CursorIcon.CURSOR_ARROW,
            CursorIcon.CURSOR_CROSSHAIR,
            CursorIcon.CURSOR_ALL_RESIZE,
            CursorIcon.CURSOR_NESW_RESIZE,
            CursorIcon.CURSOR_NOT_ALLOWED,
            CursorIcon.CURSOR_POINTING_HAND,
            CursorIcon.CURSOR_TEXT
        ];

        public CursorExamples()
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
                            new Label("Cursor examples", TextAlignmentType.Center)
                            {
                                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                                FontSize = 32,
                                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                            },
                            new Button("Set random cursor", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = new ElementLayout().SetFixedWidth(150).SetFixedHeight(32),
                                OnClick = (_, _) =>
                                {
                                    Document?.CursorManager?.SetPersistentCursor(
                                        _cursors[Random.Shared.Next(0, _cursors.Count)]);
                                }
                            }
                        ]
                    }
                ]
            };
        }
    }
}
