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
using CatUISample.UI.Views;
using CatUISample.UI.Views.Layout;
using CatUISample.UI.Views.Misc;
using CatUISample.UI.Views.NativeUi;
using CatUISample.UI.Views.UiElements;
using CatUISample.UI.Views.UiElements.Input;
using CatUISample.UI.Theming;

namespace CatUISample.UI;

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
                    { "/", _ => new NavRoute(new MainView()) },
                    { "/Layout/RowContainer", _ => new NavRoute(new RowContainerExamples()) },
                    { "/Layout/ScrollContainer", _ => new NavRoute(new ScrollContainerExamples()) },
                    { "/UiElements/Buttons", _ => new NavRoute(new ButtonsExample()) },
                    { "/UiElements/Input/TextFields", _ => new NavRoute(new TextFieldsExample()) },
                    { "/NativeUi/FilePicking", _ => new NavRoute(new FilePickingExamples()) },
                    { "/Misc/Cursors", _ => new NavRoute(new CursorExamples()) },
                    { "/Misc/Animation", _ => new NavRoute(new AnimationExamples()) },
                    { "/Misc/Canvas", _ => new NavRoute(new CanvasExamples()) },
                    { "/Misc/Internationalization", _ => new NavRoute(new InternationalizationExamples()) }
                },
                "/")
            {
                Ref = navigatorRef,
                Layout =
                    new ElementLayout()
                        .SetMinMaxAndPreferredWidth(Dimension.Unset, 0, Dimension.Unset)
                        .SetFixedHeight("100%"),
                ElementContainerSizing = new RowContainerSizing()
            }
        ];
    }
}
