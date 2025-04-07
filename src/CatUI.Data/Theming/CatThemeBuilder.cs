using System.Collections.Generic;
using CatUI.Data.Shapes;
using CatUI.Data.Theming.ClipShapes;
using CatUI.Data.Theming.Colors;
using CatUI.Data.Theming.Typography;

namespace CatUI.Data.Theming
{
    /// <summary>
    /// A utility that sets the given values to <see cref="CatTheme"/>. Don't forget to call <see cref="ApplyTheme"/>
    /// to actually set the given values.
    /// </summary>
    public static class CatThemeBuilder
    {
        private static Dictionary<string, ThemeColor>? _colors;
        private static CatThemeTypography? _typography;
        private static CatThemeClipShapes? _shapes;

        /// <summary>
        /// Sets the theme colors based on the given dictionary.
        /// </summary>
        /// <param name="colors">
        /// A dictionary where the key is nameof(&lt;color from <see cref="CatThemeColors"/>&gt;), the value is the
        /// <see cref="ThemeColor"/> for the specified color name.
        /// </param>
        /// <remarks>
        /// This will override what <see cref="SetColors"/> set before, so only use one of these methods.
        /// </remarks>
        public static void SetColorsFromDictionary(Dictionary<string, ThemeColor> colors)
        {
            //we do this so it won't save the changes until ApplyTheme is called
            _colors = colors;
        }

        /// <summary>
        /// Set the colors for each token. All arguments are optional, and you can use C# named arguments
        /// when setting only some of the tokens. Unset tokens will have the default value.
        /// </summary>
        /// <remarks>
        /// This will override what <see cref="SetColorsFromDictionary"/> set before, so only use one of these methods.
        /// </remarks>
        public static void SetColors(
            ThemeColor? primary = null,
            ThemeColor? onPrimary = null,
            ThemeColor? primaryContainer = null,
            ThemeColor? onPrimaryContainer = null,
            //
            ThemeColor? secondary = null,
            ThemeColor? onSecondary = null,
            ThemeColor? secondaryContainer = null,
            ThemeColor? onSecondaryContainer = null,
            //
            ThemeColor? tertiary = null,
            ThemeColor? onTertiary = null,
            ThemeColor? tertiaryContainer = null,
            ThemeColor? onTertiaryContainer = null,
            //
            ThemeColor? error = null,
            ThemeColor? onError = null,
            ThemeColor? errorContainer = null,
            ThemeColor? onErrorContainer = null,
            //
            ThemeColor? success = null,
            ThemeColor? onSuccess = null,
            ThemeColor? successContainer = null,
            ThemeColor? onSuccessContainer = null,
            //
            ThemeColor? surface = null,
            ThemeColor? surfaceDim = null,
            ThemeColor? surfaceBright = null,
            ThemeColor? surfaceContainerLowest = null,
            ThemeColor? surfaceContainerHighest = null,
            ThemeColor? surfaceContainerLow = null,
            ThemeColor? surfaceContainerHigh = null,
            ThemeColor? surfaceContainer = null,
            //
            ThemeColor? onSurface = null,
            ThemeColor? onSurfaceVariant = null,
            //
            ThemeColor? inverseSurface = null,
            ThemeColor? inverseOnSurface = null,
            ThemeColor? inversePrimary = null,
            //
            ThemeColor? outline = null,
            ThemeColor? outlineVariant = null,
            ThemeColor? scrim = null,
            ThemeColor? shadow = null)
        {
            //we do this so it won't save the changes until ApplyTheme is called
            _colors = new Dictionary<string, ThemeColor>();

            //primary
            if (primary != null)
            {
                _colors.Add(nameof(CatThemeColors.Primary), primary);
            }

            if (onPrimary != null)
            {
                _colors.Add(nameof(CatThemeColors.OnPrimary), onPrimary);
            }

            if (primaryContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.PrimaryContainer), primaryContainer);
            }

            if (onPrimaryContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.OnPrimaryContainer), onPrimaryContainer);
            }

            //secondary
            if (secondary != null)
            {
                _colors.Add(nameof(CatThemeColors.Secondary), secondary);
            }

            if (onSecondary != null)
            {
                _colors.Add(nameof(CatThemeColors.OnSecondary), onSecondary);
            }

            if (secondaryContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.SecondaryContainer), secondaryContainer);
            }

            if (onSecondaryContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.OnSecondaryContainer), onSecondaryContainer);
            }

            //tertiary
            if (tertiary != null)
            {
                _colors.Add(nameof(CatThemeColors.Tertiary), tertiary);
            }

            if (onTertiary != null)
            {
                _colors.Add(nameof(CatThemeColors.OnTertiary), onTertiary);
            }

            if (tertiaryContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.TertiaryContainer), tertiaryContainer);
            }

            if (onTertiaryContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.OnTertiaryContainer), onTertiaryContainer);
            }

            //error
            if (error != null)
            {
                _colors.Add(nameof(CatThemeColors.Error), error);
            }

            if (onError != null)
            {
                _colors.Add(nameof(CatThemeColors.OnError), onError);
            }

            if (errorContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.ErrorContainer), errorContainer);
            }

            if (onErrorContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.OnErrorContainer), onErrorContainer);
            }

            //success
            if (success != null)
            {
                _colors.Add(nameof(CatThemeColors.Success), success);
            }

            if (onSuccess != null)
            {
                _colors.Add(nameof(CatThemeColors.OnSuccess), onSuccess);
            }

            if (successContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.SuccessContainer), successContainer);
            }

            if (onSuccessContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.OnSuccessContainer), onSuccessContainer);
            }

            //BG surface
            if (surface != null)
            {
                _colors.Add(nameof(CatThemeColors.Surface), surface);
            }

            if (surfaceDim != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceDim), surfaceDim);
            }

            if (surfaceBright != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceBright), surfaceBright);
            }

            if (surfaceContainerLowest != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceContainerLowest), surfaceContainerLowest);
            }

            if (surfaceContainerLow != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceContainerLow), surfaceContainerLow);
            }

            if (surfaceContainer != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceContainer), surfaceContainer);
            }

            if (surfaceContainerHigh != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceContainerHigh), surfaceContainerHigh);
            }

            if (surfaceContainerHighest != null)
            {
                _colors.Add(nameof(CatThemeColors.SurfaceContainerHighest), surfaceContainerHighest);
            }

            //FG surface
            if (onSurface != null)
            {
                _colors.Add(nameof(CatThemeColors.OnSurface), onSurface);
            }

            if (onSurfaceVariant != null)
            {
                _colors.Add(nameof(CatThemeColors.OnSurfaceVariant), onSurfaceVariant);
            }

            //inverse
            if (inverseSurface != null)
            {
                _colors.Add(nameof(CatThemeColors.InverseSurface), inverseSurface);
            }

            if (inverseOnSurface != null)
            {
                _colors.Add(nameof(CatThemeColors.InverseOnSurface), inverseOnSurface);
            }

            if (inversePrimary != null)
            {
                _colors.Add(nameof(CatThemeColors.InversePrimary), inversePrimary);
            }

            //misc
            if (outline != null)
            {
                _colors.Add(nameof(CatThemeColors.Outline), outline);
            }

            if (outlineVariant != null)
            {
                _colors.Add(nameof(CatThemeColors.OutlineVariant), outlineVariant);
            }

            if (scrim != null)
            {
                _colors.Add(nameof(CatThemeColors.Scrim), scrim);
            }

            if (shadow != null)
            {
                _colors.Add(nameof(CatThemeColors.Shadow), shadow);
            }
        }

        /// <summary>
        /// Set the typography rules for each token. All arguments are optional, and you can use C# named arguments
        /// when setting only some of the tokens. Unset tokens will have the default value.
        /// </summary>
        public static void SetTypographyRules(
            ThemeTextStyle? displayLarge = null,
            ThemeTextStyle? displayMedium = null,
            ThemeTextStyle? displaySmall = null,
            //
            ThemeTextStyle? headingLarge = null,
            ThemeTextStyle? headingMedium = null,
            ThemeTextStyle? headingSmall = null,
            //
            ThemeTextStyle? bodyLarge = null,
            ThemeTextStyle? bodyMedium = null,
            ThemeTextStyle? bodySmall = null,
            //
            ThemeTextStyle? labelLarge = null,
            ThemeTextStyle? labelMedium = null,
            ThemeTextStyle? labelSmall = null)
        {
            _typography = new CatThemeTypography();
            if (displayLarge != null)
            {
                _typography.DisplayLarge = displayLarge;
            }

            if (displayMedium != null)
            {
                _typography.DisplayMedium = displayMedium;
            }

            if (displaySmall != null)
            {
                _typography.DisplaySmall = displaySmall;
            }

            if (headingLarge != null)
            {
                _typography.HeadingLarge = headingLarge;
            }

            if (headingMedium != null)
            {
                _typography.HeadingMedium = headingMedium;
            }

            if (headingSmall != null)
            {
                _typography.HeadingSmall = headingSmall;
            }

            if (bodyLarge != null)
            {
                _typography.BodyLarge = bodyLarge;
            }

            if (bodyMedium != null)
            {
                _typography.BodyMedium = bodyMedium;
            }

            if (bodySmall != null)
            {
                _typography.BodySmall = bodySmall;
            }

            if (labelLarge != null)
            {
                _typography.LabelLarge = labelLarge;
            }

            if (labelMedium != null)
            {
                _typography.LabelMedium = labelMedium;
            }

            if (labelSmall != null)
            {
                _typography.LabelSmall = labelSmall;
            }
        }

        /// <summary>
        /// Set the clip shapes for each token. All arguments are optional, and you can use C# named arguments
        /// when setting only some of the tokens. Unset tokens will have the default value.
        /// </summary>
        public static void SetClipShapes(
            RoundedRectangleClipShape? smallRounding = null,
            RoundedRectangleClipShape? mediumRounding = null,
            RoundedRectangleClipShape? largeRounding = null,
            RoundedRectangleClipShape? xlRounding = null)
        {
            _shapes = new CatThemeClipShapes();
            if (smallRounding != null)
            {
                _shapes.SmallRounding = smallRounding;
            }

            if (mediumRounding != null)
            {
                _shapes.MediumRounding = mediumRounding;
            }

            if (largeRounding != null)
            {
                _shapes.LargeRounding = largeRounding;
            }

            if (xlRounding != null)
            {
                _shapes.XlRounding = xlRounding;
            }
        }

        /// <summary>
        /// Applies the given values. Only the values set will be used, the other ones won't affect the <see cref="CatTheme"/>.
        /// After this, all the internal data will be reset.
        /// </summary>
        public static void ApplyTheme()
        {
            if (_colors != null)
            {
                ColorProvider.SetColorPrototypes(_colors);
                _colors = null;
            }

            if (_typography != null)
            {
                CatTheme.Typography = _typography;
                _typography = null;
            }

            if (_shapes != null)
            {
                CatTheme.ClipShapes = _shapes;
                _shapes = null;
            }
        }
    }
}
