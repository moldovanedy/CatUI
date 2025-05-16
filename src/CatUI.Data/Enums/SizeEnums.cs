namespace CatUI.Data.Enums
{
    public enum Unit
    {
        /// <summary>
        /// Device independent pixels. This value is scaled according to the device size and, more importantly, the
        /// user's preference. In almost all cases, you should use this instead of <see cref="Pixels"/> because this
        /// will generally look good on any device.
        /// </summary>
        Dp = 0,

        /// <summary>
        /// Actual screen pixels. No scaling applied. In almost all cases, you should use <see cref="Dp"/> instead of this
        /// because this is not scaled and the content might be too small or too large when using the app on another device.
        /// </summary>
        Pixels = 1,

        /// <summary>
        /// Relative to the parent element. From 0 to 100.
        /// </summary>
        Percent = 2,

        /// <summary>
        /// Relative to the viewport width (in most cases, the window width). From 0 to 100.
        /// </summary>
        ViewportWidth = 3,

        /// <summary>
        /// Relative to the viewport height (in most cases, the window height). From 0 to 100.
        /// </summary>
        ViewportHeight = 4,

        /// <summary>
        /// Factor of the root em font size (set in UiDocument). Must be greater than 0. You should use this only for
        /// font sizes.
        /// </summary>
        /// <example>
        /// 1 em means 1 * RootEmSize, 1.5em means 1.5 * RootEmSize, so in these examples, if RootEmSize is 16 dp
        /// (always set it in dp, not px or anything else), then you'll have 16 dp and 24 dp respectively.
        /// </example>
        Em = 5
    }
}
