using CatUI.Data.Assets;
using SkiaSharp;

namespace CatUI.Data.Theming.Typography
{
    public static class FontUtility
    {
        /// <summary>
        /// Returns the platform's default font. This is used by default on every text inside your UI when you don't
        /// override it in an element or in a theme override.
        /// </summary>
        public static FontAsset DefaultPlatformFont { get; } = new(SKTypeface.Default);

        /// <summary>
        /// Returns the font that has the given family name (e.g. Arial, Times New Roman) or the closest-matching one
        /// (e.g. can't find Times New Roman, so it will return a generic serif font). There is no way to detect if
        /// the font is the requested one or not, but it's certain that this will always return a valid font.
        /// </summary>
        /// <remarks>
        /// Avoid using generic names like serif or sans-serif as they might not work properly. There is no single font
        /// that works on all platforms (except the ones provided by your app), so you can't be sure the searched family
        /// name will return the same font on all platforms. Cache the results instead of repeatedly calling this method.
        /// </remarks>
        /// <param name="familyName">The family name of the font you want to use.</param>
        /// <returns>The searched font if it exists or the closest-matching one.</returns>
        public static FontAsset SearchSystemFonts(string familyName)
        {
            return new FontAsset(SKTypeface.FromFamilyName(familyName));
        }

        /// <inheritdoc cref="SearchSystemFonts(string)"/>
        /// <param name="preferredWeight">
        /// The preferred font weight. Generally <see cref="FontWeightPreset.Normal"/> or <see cref="FontWeightPreset.Bold"/>.
        /// </param>
        /// <param name="preferredWidth">
        /// The preferred font width. Generally <see cref="FontWidthPreset.Normal"/> or <see cref="FontWidthPreset.Condensed"/>.
        /// </param>
        /// <param name="preferredSlant">
        /// The preferred font slant. Generally <see cref="FontSlantPreset.None"/> or <see cref="FontSlantPreset.Italic"/>.
        /// </param>
        public static FontAsset SearchSystemFonts(
            // ReSharper disable once InvalidXmlDocComment
            string familyName,
            FontWeightPreset preferredWeight,
            FontWidthPreset preferredWidth,
            FontSlantPreset preferredSlant)
        {
            return new FontAsset(
                SKTypeface.FromFamilyName(
                    familyName,
                    (SKFontStyleWeight)preferredWeight,
                    (SKFontStyleWidth)preferredWidth,
                    (SKFontStyleSlant)preferredSlant));
        }
    }
}
