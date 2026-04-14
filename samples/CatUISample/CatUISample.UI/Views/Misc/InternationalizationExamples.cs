using CatUI.CoreExtensions.I18n;
using CatUI.CoreExtensions.I18n.GetText;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;

namespace CatUISample.UI.Views.Misc;

public class InternationalizationExamples : ScrollContainer
{
    private const string EN = "en_US";
    private const string RO = "ro_RO";
    private const string BG = "bg_BG";

    private readonly ObservableProperty<bool> _isEnSet = new(true);
    private readonly ObservableProperty<bool> _isRoSet = new(false);
    private readonly ObservableProperty<bool> _isBgSet = new(false);
    private readonly ObservableProperty<string> _currentLanguage = new(EN);

    public InternationalizationExamples()
    {
        _isEnSet.ValueChangedEvent += OnEnSet;
        _isRoSet.ValueChangedEvent += OnRoSet;
        _isBgSet.ValueChangedEvent += OnBgSet;
        _currentLanguage.ValueChangedEvent += OnLangChanged;

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
                        new Label("Internationalization examples", TextAlignmentType.Center)
                        {
                            Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                            FontSize = 32,
                            TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                        },
                        new RowContainer
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%"),
                            Arrangement = LinearArrangement.SpacedBy(10),
                            Children =
                            [
                                new RadioButton(
                                    true,
                                    new Label("en-US")
                                    {
                                        TextBrush = new ColorBrush(CatTheme.Colors.OnSurface),
                                        Layout = new ElementLayout().SetFixedWidth(50).SetFixedHeight(24)
                                    })
                                {
                                    ValueProperty = _isEnSet,
                                    Layout = new ElementLayout().SetFixedWidth(100).SetFixedHeight(24)
                                },
                                new RadioButton(
                                    false,
                                    new Label("ro-RO")
                                    {
                                        TextBrush = new ColorBrush(CatTheme.Colors.OnSurface),
                                        Layout = new ElementLayout().SetFixedWidth(50).SetFixedHeight(24)
                                    })
                                {
                                    ValueProperty = _isRoSet,
                                    Layout = new ElementLayout().SetFixedWidth(100).SetFixedHeight(24)
                                },
                                new RadioButton(
                                    false,
                                    new Label("bg-BG")
                                    {
                                        TextBrush = new ColorBrush(CatTheme.Colors.OnSurface),
                                        Layout = new ElementLayout().SetFixedWidth(50).SetFixedHeight(24)
                                    })
                                {
                                    ValueProperty = _isBgSet,
                                    Layout = new ElementLayout().SetFixedWidth(100).SetFixedHeight(24)
                                }
                            ]
                        },
                        new Label
                        {
                            TextProperty = StringMgr.Instance.GetProperty("Hello world in English!"),
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(24),
                            TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                        },
                        new Label
                        {
                            TextProperty = StringMgr.Instance.GetPropertyPlural(
                                "I have {0} apple.", "I have {0} apples.", 28),
                            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(24),
                            TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                        },
                        new RowContainer
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%"),
                            Arrangement = LinearArrangement.SpacedBy(10),
                            Children =
                            [
                                new Label("As verb (e.g., to file a complaint):")
                                {
                                    Layout = new ElementLayout().SetMinMaxAndPreferredWidth("70%", 70, 240),
                                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface),
                                    WordWrap = true
                                },
                                new Label
                                {
                                    TextProperty = StringMgr.Instance.GetPropertyCtx("File", "Verb"),
                                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                                }
                            ]
                        },
                        new RowContainer
                        {
                            Layout = new ElementLayout().SetFixedWidth("100%"),
                            Arrangement = LinearArrangement.SpacedBy(10),
                            Children =
                            [
                                new Label("As noun (e.g., a file on the computer):")
                                {
                                    Layout = new ElementLayout().SetMinMaxAndPreferredWidth("70%", 70, 270),
                                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface),
                                    WordWrap = true
                                },
                                new Label
                                {
                                    TextProperty = StringMgr.Instance.GetPropertyCtx("File", "Noun"),
                                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                                }
                            ]
                        }
                    ]
                }
            ]
        };
    }

    private void OnLangChanged(string? newValue)
    {
        if (newValue == null)
        {
            return;
        }

        _isEnSet.Value = newValue == EN;
        _isRoSet.Value = newValue == RO;
        _isBgSet.Value = newValue == BG;

        var i18NAsset = AssetsManager.LoadFromAssembly<I18NStreamAsset>($"/Assets/Strings/{newValue}.mo");
        if (i18NAsset != null)
        {
            StringMgr.Instance.SetLocalizationManager(new GetTextLocalizationManager(newValue, i18NAsset));
        }
    }

    private void OnEnSet(bool value)
    {
        if (value && _currentLanguage.Value != EN)
        {
            _currentLanguage.Value = EN;
        }
    }

    private void OnRoSet(bool value)
    {
        if (value && _currentLanguage.Value != RO)
        {
            _currentLanguage.Value = RO;
        }
    }

    private void OnBgSet(bool value)
    {
        if (value && _currentLanguage.Value != BG)
        {
            _currentLanguage.Value = BG;
        }
    }
}
