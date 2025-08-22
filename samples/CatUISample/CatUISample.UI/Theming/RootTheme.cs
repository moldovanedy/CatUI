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
                    onPseudoClassesChanged: (el, pseudoClasses) =>
                    {
                        var background = new ColorBrush(CatTheme.Colors.Primary);
                        foreach (string pseudoClass in pseudoClasses)
                        {
                            switch (pseudoClass)
                            {
                                case Element.PSEUDO_CLASS_NORMAL:
                                    background = new ColorBrush(CatTheme.Colors.Primary);
                                    break;
                                case Element.PSEUDO_CLASS_HOVER:
                                    background = new ColorBrush(CatTheme.Colors.Tertiary);
                                    break;
                            }
                        }

                        el.Background = background;
                    }));
            return theme;
        }
    }
}
