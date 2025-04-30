using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Navigator;
using CatUI.Data.Theming;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Helpers.Navigation;
using CatUI.Utils;
using CatUISample.UI.Pages;
using CatUISample.UI.Pages.Layout;
using CatUISample.UI.Theming;

namespace CatUISample.UI
{
    public class RootElement : RowContainer
    {
        protected override void EnterDocument(object sender)
        {
            ObjectRef<Navigator> navigatorRef = new();
            Document!.BackgroundColor = CatTheme.Colors.Surface;

            ThemeOverride = RootTheme.GetTheme();
            Children =
            [
                new Sidebar(navigatorRef),
                new Navigator(
                    new Dictionary<string, Func<NavArgs?, NavRoute>>
                    {
                        { "/", _ => new NavRoute(new MainPage()) },
                        { "/Layout/RowContainer", _ => new NavRoute(new RowContainerExamples()) },
                        { "/Layout/ScrollContainer", _ => new NavRoute(new ScrollContainerExamples()) }
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
