using System;
using System.Collections.Generic;
using CatUI.Data;

namespace CatUI.Elements
{
    public partial class Element
    {
        public Theme? ThemeOverride
        {
            get => _themeOverride;
            set
            {
                SetThemeOverride(value);
                ThemeOverrideProperty.Value = value;
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
                SetStyleClass(value);
                StyleClassProperty.Value = value;
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
                SetBaseThemingCount(value);
                BaseThemingCountProperty.Value = value;
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
                SetIgnoreGlobalTheming(value);
                IgnoreGlobalThemingProperty.Value = value;
            }
        }

        private bool _ignoreGlobalTheming;
        public ObservableProperty<bool> IgnoreGlobalThemingProperty { get; } = new(false);

        private void SetIgnoreGlobalTheming(bool value)
        {
            _ignoreGlobalTheming = value;
            ApplyElementTypeTheme();
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
                return;
            }

            foreach (Element child in Children)
            {
                child.ApplyElementTypeTheme();
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
                return;
            }

            foreach (Element child in Children)
            {
                child.ApplyClassTheme(affectedClass);
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

            for (int i = indices.Count - 1; i >= 0; i--)
            {
                currentElement.ThemeOverride?.GetElementTypeDefinition(GetType())?.InvokeOnStateChanged(this, State);
                currentElement.ThemeOverride?.GetClassDefinition(StyleClass)?.InvokeOnStateChanged(this, State);
                currentElement = currentElement.Children[indices[i]];
            }

            //check self override
            currentElement.ThemeOverride?.GetElementTypeDefinition(GetType())?.InvokeOnStateChanged(this, State);
            currentElement.ThemeOverride?.GetClassDefinition(StyleClass)?.InvokeOnStateChanged(this, State);
        }
    }
}
