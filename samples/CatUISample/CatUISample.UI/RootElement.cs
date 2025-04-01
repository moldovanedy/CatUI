using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Navigator;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers;
using CatUI.Elements.Helpers.Navigation;
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
            Document!.BackgroundColor = new Color(0x21_21_21);

            Children =
            [
                //sidebar
                new ColumnContainer
                {
                    Layout = new ElementLayout().SetFixedWidth(250).SetFixedHeight("100%"),
                    Background = new ColorBrush(new Color(0x31_31_31)),
                    Children =
                    [
                        new Button("Main page", 16, new ColorBrush(new Color(0xff_ff_ff)))
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(40),
                            Background = new ColorBrush(new Color(0x61_61_61)),
                            OnClick = (_, _) => navigatorRef.Value?.Navigate("/")
                        },
                        new Button("Layout", 16, new ColorBrush(new Color(0xff_ff_ff)))
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(40),
                            Background = new ColorBrush(new Color(0x61_61_61)),
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
