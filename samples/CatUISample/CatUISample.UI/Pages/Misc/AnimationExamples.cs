using System;
using System.Threading.Tasks;
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

namespace CatUISample.UI.Pages.Misc;

public class AnimationExamples : ScrollContainer
{
    private bool _isLarge;
    private bool _isAnimating;

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
                            OnClick = async void (sender, _) =>
                            {
                                try
                                {
                                    if (_isAnimating || sender is not Button element)
                                    {
                                        return;
                                    }

                                    _isAnimating = true;
                                    var tween = new Tween(element)
                                    {
                                        AnimationEasing = new Easing(Easing.EasingType.BackInOut)
                                    };

                                    Task fontTask = tween.TweenPropertyAsync(
                                        el => ((el as Button)!.TextElement as Label)!.FontSizeProperty,
                                        _isLarge ? new Dimension(1, Unit.Em) : new Dimension(1.5f, Unit.Em),
                                        false,
                                        0.5,
                                        PlaneGeometryTweener.DimensionTweener);

                                    Task bgColorTask = tween.TweenPropertyAsync(
                                        el => (el as Button)!.BackgroundProperty,
                                        _isLarge
                                            ? new ColorBrush(CatTheme.Colors.Primary)
                                            : new ColorBrush(CatTheme.Colors.Tertiary),
                                        false,
                                        0.5,
                                        BrushTweener.GenericBrushTweener);

                                    await Task.WhenAll(fontTask, bgColorTask);
                                    _isLarge = !_isLarge;
                                    _isAnimating = false;
                                }
                                catch (Exception ex)
                                {
                                    CatLogger.LogError($"Error at AnimationExamples: {ex}");
                                }
                            }
                        }
                    ]
                }
            ]
        };
    }
}
