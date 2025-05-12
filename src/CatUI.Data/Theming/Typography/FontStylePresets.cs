namespace CatUI.Data.Theming.Typography
{
    /// <summary>
    /// Provides the font weight presets that are generally used in typography. This specifies the "thickness" of the
    /// text.
    /// </summary>
    public enum FontWeightPreset
    {
        Invalid = -1,
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900,
        ExtraBlack = 1000
    }

    /// <summary>
    /// Provides the font width presets that are generally used in typography. This specifies the general spacing
    /// between the letters.
    /// </summary>
    public enum FontWidthPreset
    {
        Invalid = -1,
        UltraCondensed = 1,
        ExtraCondensed = 2,
        Condensed = 3,
        SemiCondensed = 4,
        Normal = 5,
        SemiExpanded = 6,
        Expanded = 7,
        ExtraExpanded = 8,
        UltraExpanded = 9
    }

    /// <summary>
    /// Provides the font slant presets that are generally used in typography. This specifies the incline of the letters.
    /// </summary>
    public enum FontSlantPreset
    {
        Invalid = -1,

        /// <summary>
        /// Or upright. Means the text won't be slanted (inclined).
        /// </summary>
        None = 0,

        /// <summary>
        /// Italic is slanted naturally (by the font creators).
        /// </summary>
        Italic = 1,

        /// <summary>
        /// Oblique is slanted artificially by the renderer (i.e. it's not slanted by default inside the font data).
        /// </summary>
        Oblique = 2
    }
}
