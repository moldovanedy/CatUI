using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Data.Theming;
using CatUI.Elements;
using CatUI.Elements.Behaviors;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.ControlFlow;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;

namespace CatUI.CoreExtensions.Itania
{
    public static class ItaniaTheme
    {
        public static Theme GetTheme()
        {
            Theme theme = new();

            //IFocusable
            theme.AddOrUpdateElementTypeDefinition<IFocusable>(
                new ThemeDefinition(
                    onStateChanged: (el, newState) =>
                    {
                        if (newState == IFocusable.STATE_FOCUSED)
                        {
                            el.DrawEvent += OnFocusableDraw;
                        }
                        else
                        {
                            el.DrawEvent -= OnFocusableDraw;
                        }

                        el.RequestRedraw();
                    }));

            //text
            theme.AddOrUpdateElementTypeDefinition<TextElement>(
                new ThemeDefinition(el => ((TextElement)el).FontSize = "1em"));

            theme.AddOrUpdateElementTypeDefinition<TextBlock>(
                new ThemeDefinition(el => ((TextBlock)el).TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)));

            //scroll
            theme.AddOrUpdateElementTypeDefinition<ScrollContainer>(
                new ThemeDefinition(el =>
                {
                    var container = (ScrollContainer)el;
                    container.InternalHorizontalScrollBar.Layout?.SetFixedHeight(12);
                    container.InternalVerticalScrollBar.Layout?.SetFixedWidth(12);

                    Theme scrollBarTheme = new();
                    scrollBarTheme.AddOrUpdateElementTypeDefinition<ScrollBarBase>(
                        new ThemeDefinition(e =>
                        {
                            var scrollBar = (ScrollBarBase)e;
                            scrollBar.ShouldDisplayButtons = false;

                            scrollBar.InternalThumbElement.ClipPath
                                = new RoundedRectangleClipShape(new Dimension("50%"));

                            //override the button style from the scroll bar itself
                            Theme selfButtonTheme = new();
                            selfButtonTheme.AddOrUpdateElementTypeDefinition<Button>(
                                new ThemeDefinition(barButton =>
                                {
                                    var btn = (Button)barButton;
                                    btn.ClipPath = null;
                                }));
                            scrollBar.ThemeOverride = selfButtonTheme;
                        }));

                    container.InternalHorizontalScrollBar.ThemeOverride = scrollBarTheme;
                    container.InternalVerticalScrollBar.ThemeOverride = scrollBarTheme;
                }));

            //button
            theme.AddOrUpdateElementTypeDefinition<Button>(
                new ThemeDefinition(el =>
                {
                    var btn = (Button)el;
                    btn.Spacing = 10;
                    btn.Padding = new EdgeInset(5, 7);
                    btn.ClipPath = new RoundedRectangleClipShape(8);
                    btn.Background = new ColorBrush(CatTheme.Colors.Primary);
                }));

            //checkbox
            theme.AddOrUpdateElementTypeDefinition<CheckBox>(
                new ThemeDefinition(el =>
                {
                    //Itania needs to provide its own indicator element, as the default one has direct local
                    //properties set, meaning we can't modify those from a theme
                    var checkBox = (CheckBox)el;
                    checkBox.Spacing = 10;

                    checkBox.IndicatorElement = new TriStateCheckBoxIndicator(
                        CheckBox.CheckBoxState.Unchecked,
                        new RoundedRectangleElement(new ColorBrush(CatTheme.Colors.Primary))
                        {
                            RoundCornersDescriptor = CatTheme.ClipShapes.SmallRounding.RoundCornersDescriptor,
                            StyleClass = "CheckBox::Indicator::Checked::Inner",
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                            Children =
                            [
                                new GeometricPathElement(
                                    //from CatUI/IndirectAssets/checkmark.svg
                                    "m 9.118746,16.621968 c -0.892928,0.892928 -2.341566,0.892713 -3.2342794,0 C 4.2141044,14.951606 2.5437422,13.281244 0.87338,11.610881 c -0.8927129,-0.892713 -0.8927129,-2.341136 0,-3.2338485 0.892713,-0.8927129 2.3412869,-0.892864 3.2342797,0 0.9952072,0.9950635 1.9904144,1.9901275 2.9856213,2.9851905 0.225188,0.225156 0.591694,0.225387 0.817082,0 2.69473,-2.6947302 5.38946,-5.3894605 8.084191,-8.0841909 0.892712,-0.8927129 2.341351,-0.8929284 3.234279,0 0.428796,0.4287953 0.669697,1.0105778 0.669697,1.6169244 0,0.6063467 -0.240901,1.1881292 -0.669697,1.6169244 C 15.858804,9.88191 12.488775,13.251939 9.118746,16.621968 Z",
                                    new ColorBrush(CatTheme.Colors.OnPrimary))
                                {
                                    StyleClass = "CheckBox::Indicator::Checked::Graphic",
                                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                                }
                            ]
                        },
                        new RoundedRectangleElement(outlineBrush: new ColorBrush(CatTheme.Colors.Outline))
                        {
                            RoundCornersDescriptor = CatTheme.ClipShapes.SmallRounding.RoundCornersDescriptor,
                            StyleClass = "CheckBox::Indicator::Unchecked::Outer",
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                            OutlineParameters = new OutlineParams(3f)
                        },
                        new RoundedRectangleElement(outlineBrush: new ColorBrush(CatTheme.Colors.Outline))
                        {
                            RoundCornersDescriptor = CatTheme.ClipShapes.MediumRounding.RoundCornersDescriptor,
                            StyleClass = "CheckBox::Indicator::Indeterminate::Outer",
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                            OutlineParameters = new OutlineParams(3f),
                            Children =
                            [
                                new RoundedRectangleElement(new ColorBrush(CatTheme.Colors.Primary))
                                {
                                    RoundCornersDescriptor = CatTheme.ClipShapes.MediumRounding.RoundCornersDescriptor,
                                    StyleClass = "CheckBox::Indicator::Indeterminate::Inner",
                                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                    Children =
                                    [
                                        new GeometricPathElement(
                                            //from CatUI/IndirectAssets/radio-button.svg
                                            "m 5,7 h 10 c 1.662,0 3,1.338 3,3 0,1.662 -1.338,3 -3,3 H 5 C 3.338,13 2,11.662 2,10 2,8.338 3.338,7 5,7 Z",
                                            new ColorBrush(CatTheme.Colors.OnPrimary))
                                        {
                                            StyleClass = "CheckBox::Indicator::Indeterminate::Graphic",
                                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                                        }
                                    ]
                                }
                            ]
                        }
                    ) { Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20) };
                }));

            //radio button
            theme.AddOrUpdateElementTypeDefinition<RadioButton>(
                new ThemeDefinition(el =>
                {
                    var radioButton = (RadioButton)el;
                    radioButton.Spacing = 10;

                    radioButton.IndicatorElement = new IfElement(
                        radioButton.ValueProperty,
                        new EllipseElement(outlineBrush: new ColorBrush(CatTheme.Colors.Primary))
                        {
                            StyleClass = "RadioButton::Indicator::Active::Outer",
                            Position = new Dimension2(1, 1),
                            Layout = new ElementLayout().SetFixedWidth(18).SetFixedHeight(18),
                            OutlineParameters = new OutlineParams(2f),
                            ClipType = ClipApplicability.HitTesting,
                            Children =
                            [
                                new EllipseElement(new ColorBrush(CatTheme.Colors.Primary))
                                {
                                    StyleClass = "RadioButton::Indicator::Active::Inner",
                                    Position = new Dimension2(2, 2),
                                    Layout = new ElementLayout().SetFixedWidth(14).SetFixedHeight(14),
                                    ClipType = ClipApplicability.HitTesting
                                }
                            ]
                        },
                        new EllipseElement(outlineBrush: new ColorBrush(CatTheme.Colors.Outline))
                        {
                            StyleClass = "RadioButton::Indicator::Inactive::Outer",
                            Position = new Dimension2(1, 1),
                            Layout = new ElementLayout().SetFixedWidth(18).SetFixedHeight(18),
                            OutlineParameters = new OutlineParams(2f),
                            ClipType = ClipApplicability.HitTesting
                        }
                    ) { Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20) };
                }));

            //switch button
            theme.AddOrUpdateElementTypeDefinition<SwitchButton>(new ThemeDefinition(el =>
            {
                var switchButton = (SwitchButton)el;
                switchButton.Spacing = 10;

                switchButton.IndicatorElement = new IfElement(
                    switchButton.ValueProperty,
                    new RoundedRectangleElement(new ColorBrush(CatTheme.Colors.Primary))
                    {
                        StyleClass = "SwitchButton::Indicator::Active::Outer",
                        RoundCornersDescriptor = new CornerInset(1000),
                        Position = new Dimension2(0, 3),
                        Layout = new ElementLayout().SetFixedWidth(36).SetFixedHeight(14),
                        ClipType = ClipApplicability.HitTesting,
                        Children =
                        [
                            new RowContainer
                            {
                                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                ClipType = ClipApplicability.HitTesting,
                                Children =
                                [
                                    new Element { ElementContainerSizing = new RowContainerSizing() },
                                    new EllipseElement(new ColorBrush(CatTheme.Colors.InverseSurface))
                                    {
                                        StyleClass = "SwitchButton::Indicator::Active::Inner",
                                        ElementContainerSizing =
                                            new RowContainerSizing(verticalAlignment: VerticalAlignmentType.Center),
                                        Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                                        ClipType = ClipApplicability.HitTesting
                                    }
                                ]
                            }
                        ]
                    },
                    new RoundedRectangleElement(new ColorBrush(CatTheme.Colors.SurfaceContainerHigh))
                    {
                        StyleClass = "SwitchButton::Indicator::Inactive::Outer",
                        RoundCornersDescriptor = new CornerInset(1000),
                        Position = new Dimension2(0, 3),
                        Layout = new ElementLayout().SetFixedWidth(36).SetFixedHeight(14),
                        ClipType = ClipApplicability.HitTesting,
                        Children =
                        [
                            new RowContainer
                            {
                                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                                ClipType = ClipApplicability.HitTesting,
                                Children =
                                [
                                    new EllipseElement(new ColorBrush(CatTheme.Colors.InverseSurface))
                                    {
                                        StyleClass = "SwitchButton::Indicator::Inactive::Inner",
                                        ElementContainerSizing =
                                            new RowContainerSizing(verticalAlignment: VerticalAlignmentType.Center),
                                        Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                                        ClipType = ClipApplicability.HitTesting
                                    },
                                    new Element { ElementContainerSizing = new RowContainerSizing() }
                                ]
                            }
                        ]
                    }
                )
                {
                    Layout = new ElementLayout().SetFixedWidth(36).SetFixedHeight(20),
                    ElementContainerSizing =
                        new RowContainerSizing(0, VerticalAlignmentType.Center),
                    ClipType = ClipApplicability.HitTesting
                };
            }));

            return theme;

            void OnFocusableDraw(object sender)
            {
                if (sender is not Element element)
                {
                    return;
                }

                element.Document?.Renderer.DrawRectOutline(
                    element.Bounds,
                    new ColorBrush(CatTheme.Colors.OutlineVariant),
                    new OutlineParams(3f));
            }
        }
    }
}
