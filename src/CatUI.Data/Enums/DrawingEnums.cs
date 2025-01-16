namespace CatUI.Data.Enums
{
    public enum LineCapType
    {
        Butt = 0,
        Round = 1,
        Square = 2
    }

    public enum LineJoinType
    {
        Miter = 0,
        Round = 1,
        Bevel = 2
    }

    /// <summary>
    /// Specifies the image quality when resizing. Higher quality will take longer and might affect your app performance.
    /// </summary>
    public enum ImageResizeQuality
    {
        /// <summary>
        /// Uses nearest-neighbour sampling. Very fast, but very low quality.
        /// </summary>
        VeryLow = 0,

        /// <summary>
        /// Bilinear filtering. Fast, but low quality.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Bilinear filtering with mipmaps (fast drawing when downscaling, slower on upscaling). Medium quality.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Bicubic filtering with mipmaps (fast drawing when downscaling, very slow on upscaling). Highest quality.
        /// </summary>
        High = 3
    }
}
