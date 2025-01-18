using System.Collections.Generic;
using System.Data;

namespace CatUI.Elements.Themes
{
    /// <summary>
    /// A helper to create theme definitions easier and more concise.
    /// </summary>
    /// <typeparam name="T">
    /// The type of theme data of the element for which the theme definition is created.
    /// Must derive from <see cref="ElementThemeData"/>
    /// </typeparam>
    public class ThemeBuilder<T> where T : ElementThemeData, new()
    {
        private readonly Dictionary<string, ElementThemeData> _themes = new();

        private ThemeBuilder() { }

        /// <summary>
        /// Creates a new instance of the theme builder. Use this to set the theme data and call <see cref="Build"/>
        /// at the end to return the created theme definition.
        /// </summary>
        /// <returns></returns>
        public static ThemeBuilder<T> New()
        {
            return new ThemeBuilder<T>();
        }

        /// <summary>
        /// Adds the theme data to the given state. Throws DuplicateNameException if the state already exists.
        /// </summary>
        /// <param name="state">The name of the state to add the data to.</param>
        /// <param name="data">The actual theme data.</param>
        /// <returns>This instance of the builder for further use.</returns>
        /// <exception cref="DuplicateNameException">Thrown if the given state has already been added.</exception>
        public ThemeBuilder<T> AddData(string state, T data)
        {
            if (!_themes.TryAdd(state, data))
            {
                throw new DuplicateNameException("The given state has already been added.");
            }

            data.ForState = state;
            return this;
        }

        /// <summary>
        /// Creates the theme definition. You can use this function at any time in the theme building process,
        /// even multiple times. It will contain all the data that was added until this call.
        /// </summary>
        /// <returns>The newly created theme definition.</returns>
        public ThemeDefinition<T> Build()
        {
            return new ThemeDefinition<T>(_themes);
        }
    }
}
