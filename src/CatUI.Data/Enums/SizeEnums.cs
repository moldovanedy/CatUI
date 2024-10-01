namespace CatUI.Data.Enums
{
    public enum Unit
    {
        /// <summary>
        /// Device independent pixels. Corresponds to 1/160 inches.
        /// </summary>
        Dp = 0,
        /// <summary>
        /// Actual screen pixels. No scaling applied.
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
    }
}
