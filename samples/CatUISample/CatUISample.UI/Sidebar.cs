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

namespace CatUISample.UI
{
    public class Sidebar : ColumnContainer
    {
        private readonly ObjectRef<Navigator> _navigatorRef;

        private readonly List<(string, string)> _entries =
        [
            ("Main page", "/"),
            ("Layout - RowContainer", "/Layout/RowContainer"),
            ("Layout - ScrollContainer", "/Layout/ScrollContainer")
        ];

        public Sidebar(ObjectRef<Navigator> navigatorRef)
        {
            _navigatorRef = navigatorRef;

            Layout = new ElementLayout().SetFixedWidth(250).SetFixedHeight("100%");
            Background = new ColorBrush(CatTheme.Colors.SurfaceContainer);
        }

        protected override void EnterDocument(object sender)
        {
            foreach ((string, string) entry in _entries)
            {
                Children.Add(
                    new Button(entry.Item1, 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(40),
                        Background = new ColorBrush(CatTheme.Colors.Primary),
                        OnClick = (_, _) => _navigatorRef.Value?.Navigate(entry.Item2)
                    });
                Children.Add(new HorizontalDivider(1, new ColorBrush(Color.Default)));
            }
        }
    }
}
