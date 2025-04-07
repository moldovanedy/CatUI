using CatUI.Data.Enums;
using CatUI.Data.Theming.ClipShapes;
using CatUI.Data.Theming.Colors;
using CatUI.Data.Theming.Typography;

namespace CatUI.Data.Theming
{
    public static class CatTheme
    {
        private static bool _initializationGuard;

        public static CatThemeColors Colors { get; } = new();

        public static CatThemeTypography Typography { get; internal set; } = new();

        public static CatThemeClipShapes ClipShapes { get; internal set; } = new();

        /// <summary>
        /// Represents the theme "settings", which affect how the theme returns the colors.
        /// For example, setting <see cref="CatThemeSettings.IsDarkModeEnabled"/> will affect the actual value from
        /// <see cref="DarkModeValue"/> and the colors from <see cref="Colors"/>.
        /// </summary>
        public static CatThemeSettings Settings { get; } = new();

        /// <summary>
        /// Returns true if dark mode is used, false if not. To control this behavior, see
        /// <see cref="CatThemeSettings.IsDarkModeEnabled"/>.
        /// </summary>
        public static bool DarkModeValue
        {
            get
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (Settings.IsDarkModeEnabled)
                {
                    case PlatformOption.Enabled:
                        return true;
                    case PlatformOption.Disabled:
                        return false;
                }

                bool? platformValue = CatApplication.Instance.PlatformUiOptions.IsDarkModeEnabled;
                if (platformValue.HasValue)
                {
                    return platformValue.Value;
                }

                return Settings.IsDarkModeEnabled == PlatformOption.PlatformDependentFallbackEnabled;
            }
        }

        /// <summary>
        /// Returns the color contrast that is used. To control this, see <see cref="CatThemeSettings.Contrast"/>.
        /// </summary>
        public static ColorContrastMode ContrastValue
        {
            get
            {
                if (Settings.Contrast.PrefersPlatformOption)
                {
                    int? contrast = CatApplication.Instance.PlatformUiOptions.ColorContrast;
                    if (contrast.HasValue)
                    {
                        switch (contrast.Value)
                        {
                            case 1:
                                return ColorContrastMode.Medium;
                            case 2:
                                return ColorContrastMode.High;
                            default:
                                return ColorContrastMode.Standard;
                        }
                    }
                }

                return Settings.Contrast.FallbackValue;
            }
        }

        internal static void Initialize()
        {
            if (_initializationGuard)
            {
                return;
            }

            _initializationGuard = true;
            ColorProvider.Initialize();
        }
    }
}
