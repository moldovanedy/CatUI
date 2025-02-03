using System;
using System.Collections.Generic;
using CatUI.Elements.Themes.Text;

namespace CatUI.Elements.Themes
{
    public class Theme
    {
        /// <summary>
        /// The key is the element name (a class that extends <see cref="Element"/>), the value is its 
        /// theme definition (like a theme override).
        /// </summary>
        private readonly Dictionary<string, ThemeDefinition<ElementThemeData>> _themeDefinitions = new();

        /// <summary>
        /// Adds the given theme definition to the theme. Returns false if a theme for the given element already exists
        /// (use <see cref="AddOrUpdateThemeDefinition{TElement}"/> for overwriting a theme).
        /// </summary>
        /// <param name="themeDefinition">
        /// The theme definition to set to the theme for the specified element. The <see cref="ThemeDefinition{T}"/>
        /// can be of any type derived from <see cref="ElementThemeData"/>.
        /// </param>
        /// <typeparam name="TElement">
        /// The type of element for which you want to add the theme. Must derive from <see cref="Element"/> or
        /// any derived class of <see cref="Element"/>.
        /// </typeparam>
        /// <returns>
        /// True if the addition succeeded, false if there already was a theme definition for the element
        /// of type TElement.
        /// </returns>
        public bool AddThemeDefinition<TElement>(ThemeDefinition<ElementThemeData> themeDefinition)
            where TElement : Element
        {
            return _themeDefinitions.TryAdd(nameof(TElement), themeDefinition);
        }

        /// <inheritdoc cref="AddThemeDefinition{TElement}"/>
        /// <summary>
        /// Adds the given theme definition if one wasn't found for the element of type TElement
        /// (basically works exactly like <see cref="AddThemeDefinition{TElement}"/> in this case)
        /// or overwrites it if it exists.
        /// </summary>
        /// <returns></returns>
        public void AddOrUpdateThemeDefinition<TElement>(ThemeDefinition<ElementThemeData> themeDefinition)
        {
            if (!_themeDefinitions.TryAdd(nameof(TElement), themeDefinition))
            {
                _themeDefinitions[nameof(TElement)] = themeDefinition;
            }
        }

        /// <summary>
        /// Given the Element type, attempts to find a theme definition for that element and casts it to
        /// <see cref="ThemeDefinition{T}"/> if it finds one, returns null otherwise. If it cannot cast because
        /// the type is not T, an InvalidCastException is thrown. 
        /// </summary>
        /// <typeparam name="TElement">The type of element for which you want to get the theme definition.</typeparam>
        /// <typeparam name="T">
        /// The type of <see cref="ElementThemeData"/> that the element has. An invalid parameter (for example
        /// if it is stored as <see cref="ElementThemeData"/> but T is <see cref="TextElementThemeData"/>)
        /// will throw an InvalidCastException. If you are unsure about the type, pass <see cref="ElementThemeData"/>.
        /// </typeparam>
        /// <returns>The existing definition if one was found, null otherwise.</returns>
        /// <exception cref="InvalidCastException">
        /// If the stored theme definition has a type that is incompatible with type T.
        /// </exception>
        public ThemeDefinition<T>? GetThemeDefinition<TElement, T>()
            where TElement : Element
            where T : ElementThemeData, new()
        {
            return
                _themeDefinitions.TryGetValue(nameof(TElement), out ThemeDefinition<ElementThemeData>? themeDefinition)
                    ? themeDefinition as ThemeDefinition<T> ?? throw new InvalidCastException()
                    : null;
        }

        /// <summary>
        /// Removes the theme definition for the given element type.
        /// </summary>
        /// <typeparam name="TElement">The type of element for which you want to remove the theme definition.</typeparam>
        /// <returns>True if the removal succeeds, false otherwise (if the theme wasn't there in the first place).</returns>
        public bool RemoveThemeDefinition<TElement>() where TElement : Element
        {
            return _themeDefinitions.Remove(nameof(TElement));
        }

        public void Clear()
        {
            _themeDefinitions.Clear();
        }
    }
}
