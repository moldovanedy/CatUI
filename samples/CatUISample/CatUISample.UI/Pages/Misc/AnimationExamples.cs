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
using CatUI.Elements.Transitions;
using CatUI.Elements.Transitions.PredefinedTweeners;
using CatUI.Elements.Utils;

namespace CatUISample.UI.Pages.Misc
{
    public class AnimationExamples : ScrollContainer
    {
        private bool _isLarge;

        public AnimationExamples()
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
                            new Label("Animation examples", TextAlignmentType.Center)
                            {
                                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                                FontSize = 32,
                                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                            },
                            new Button("Animate!", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = new ElementLayout().SetFixedWidth(150).SetFixedHeight(48),
                                OnClick = (sender, _) =>
                                {
                                    if (sender is not Button element)
                                    {
                                        return;
                                    }

                                    var tween = new Tween(element)
                                    {
                                        AnimationEasing = new Easing(Easing.EasingType.BackInOut)
                                    };

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    tween.TweenPropertyAsync(
                                        el => ((el as Button)!.TextElement as Label)!.FontSizeProperty,
                                        _isLarge ? new Dimension(1, Unit.Em) : new Dimension(1.5f, Unit.Em),
                                        false,
                                        0.5,
                                        PlaneGeometryTweener.DimensionTweener);

                                    tween.TweenPropertyAsync(
                                        el => (el as Button)!.BackgroundProperty,
                                        _isLarge
                                            ? new ColorBrush(CatTheme.Colors.Primary)
                                            : new ColorBrush(CatTheme.Colors.Tertiary),
                                        false,
                                        0.5,
                                        BrushTweener.GenericBrushTweener);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                                    _isLarge = !_isLarge;
                                }
                            }
                        ]
                    }
                ]
            };
        }
    }
}
