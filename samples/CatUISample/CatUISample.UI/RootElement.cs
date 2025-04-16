using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Navigator;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Helpers.Navigation;
using CatUI.Elements.Utils;
using CatUI.Utils;
using CatUISample.UI.Pages;
using CatUISample.UI.Pages.Layout;

namespace CatUISample.UI
{
    public class RootElement : RowContainer
    {
        protected override void EnterDocument(object sender)
        {
            ObjectRef<Navigator> navigatorRef = new();
            Document!.BackgroundColor = CatTheme.Colors.Surface;

            Children =
            [
                //sidebar
                new ColumnContainer
                {
                    Id = "Sidebar",
                    Layout = new ElementLayout().SetFixedWidth(250).SetFixedHeight("100%"),
                    Background = new ColorBrush(CatTheme.Colors.SurfaceContainer),
                    Children =
                    [
                        new Button("Main page", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(40),
                            Background = new ColorBrush(CatTheme.Colors.Primary),
                            OnClick = (_, _) => navigatorRef.Value?.Navigate("/")
                            // InitializationFunction = e =>
                            //     El.Style(
                            //         e,
                            //         (state, el) =>
                            //         {
                            //             switch (state)
                            //             {
                            //                 default:
                            //                     el.Background = new ColorBrush(CatTheme.Colors.Primary);
                            //                     break;
                            //                 case "hover":
                            //                     el.Background = new ColorBrush(CatTheme.Colors.Tertiary);
                            //                     break;
                            //             }
                            //         })
                        },
                        new HorizontalDivider(1, new ColorBrush(Color.Default)),
                        new Button("Layout", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(40),
                            Background = new ColorBrush(CatTheme.Colors.Primary),
                            OnClick = (_, _) => navigatorRef.Value?.Navigate("/layout/rowContainers")
                        }
                    ]
                },
                new Navigator(
                    new Dictionary<string, Func<NavArgs?, NavRoute>>
                    {
                        { "/", _ => new NavRoute(new MainPage()) },
                        { "/layout/rowContainers", _ => new NavRoute(new RowContainersExamples()) }
                    },
                    "/")
                {
                    Ref = navigatorRef,
                    Layout =
                        new ElementLayout()
                            .SetMinMaxWidth(0, Dimension.Unset, true)
                            .SetFixedHeight("100%"),
                    ElementContainerSizing = new RowContainerSizing()
                }
            ];
        }
    }
}
