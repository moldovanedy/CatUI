using System;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Media;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.Platform.CommonInterface;
using CatUI.Platform.NativeUI;
using CatUI.Windowing.DesktopApp;

namespace CatUISample.UI.Pages.UiElements
{
    public class ButtonsExample : ScrollContainer
    {
        public ButtonsExample()
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
                            new Label("Buttons examples", TextAlignmentType.Center)
                            {
                                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                                FontSize = 32,
                                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                            },
                            new CheckBox(
                                CheckBox.CheckBoxState.Indeterminate,
                                "Checkbox 1",
                                16,
                                new ColorBrush(CatTheme.Colors.OnSurface))
                            {
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 100, "100%")
                                        .SetFixedHeight(20)
                            },
                            new RadioButton(
                                true,
                                "Radio button 1",
                                16,
                                new ColorBrush(CatTheme.Colors.OnSurface))
                            {
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 100, "100%")
                                        .SetFixedHeight(20),
                                //TESTING: native alerts
                                OnClick = async void (_, _) =>
                                {
                                    try
                                    {
                                        var alert = new NativeAlert(
                                            "Alert title",
                                            "Alert message: this can be a slightly longer string, giving more details, but avoid very long strings.");

                                        INativeAlert.Button? result =
                                            await alert.OpenAsync(
                                                INativeAlert.Button.Ok);
                                        CatLogger.LogDebug(result?.ToString() ?? "NO RESULT");
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogException(ex);
                                    }
                                }
                            },
                            new SwitchButton(
                                true,
                                "Switch button 1",
                                16,
                                new ColorBrush(CatTheme.Colors.OnSurface))
                            {
                                Layout =
                                    new ElementLayout()
                                        .SetMinMaxAndPreferredWidth("100%", 100, "100%")
                                        .SetFixedHeight(20)
                            },
                            //TESTING: on X11 and Windows it should display the window icon
                            new ImageView
                            {
                                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                ImageFit = ImageFitType.CanShrinkAndGrow,
                                OnEnterDocument = el =>
                                {
                                    if (el is not ImageView imgView)
                                    {
                                        return;
                                    }

                                    ImageAsset? imgAsset =
                                        (imgView.Document?.GetWindowData()?.CatWindow as DesktopWindow)
                                        ?.GetWindowIcon()
                                        ?.Icon512X512;
                                    imgView.Source = imgAsset;
                                }
                            }
                        ]
                    }
                ]
            };
        }
    }
}
