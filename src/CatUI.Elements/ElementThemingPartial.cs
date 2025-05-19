using System;
using System.Collections.Generic;
using CatUI.Data;

namespace CatUI.Elements
{
    public partial class Element
    {
        /// <summary>
        /// A lambda function that has as a parameter the new state the element is in and this element. You should
        /// set the properties directly depending on the state (i.e. use a switch statement or something).
        /// </summary>
        public delegate void LocalThemeLambda(string? newState, Element element);

        /// <summary>
        /// The local theming function that is called:
        /// <list type="bullet">
        /// <item>
        /// directly when set (always with the default state of null and, if the current state is other than null,
        /// called again with the current state)
        /// </item>
        /// <item>whenever the element enters a new state (with the current state)</item>
        /// <item>
        /// whenever the global theme is applied for this element (generally when there is a change in the global theme)
        /// with the current state
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This is always called after the global themes are applied, but before setting the direct properties to their
        /// custom value (i.e. if you set a certain property that has a *Property counterpart to a value directly, then
        /// that value will have precedence over the global theme and this local theme). Refer to the manual for more
        /// information about theming.
        /// </remarks>
        public LocalThemeLambda? LocalThemingFunction
        {
            get => _localThemingFunction;
            set
            {
                _localThemingFunction = value;
                //always call for the default state
                _localThemingFunction?.Invoke(null, this);

                //then, call for the current state if it's other than default
                if (State != null)
                {
                    _localThemingFunction?.Invoke(State, this);
                }
            }
        }

        private LocalThemeLambda? _localThemingFunction;

        public Theme? ThemeOverride
        {
            get => _themeOverride;
            set
            {
                if (value != _themeOverride)
                {
                    ThemeOverrideProperty.Value = value;
                }
            }
        }

        private Theme? _themeOverride;

        public ObservableProperty<Theme> ThemeOverrideProperty { get; } = new(null);

        private void SetThemeOverride(Theme? value)
        {
            if (_themeOverride != null)
            {
                _themeOverride.ThemeModified -= OnThemeModified;
            }

            _themeOverride = value;

            if (_themeOverride != null)
            {
                _themeOverride.ThemeModified += OnThemeModified;
            }
        }

        public string StyleClass
        {
            get => _styleClass;
            set
            {
                if (value != _styleClass)
                {
                    StyleClassProperty.Value = value;
                }
            }
        }

        private string _styleClass = "";

        public ObservableProperty<string> StyleClassProperty { get; } = new("");

        private void SetStyleClass(string? value)
        {
            value ??= "";
            _styleClass = value;
            ApplyClassTheme(value, false);
        }

        /// <summary>
        /// The number of times you want the element to go to its base type and apply the applicable theme for that type.
        /// A value lower than 0 will apply every theme until reaching Element; 0 means only the current type will get
        /// considered, ignoring base types. The default value is -1.
        /// </summary>
        /// <example>
        /// For a TextBlock, a value of -1 means that the themes for Element, TextElement and TextBlock will be applied
        /// in this order. A value of 0 means that only the Theme for TextBlock will be applied, while a value of 1
        /// means that the themes for TextElement and TextBlock will be applied in this order.
        /// </example>
        public int BaseThemingCount
        {
            get => _baseThemingCount;
            set
            {
                if (value != _baseThemingCount)
                {
                    BaseThemingCountProperty.Value = value;
                }
            }
        }

        private int _baseThemingCount = -1;
        public ObservableProperty<int> BaseThemingCountProperty { get; } = new(-1);

        private void SetBaseThemingCount(int value)
        {
            _baseThemingCount = value;
            ApplyElementTypeTheme();
        }

        /// <summary>
        /// If set to true, the element will ignore any theme overrides from the ascendants and instead only uses the
        /// theme overrides of itself.
        /// </summary>
        public bool IgnoreGlobalTheming
        {
            get => _ignoreGlobalTheming;
            set
            {
                if (value != _ignoreGlobalTheming)
                {
                    IgnoreGlobalThemingProperty.Value = value;
                }
            }
        }

        private bool _ignoreGlobalTheming;
        public ObservableProperty<bool> IgnoreGlobalThemingProperty { get; } = new(false);

        private void SetIgnoreGlobalTheming(bool value)
        {
            _ignoreGlobalTheming = value;
            ApplyElementTypeTheme();
        }


        private readonly Dictionary<string, object?> _localValues = [];
        private bool _isLocalValueDictionaryLocked;

        /// <summary>
        /// Local values are the values that are directly set on a property outside the global theme. Once you set it,
        /// it takes precedence over the global themes. Call this if you want to remove the local value and make the
        /// global theme relevant for the given property name once again.
        /// </summary>
        /// <remarks>
        /// This is only relevant for bindable properties (i.e. the ones that have a *Property counterpart). Calling
        /// this for another type of property won't do anything. Refer to the manual for more information about theming.
        /// </remarks>
        /// <param name="propertyName">The property name; always use <c>nameof(PROPERTY)</c> for this.</param>
        public void RemoveLocalValue(string propertyName)
        {
            _localValues.Remove(propertyName);
        }

        /// <summary>
        /// Only call this on property change handlers, as this marks the given property value as being "local", so it
        /// will always be reset to the given value after the global theming finishes.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property. You should always use `nameof(PROPERTY)` to get this string.
        /// </param>
        /// <param name="value">The local value that will be set after global theming.</param>
        protected void SetLocalValue(string propertyName, object? value)
        {
            if (!_isLocalValueDictionaryLocked)
            {
                _localValues[propertyName] = value;
            }
        }

        /// <summary>
        /// Returns the value of a property that was set as "local" either directly or inside
        /// <see cref="LocalThemingFunction"/>. Returns null if nothing was found, with wasSet being false.
        /// </summary>
        /// <param name="propertyName">The name of the searched property (use <c>nameof(PROPERTY)</c>).</param>
        /// <param name="wasSet">Will be true if the property was found, otherwise false.</param>
        /// <returns></returns>
        public object? GetLocalValue(string propertyName, out bool wasSet)
        {
            if (_localValues.TryGetValue(propertyName, out object? value))
            {
                wasSet = true;
                return value;
            }

            wasSet = false;
            return null;
        }

        /// <summary>
        /// Applies the local theme; it should always be called after the global themes are applied. The local theme
        /// is represented by all the properties that are set by the user outside the global themes (the value overrides).
        /// </summary>
        private void ApplyLocalTheme()
        {
            LocalThemingFunction?.Invoke(State, this);

            foreach (KeyValuePair<string, object?> localValue in _localValues)
            {
                GetType().GetProperty(localValue.Key)?.SetValue(this, localValue.Value);
            }
        }

        /// <summary>
        /// This will only be called when the theme override is modified internally (like adding a new definition or
        /// updating an existing one). This will update any descendant of the given type, as well as this element if
        /// necessary.
        /// </summary>
        /// <param name="modifiedArgs"></param>
        private void OnThemeModified(Theme.ThemeModifiedArgs modifiedArgs)
        {
            if (modifiedArgs.ClassName != null)
            {
                ApplyClassTheme(modifiedArgs.ClassName);
            }
            else
            {
                ApplyElementTypeTheme(modifiedArgs.ElementType);
            }
        }


        /// <summary>
        /// Will invoke <see cref="ThemeDefinition.OnThemeChanged"/> on this element and its descendants if recursive
        /// is true.
        /// </summary>
        /// <param name="affectedType">
        /// If set, it will only check the elements with the given type; this is generally for performance reasons.
        /// </param>
        /// <param name="recursive">If true, will apply to descendants, otherwise it will only affect this element.</param>
        private void ApplyElementTypeTheme(Type? affectedType = null, bool recursive = true)
        {
            if (Document == null)
            {
                return;
            }

            _isLocalValueDictionaryLocked = true;

            try
            {
                //if the type is not set OR if it's the matching type
                if (affectedType == null || IsMatchingType(affectedType))
                {
                    List<int> indices = new(10);
                    Element currentElement = this;

                    if (!IgnoreGlobalTheming)
                    {
                        while (currentElement._parent != null)
                        {
                            indices.Add(currentElement.IndexInParent);
                            currentElement = currentElement._parent;
                        }

                        for (int i = indices.Count - 1; i >= 0; i--)
                        {
                            ApplyBaseThemingFromElement(currentElement);
                            currentElement = currentElement.Children[indices[i]];
                        }
                    }

                    //check self override
                    ApplyBaseThemingFromElement(currentElement);
                }

                if (!recursive)
                {
                    _isLocalValueDictionaryLocked = false;
                    return;
                }

                foreach (Element child in Children)
                {
                    child.ApplyElementTypeTheme();
                }
            }
            finally
            {
                _isLocalValueDictionaryLocked = false;
                ApplyLocalTheme();
            }
        }

        private bool IsMatchingType(Type affectedType)
        {
            if (BaseThemingCount == 0)
            {
                return GetType() == affectedType;
            }

            Type? currentType = GetType();
            while (currentType != affectedType)
            {
                currentType = currentType.BaseType;
                //reached Object
                if (currentType == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void ApplyBaseThemingFromElement(Element currentElement)
        {
            List<Type> baseClasses = [];

            if (BaseThemingCount != 0)
            {
                int baseThemingCount = BaseThemingCount < 0 ? int.MaxValue : BaseThemingCount;
                int i = 0;
                Type? baseType = GetType();
                while (i < baseThemingCount && baseType != null)
                {
                    baseClasses.Add(baseType);
                    if (baseType == typeof(Element))
                    {
                        break;
                    }

                    baseType = baseType.BaseType;
                    i++;
                }
            }
            else
            {
                baseClasses.Add(GetType());
            }

            for (int i = baseClasses.Count - 1; i >= 0; i--)
            {
                currentElement
                    .ThemeOverride
                    ?.GetElementTypeDefinition(baseClasses[i])
                    ?.InvokeOnThemeChanged(this);
            }
        }

        /// <summary>
        /// Applies the theme changes for the given style class to this element and all children if recursive is true.
        /// </summary>
        /// <param name="affectedClass"></param>
        /// <param name="recursive"></param>
        private void ApplyClassTheme(string affectedClass, bool recursive = true)
        {
            _isLocalValueDictionaryLocked = true;

            try
            {
                if (StyleClass == affectedClass)
                {
                    List<int> indices = new(10);
                    Element currentElement = this;

                    if (!IgnoreGlobalTheming)
                    {
                        while (currentElement._parent != null)
                        {
                            indices.Add(currentElement.IndexInParent);
                            currentElement = currentElement._parent;
                        }

                        for (int i = indices.Count - 1; i >= 0; i--)
                        {
                            currentElement.ThemeOverride?.GetClassDefinition(affectedClass)?.InvokeOnThemeChanged(this);
                            currentElement = currentElement.Children[indices[i]];
                        }
                    }

                    //check self override
                    currentElement.ThemeOverride?.GetClassDefinition(affectedClass)?.InvokeOnThemeChanged(this);
                }

                if (!recursive)
                {
                    _isLocalValueDictionaryLocked = false;
                    return;
                }

                foreach (Element child in Children)
                {
                    child.ApplyClassTheme(affectedClass);
                }
            }
            finally
            {
                _isLocalValueDictionaryLocked = false;
                ApplyLocalTheme();
            }
        }

        private void ApplyThemeStateChanges()
        {
            if (Document == null)
            {
                return;
            }

            List<int> indices = new(10);
            Element currentElement = this;

            while (currentElement._parent != null)
            {
                indices.Add(currentElement.IndexInParent);
                currentElement = currentElement._parent;
            }

            _isLocalValueDictionaryLocked = true;

            try
            {
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    currentElement.ThemeOverride?.GetElementTypeDefinition(GetType())
                                  ?.InvokeOnStateChanged(this, State);
                    currentElement.ThemeOverride?.GetClassDefinition(StyleClass)?.InvokeOnStateChanged(this, State);
                    currentElement = currentElement.Children[indices[i]];
                }

                //check self override
                currentElement.ThemeOverride?.GetElementTypeDefinition(GetType())?.InvokeOnStateChanged(this, State);
                currentElement.ThemeOverride?.GetClassDefinition(StyleClass)?.InvokeOnStateChanged(this, State);
            }
            finally
            {
                _isLocalValueDictionaryLocked = false;
                ApplyLocalTheme();
            }
        }
    }
}
