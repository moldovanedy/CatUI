using CatUI.CoreExtensions.Itania;
using CatUI.Elements;

namespace ProjectName.UI
{
    public static class ThemeSetup
    {
        /// <summary>
        /// Returns the root theme, the theme of the entire application.
        /// </summary>
        /// <returns></returns>
        public static Theme GetTheme()
        {
            //get the default CatUI theme, Itania
            Theme theme = ItaniaTheme.GetTheme();
            //add your own theme definitions here, then return the theme
            return theme;
        }
    }
}
