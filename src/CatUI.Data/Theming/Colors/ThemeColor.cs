using CatUI.Data.Enums;

namespace CatUI.Data.Theming.Colors
{
    /// <summary>
    /// Holds color values for all combinations of theme (dark/light) and contrast (standard/medium/high).
    /// At runtime, the suitable one will be picked and used in <see cref="CatThemeColors"/>, so this is just a data
    /// container. Medium contrast can be set by your code on any platform, but getting it as a platform option is only
    /// available on Android 14 and newer (API >= 34) at the moment.
    /// </summary>
    /// <remarks>
    /// This is heavily inspired by Material Design 3 by Google. Use of Material Design 3 is not forced by CatUI, nor
    /// is it discouraged or encouraged.
    /// </remarks>
    public class ThemeColor
    {
        /// <summary>
        /// The color used when dark mode is enabled and standard contrast is set. This is the default color for dark mode
        /// and will be used when other contrast colors are unset. Failing to set this will fall back to black.
        /// </summary>
        public Color? DarkStandardContrast { get; }

        /// <summary>
        /// The color used when dark mode is enabled and medium contrast is set. This is available on any platform when
        /// you set the contrast directly, but when you use the platform options, this will only be available on Android
        /// 14 and newer (API >= 34). If this is unset (or platform unavailable), it will fall back to
        /// <see cref="DarkStandardContrast"/>.
        /// </summary>
        public Color? DarkMediumContrast { get; }

        /// <summary>
        /// The color used when dark mode is enabled and high contrast is set. This is available on any platform when
        /// you set the contrast directly, but when you use the platform options, this should be available on most
        /// platforms (if CatUI has a platform implementation to get that option). If this is unset (or platform
        /// unavailable), it will fall back to <see cref="DarkStandardContrast"/>.
        /// </summary>
        public Color? DarkHighContrast { get; }

        /// <summary>
        /// The color used when light mode is enabled and standard contrast is set. This is the default color for light
        /// mode and will be used when other contrast colors are unset. Failing to set this will fall back to white.
        /// </summary>
        public Color? LightStandardContrast { get; }

        /// <summary>
        /// The color used when light mode is enabled and medium contrast is set. This is available on any platform when
        /// you set the contrast directly, but when you use the platform options, this will only be available on Android
        /// 14 and newer (API >= 34). If this is unset (or platform unavailable), it will fall back to
        /// <see cref="LightStandardContrast"/>.
        /// </summary>
        public Color? LightMediumContrast { get; }

        /// <summary>
        /// The color used when dark mode is enabled and high contrast is set. This is available on any platform when
        /// you set the contrast directly, but when you use the platform options, this should be available on most
        /// platforms (if CatUI has a platform implementation to get that option). If this is unset (or platform
        /// unavailable), it will fall back to <see cref="LightStandardContrast"/>.
        /// </summary>
        public Color? LightHighContrast { get; }

        /// <summary>
        /// Sets a single color for both dark and light mode and all contrast options.
        /// </summary>
        /// <param name="color"></param>
        public ThemeColor(Color color)
        {
            LightStandardContrast = color;
            DarkStandardContrast = color;
        }

        /// <summary>
        /// Sets a light color and a dark color for all contrast options.
        /// </summary>
        /// <param name="lightColor"></param>
        /// <param name="darkColor"></param>
        public ThemeColor(Color lightColor, Color darkColor)
        {
            LightStandardContrast = lightColor;
            DarkStandardContrast = darkColor;
        }

        /// <summary>
        /// Sets a light color and a dark color for both standard contrast and high contrast.
        /// </summary>
        /// <param name="lightStandardContrastColor"></param>
        /// <param name="darkStandardContrastColor"></param>
        /// <param name="lightHighContrastColor"></param>
        /// <param name="darkHighContrastColor"></param>
        public ThemeColor(
            Color lightStandardContrastColor,
            Color darkStandardContrastColor,
            Color lightHighContrastColor,
            Color darkHighContrastColor)
        {
            LightStandardContrast = lightStandardContrastColor;
            DarkStandardContrast = darkStandardContrastColor;
            LightHighContrast = lightHighContrastColor;
            DarkHighContrast = darkHighContrastColor;
        }

        /// <summary>
        /// Sets a light color and a dark color for all contrast options: standard, medium and high.
        /// </summary>
        /// <param name="lightStandardContrastColor"></param>
        /// <param name="darkStandardContrastColor"></param>
        /// <param name="lightMediumContrastColor"></param>
        /// <param name="darkMediumContrastColor"></param>
        /// <param name="lightHighContrastColor"></param>
        /// <param name="darkHighContrastColor"></param>
        public ThemeColor(
            Color lightStandardContrastColor,
            Color darkStandardContrastColor,
            Color lightMediumContrastColor,
            Color darkMediumContrastColor,
            Color lightHighContrastColor,
            Color darkHighContrastColor)
        {
            LightStandardContrast = lightStandardContrastColor;
            DarkStandardContrast = darkStandardContrastColor;
            LightMediumContrast = lightMediumContrastColor;
            DarkMediumContrast = darkMediumContrastColor;
            LightHighContrast = lightHighContrastColor;
            DarkHighContrast = darkHighContrastColor;
        }

        public Color GetAppropriateColor(bool isDarkModeEnabled, ColorContrastMode contrast = 0)
        {
            switch (contrast)
            {
                default:
                case ColorContrastMode.Standard:
                    return isDarkModeEnabled
                        ? DarkStandardContrast ?? new Color(0)
                        : LightStandardContrast ?? new Color(0xff_ff_ff);
                case ColorContrastMode.Medium:
                    return isDarkModeEnabled
                        ? DarkMediumContrast ?? DarkStandardContrast ?? new Color(0)
                        : LightMediumContrast ?? LightStandardContrast ?? new Color(0xff_ff_ff);
                case ColorContrastMode.High:
                    return isDarkModeEnabled
                        ? DarkHighContrast ?? DarkStandardContrast ?? new Color(0)
                        : LightHighContrast ?? LightStandardContrast ?? new Color(0xff_ff_ff);
            }
        }
    }
}
