using System.Collections.Generic;

namespace CatUI.Elements.Themes
{
    /// <summary>
    /// This holds all the <see cref="ElementThemeData"/> for an element, either the definitions in a theme, 
    /// or the theme overrides of that element.
    /// </summary>
    /// <typeparam name="T">The type of ThemeData that the element accepts.</typeparam>
    public class ThemeDefinition<T> where T : ElementThemeData, new()
    {
        /// <summary>
        /// The key is the state, the value is the actual data.
        /// </summary>
        private readonly Dictionary<string, T> _definitions = new Dictionary<string, T>();

        public ThemeDefinition() { }

        public ThemeDefinition(Dictionary<string, T> definitions)
        {
            _definitions = definitions;
        }

        /// <summary>
        /// Returns a theme data for the given state if one exists, otherwise null.
        /// </summary>
        /// <param name="state">The state for which to get the theme data.</param>
        /// <returns>A theme data for the given state if one exists, otherwise null.</returns>
        public T? GetThemeDataForState(string state)
        {
            if (_definitions.TryGetValue(state, out T? themeData))
            {
                return themeData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the theme data for a given state. This works regardless of the existence of the state previously.
        /// </summary>
        /// <param name="state">The state for which to set the theme data.</param>
        /// <param name="themeData">The actual theme data.</param>
        public void SetThemeDataForState(string state, T themeData)
        {
            _definitions[state] = themeData;
        }

        public string[] GetStates()
        {
            Dictionary<string, T>.KeyCollection collection = _definitions.Keys;
            string[] states = new string[collection.Count];
            collection.CopyTo(states, 0);

            return states;
        }
    }
}