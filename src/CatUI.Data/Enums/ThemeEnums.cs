namespace CatUI.Data.Enums
{
    /// <summary>
    /// Specifies the preferred contrast between colors, as determined by the theme.
    /// </summary>
    public enum ColorContrastMode
    {
        /// <summary>
        /// The normal contrast. This will generally be used by most users.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// A bigger contrast than <see cref="Standard"/>. Rarely implemented by platforms (only Android 14 and newer
        /// (API >= 34)), but can be set by your code on andy platform.
        /// </summary>
        Medium = 1,

        /// <summary>
        /// High contrast. This provides even higher contrast than <see cref="Medium"/> and is generally implemented
        /// by all platforms.
        /// </summary>
        High = 2
    }
}
