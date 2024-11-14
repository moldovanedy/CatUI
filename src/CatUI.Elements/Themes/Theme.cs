using System.Collections.Generic;

namespace CatUI.Elements.Themes
{
    public class Theme
    {
        /// <summary>
        /// The key is the element name (a class that extends <see cref="Element"/>), the value is its 
        /// theme definition (like a theme override).
        /// </summary>
        private Dictionary<string, Dictionary<string, ElementThemeData>> _themeDefinitions =
            new Dictionary<string, Dictionary<string, ElementThemeData>>();

        //public void AddDefinition<T>(Dictionary<>)

        public void Clear()
        {
            _themeDefinitions.Clear();
        }
    }
}