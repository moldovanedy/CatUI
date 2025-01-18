using CatUI.Elements.Themes;

namespace CatUI.Elements
{
    public partial class Element
    {
        private readonly ThemeDefinition<ElementThemeData> _themeOverrides = new();

        public ElementThemeData? GetElementThemeOverride(string state)
        {
            return _themeOverrides.GetThemeDataForState(state);
        }

        public T? GetElementThemeOverride<T>(string state) where T : ElementThemeData, new()
        {
            return (T?)_themeOverrides.GetThemeDataForState(state);
        }

        /// <summary>
        /// Returns the element's ThemeData that should be applied and all styling should respect this theme.
        /// Any change to this element will not reflect in the style, use theme overrides or sub-themes for that.
        /// If there is no Theme in the document tree hierarchy (not even at root level), this will return
        /// the default ThemeData of the specified type parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the state exists, this will always return an object with all the properties non-null,
        /// unless you create a new theme data and don't handle virtual methods properly. But for built-in
        /// theme data objects, the properties will always be non-null.
        /// </para>
        /// <para>
        /// Once you call this, cache the result as much as possible because this call might be a little bit expensive.
        /// The cache is a clone, so it can't reflect the eventual changes after this call, but this is ok in single-threaded,
        /// non-async code. Generally you should only keep the cache for the current (or next) draw operation.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The ThemeData type of the element.</typeparam>
        /// <param name="state">The state for which the styling is applied.</param>
        /// <returns>The ThemeData that should be respected by the element at the given state.</returns>
        public T? GetElementFinalThemeData<T>(string state) where T : ElementThemeData, new()
        {
            var prototype = new T();
            var resultingTheme = prototype.GetDefaultData<T>(state);

            var overrides = (T?)_themeOverrides.GetThemeDataForState(state);
            if (overrides != null)
            {
                resultingTheme.ApplyDataAdditively(overrides);
            }

            return resultingTheme;
        }

        public void SetElementThemeOverride(string state, ElementThemeData themeOverride)
        {
            _themeOverrides.SetThemeDataForState(state, themeOverride);
        }

        public void SetElementThemeOverrides<T>(ThemeDefinition<T> themeOverrides) where T : ElementThemeData, new()
        {
            foreach (string state in themeOverrides.GetStates())
            {
                _themeOverrides.SetThemeDataForState(state, themeOverrides.GetThemeDataForState(state)!);
            }
        }
    }
}
