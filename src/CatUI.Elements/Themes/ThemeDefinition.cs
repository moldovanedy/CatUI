using System.Collections.Generic;
using System.Linq;
using CatUI.Data;

namespace CatUI.Elements.Themes
{
    /// <summary>
    /// This holds all the <see cref="ElementThemeData"/> for an element, either the definitions in a theme, 
    /// or the theme overrides of that element.
    /// </summary>
    /// <typeparam name="T">The type of ThemeData that the element accepts.</typeparam>
    public class ThemeDefinition<T> : CatObject where T : ElementThemeData, new()
    {
        /// <summary>
        /// The key is the state, the value is the actual data.
        /// </summary>
        private readonly Dictionary<string, ElementThemeData> _definitions = new();

        /// <summary>
        /// Creates a theme definition with all the default styles set.
        /// </summary>
        public ThemeDefinition()
        {
            var prototype = new T();
            List<string> states = prototype.GetAllBuiltInStates();

            foreach (string state in states)
            {
                SetThemeDataForState(state, prototype.GetDefaultData<T>(state));
            }
        }

        /// <summary>
        /// Creates a theme definition with custom definitions. Warning: it does NOT set the default definitions.
        /// </summary>
        /// <param name="definitions">
        /// The custom definitions, the key being the state name and the value is the theme data.
        /// </param>
        public ThemeDefinition(Dictionary<string, ElementThemeData> definitions)
        {
            _definitions = definitions;
        }

        public override ThemeDefinition<T> Duplicate()
        {
            Dictionary<string, ElementThemeData> duplicate = _definitions.ToDictionary(
                keySelector => keySelector.Key,
                valueSelector => valueSelector.Value.Duplicate());

            return new ThemeDefinition<T>(duplicate);
        }

        /// <summary>
        /// Returns a theme data for the given state if one exists, otherwise null. Throws InvalidCastException
        /// if T is not compatible with the stored type.
        /// </summary>
        /// <param name="state">The state for which to get the theme data.</param>
        /// <returns>A theme data for the given state if one exists, otherwise null.</returns>
        public T? GetThemeDataForState(string state)
        {
            return (T?)_definitions.GetValueOrDefault(state);
        }

        /// <summary>
        /// Sets the theme data for a given state. If the state doesn't exist, it is created. Overrides any previous theme.
        /// </summary>
        /// <param name="state">The state for which to set the theme data.</param>
        /// <param name="themeData">The actual theme data.</param>
        public void SetThemeDataForState(string state, T themeData)
        {
            _definitions[state] = themeData;
        }

        /// <summary>
        /// Similar to <see cref="SetThemeDataForState"/>, but only sets the non-null properties, meaning it ADDS overrides
        /// without removing existing ones. Use this when you want to preserve the old data and only set the properties
        /// that are set on this themeData object.
        /// </summary>
        /// <param name="state">The state for which to set the theme data.</param>
        /// <param name="themeData">The theme data to set. Only the non-null properties are considered.</param>
        public void SetThemeDataForStateAdditively(string state, T themeData)
        {
            _definitions[state].ApplyDataAdditively(themeData);
        }

        /// <summary>
        /// For any non-null field in the mask, it will set this object's field to null, so this works similar to a bitmask:
        /// the object will only interact with the properties set in the mask, in this case the non-null properties.
        /// It does not matter what kind of value there is in the mask, it is just important if it's null or not.
        /// </summary>
        /// <param name="state">The state for which to reset the theme data.</param>
        /// <param name="mask">
        /// The mask to use to make fields null. Only non-null properties are considered, their value is not important.
        /// </param>
        public void ResetThemeDataForStateAdditively(string state, T mask)
        {
            _definitions[state].ResetDataAdditively(mask);
        }

        /// <summary>
        /// Removes a state from the current object. Removing states used in the built-in elements (such as
        /// <see cref="ElementThemeData.STYLE_NORMAL"/>) might cause unexpected behavior or simply the element will throw an exception.
        /// It is highly discouraged to remove states from built-in components unless the documentation of those elements
        /// say otherwise. This is generally useful for custom states on custom elements defined by you.
        /// </summary>
        /// <param name="state">The state to remove.</param>
        /// <returns>True if the removal succeeded, false otherwise (for example, the state didn't exist).</returns>
        public bool RemoveState(string state)
        {
            return _definitions.Remove(state);
        }

        /// <summary>
        /// Returns all the current states of the object.
        /// </summary>
        /// <returns></returns>
        public string[] GetStates()
        {
            Dictionary<string, ElementThemeData>.KeyCollection collection = _definitions.Keys;
            string[] states = new string[collection.Count];
            collection.CopyTo(states, 0);

            return states;
        }
    }
}
