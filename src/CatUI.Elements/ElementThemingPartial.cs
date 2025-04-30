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

        private void SetStyleClass(string value)
        {
            _styleClass = value;
            ApplyClassTheme(value, false);
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
            if (affectedType == null || GetType() == affectedType)
            {
                List<int> indices = new(10);
                Element currentElement = this;

                while (currentElement._parent != null)
                {
                    indices.Add(currentElement.IndexInParent);
                    currentElement = currentElement._parent;
                }

                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    currentElement.ThemeOverride?.GetElementTypeDefinition(GetType())?.InvokeOnThemeChanged(this);
                    currentElement = currentElement.Children[indices[i]];
                }

                //check self override
                currentElement.ThemeOverride?.GetElementTypeDefinition(GetType())?.InvokeOnThemeChanged(this);
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
