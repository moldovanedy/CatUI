using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CatUI.Elements
{
    public class Theme
    {
        /// <summary>
        /// Fired whenever a theme definition is added, updated (even internally; for example, setting a new
        /// <see cref="ThemeDefinition.OnThemeChanged"/>) or removed from this theme. The argument specifies what
        /// kind of theme definition was modified. See <see cref="ThemeModifiedArgs"/> for more information.
        /// </summary>
        public event Action<ThemeModifiedArgs>? ThemeModified;

        private readonly Dictionary<Type, ThemeDefinition> _themeDefinitions = new();

        private readonly Dictionary<string, ThemeDefinition> _styleClassDefinitions = new();

        /// <summary>
        /// Get the definition (if one exists, otherwise null) for the type of the given element. This is NOT for the
        /// given instance but rather for its type.
        /// </summary>
        /// <param name="element">The instance of the element whose type has the definition.</param>
        /// <returns>The corresponding <see cref="ThemeDefinition"/> or null if one was not found.</returns>
        public ThemeDefinition? GetElementTypeDefinition(Element element)
        {
            return GetElementTypeDefinition(element.GetType());
        }

        /// <summary>
        /// Get the definition (if one exists, otherwise null) of the given element type.
        /// </summary>
        /// <param name="elementType">The type of element for which you want the definition.</param>
        /// <returns>The corresponding <see cref="ThemeDefinition"/> or null if one was not found.</returns>
        public ThemeDefinition? GetElementTypeDefinition(Type elementType)
        {
            return _themeDefinitions.GetValueOrDefault(elementType);
        }

        /// <summary>
        /// Get the definition (if one exists, otherwise null) of the given style class.
        /// </summary>
        /// <param name="className">The name of the style class for which you want the definition.</param>
        /// <returns>The corresponding <see cref="ThemeDefinition"/> or null if one was not found.</returns>
        public ThemeDefinition? GetClassDefinition(string className)
        {
            return _styleClassDefinitions.GetValueOrDefault(className);
        }

        /// <summary>
        /// Adds or updates an existing definition for a type of elements, denoted by the given element. This is NOT
        /// for the given instance but rather for its type.
        /// </summary>
        /// <param name="element">The instance of the element whose type has the definition.</param>
        /// <param name="themeDefinition">The new definition.</param>
        public void AddOrUpdateElementTypeDefinition(Element element, ThemeDefinition themeDefinition)
        {
            AddOrUpdateElementTypeDefinition(element.GetType(), themeDefinition);
        }

        /// <summary>
        /// Adds or updates an existing definition for a type of elements.
        /// </summary>
        /// <param name="elementType">The type of element for which you give the definition.</param>
        /// <param name="themeDefinition">The new definition.</param>
        public void AddOrUpdateElementTypeDefinition(Type elementType, ThemeDefinition themeDefinition)
        {
            if (!_themeDefinitions.TryAdd(elementType, themeDefinition))
            {
                _themeDefinitions[elementType] = themeDefinition;
            }

            _themeDefinitions[elementType].ElementType = elementType;
            _themeDefinitions[elementType].PropertyChanged += OnStylingFunctionsChanged;
            ThemeModified?.Invoke(new ThemeModifiedArgs(elementType));
        }

        /// <summary>
        /// Adds or updates an existing definition for a type of elements.
        /// </summary>
        /// <param name="themeDefinition">The new definition.</param>
        /// <typeparam name="T">The type of element for which you give the definition.</typeparam>
        public void AddOrUpdateElementTypeDefinition<T>(ThemeDefinition themeDefinition)
        {
            AddOrUpdateElementTypeDefinition(typeof(T), themeDefinition);
        }

        /// <summary>
        /// Adds or updates an existing definition for a style class.
        /// </summary>
        /// <param name="className">The style class name for which you give the definition.</param>
        /// <param name="themeDefinition">The new definition.</param>
        public void AddOrUpdateClassDefinition(string className, ThemeDefinition themeDefinition)
        {
            if (!_styleClassDefinitions.TryAdd(className, themeDefinition))
            {
                _styleClassDefinitions[className] = themeDefinition;
            }

            _styleClassDefinitions[className].StyleClass = className;
            _styleClassDefinitions[className].PropertyChanged += OnStylingFunctionsChanged;
            ThemeModified?.Invoke(new ThemeModifiedArgs(null, className));
        }

        /// <summary>
        /// Remove the definition for the type of elements, denoted by the given element. This is NOT for the given
        /// instance but rather for its type.
        /// </summary>
        /// <param name="element">The instance of the element whose type has the definition.</param>
        public void RemoveElementTypeDefinition(Element element)
        {
            RemoveElementTypeDefinition(element.GetType());
        }

        /// <summary>
        /// Remove the definition for the type of elements.
        /// </summary>
        /// <param name="elementType">The type of element that has the definition.</param>
        public void RemoveElementTypeDefinition(Type elementType)
        {
            if (!_themeDefinitions.TryGetValue(elementType, out ThemeDefinition? definition))
            {
                return;
            }

            definition.PropertyChanged -= OnStylingFunctionsChanged;
            _themeDefinitions.Remove(elementType);
            ThemeModified?.Invoke(new ThemeModifiedArgs(elementType));
        }

        /// <summary>
        /// Remove the definition for the type of elements.
        /// </summary>
        /// <typeparam name="T">The type of element that has the definition.</typeparam>
        public void RemoveElementTypeDefinition<T>()
        {
            RemoveElementTypeDefinition(typeof(T));
        }

        /// <summary>
        /// Remove the definition for the style class.
        /// </summary>
        /// <param name="className">The style class name that has the definition.</param>
        public void RemoveClassDefinition(string className)
        {
            if (!_styleClassDefinitions.TryGetValue(className, out ThemeDefinition? definition))
            {
                return;
            }

            definition.PropertyChanged -= OnStylingFunctionsChanged;
            _styleClassDefinitions.Remove(className);
            ThemeModified?.Invoke(new ThemeModifiedArgs(null, className));
        }


        private void OnStylingFunctionsChanged(object? sender, PropertyChangedEventArgs e)
        {
            var definition = sender as ThemeDefinition;
            if (definition?.ElementType != null)
            {
                ThemeModified?.Invoke(new ThemeModifiedArgs(definition.ElementType));
            }
            else if (definition?.StyleClass != null)
            {
                ThemeModified?.Invoke(new ThemeModifiedArgs(null, definition.StyleClass));
            }
        }

        /// <summary>
        /// Gives more information about what changed inside a <see cref="Theme"/> object.
        /// </summary>
        public readonly struct ThemeModifiedArgs
        {
            /// <summary>
            /// If set, it means that the theme definition for a type of element was modified.
            /// </summary>
            public Type? ElementType { get; }

            /// <summary>
            /// If set, it means that the theme definition for a style class was modified.
            /// </summary>
            public string? ClassName { get; }

            public ThemeModifiedArgs(Type? elementType = null, string? className = null)
            {
                ElementType = elementType;
                ClassName = className;
            }
        }
    }
}
