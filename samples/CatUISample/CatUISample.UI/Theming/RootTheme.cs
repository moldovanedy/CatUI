using CatUI.CoreExtensions.Itania;
using CatUI.Data.Brushes;
using CatUI.Data.Theming;
using CatUI.Elements;

namespace CatUISample.UI.Theming
{
    public static class RootTheme
    {
        public static Theme GetTheme()
        {
            Theme theme = ItaniaTheme.GetTheme();
            theme.AddOrUpdateClassDefinition(
                "MenuButtons",
                new ThemeDefinition(
                    el =>
                    {
                        el.ClipPath = null;
                        el.Background = new ColorBrush(CatTheme.Colors.Primary);
                    },
                    (el, state) =>
                    {
                        switch (state)
                        {
                            case Element.STATE_NORMAL:
                                el.Background = new ColorBrush(CatTheme.Colors.Primary);
                                break;
                            case Element.STATE_HOVER:
                                el.Background = new ColorBrush(CatTheme.Colors.Tertiary);
                                break;
                        }
                    }));
            return theme;
        }
    }
}
