using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace CatUI.Data.Theming.Colors
{
    /// <summary>
    /// Holds the color prototypes that will be used to set the colors in <see cref="CatThemeColors"/> when needed
    /// (the theme (dark/light) or contrast changes). You can set other colors from here, as well as update the existing
    /// ones, although it's generally more appropriate to set your theme colors once at startup using
    /// <see cref="CatThemeBuilder"/> and use those values throughout your app lifecycle.
    /// </summary>
    public static class ColorProvider
    {
        private static bool _initializationGuard;

        /// <summary>
        /// The key is nameof(&lt;color from <see cref="CatThemeColors"/>&gt;), the value is the <see cref="ThemeColor"/>
        /// for that token.
        /// </summary>
        private static readonly Dictionary<string, ThemeColor> _colorPrototypes = new();

        internal static void Initialize()
        {
            if (_initializationGuard)
            {
                return;
            }

            _initializationGuard = true;
            CatApplication.Instance.PlatformUiOptions.IsDarkModeEnabledChanged += _ => UpdateTheme();
            CatApplication.Instance.PlatformUiOptions.ColorContrastChanged += _ => UpdateTheme();
            CatTheme.Settings.PropertyChanged += OnSettingsChanged;
        }

        private static void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (
                e.PropertyName == nameof(CatThemeSettings.IsDarkModeEnabled) ||
                e.PropertyName == nameof(CatThemeSettings.Contrast))
            {
                UpdateTheme();
            }
        }

        /// <summary>
        /// Sets the color prototypes from which the final color will be retrieved by <see cref="CatThemeColors"/>
        /// depending on whether dark mode is enabled or not and contrast values (either controlled by the platform
        /// if available or by you).
        /// </summary>
        /// <param name="colors">
        /// A dictionary where the key is nameof(&lt;color from <see cref="CatThemeColors"/>&gt;), the value is the
        /// <see cref="ThemeColor"/> for the specified color name.
        /// </param>
        /// <param name="clearExisting">
        /// If true, will clear any existing color prototypes, otherwise it will just update existing values or add the
        /// new ones
        /// </param>
        /// <example>
        /// { nameof(CatThemeColors.Primary), new ThemeColor() {...}}, <br/>
        /// { nameof(CatThemeColors.Secondary), new ThemeColor() {...}},...
        /// </example>
        public static void SetColorPrototypes(Dictionary<string, ThemeColor> colors, bool clearExisting = true)
        {
            if (clearExisting)
            {
                _colorPrototypes.Clear();
            }

            foreach (KeyValuePair<string, ThemeColor> color in colors)
            {
                SetColorPrototype(color.Key, color.Value);
            }
        }

        /// <summary>
        /// Same as <see cref="SetColorPrototypes"/>, but just for one property.
        /// </summary>
        /// <param name="key">The property name: nameof(&lt;color from <see cref="CatThemeColors"/>&gt;)</param>
        /// <param name="color">The color prototype.</param>
        public static void SetColorPrototype(string key, ThemeColor color)
        {
            _colorPrototypes[key] = color;
            UpdateCatThemeColor(key, color);
        }

        private static void UpdateTheme()
        {
            foreach (KeyValuePair<string, ThemeColor> colorPrototype in _colorPrototypes)
            {
                UpdateCatThemeColor(colorPrototype.Key, colorPrototype.Value);
            }
        }

        private static void UpdateCatThemeColor(string token, ThemeColor prototype)
        {
            //get the color property and update its value
            Type type = CatTheme.Colors.GetType();
            PropertyInfo? property = type.GetProperty(token);
            property?.SetValue(
                CatTheme.Colors,
                prototype.GetAppropriateColor(CatTheme.DarkModeValue, CatTheme.ContrastValue));
        }
    }
}
