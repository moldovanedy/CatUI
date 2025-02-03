using SkiaSharp;

namespace CatUI.Data.Enums
{
    public enum LineCapType
    {
        Butt = 0,
        Round = 1,
        Square = 2
    }

    /// <summary>
    /// Represents the type of line join that is used when drawing paths or any geometry in general. You can see how these
    /// values work on the internet, as those are standard in 2D computer drawing and map 1:1 to <see cref="SKStrokeJoin"/>.
    /// </summary>
    /// <remarks>
    /// An especially valuable resource is lineJoin() property of CanvasRenderingContext2D on Mozilla's web docs.
    /// </remarks>
    public enum LineJoinType
    {
        /// <summary>
        /// The lines will have a sharp tip when they are joined.
        /// </summary>
        Miter = 0,

        /// <summary>
        /// The lines will have a rounded edge when they are joined.
        /// </summary>
        Round = 1,

        /// <summary>
        /// Similar to <see cref="Miter"/>, but more smooth as it adds some triangular areas to the join to make it
        /// more smooth. It looks like a <see cref="Miter"/> that is cut halfway, so it doesn't have a tip.
        /// </summary>
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

    /// <summary>
    /// Describes how will the image fit inside the element's space.
    /// </summary>
    public enum ImageFitType
    {
        /// <summary>
        /// The image won't be expanded in any way; it keeps its original size, even if it overflows the element bounds.
        /// This ensures no kind of image artifact will be present, but is generally inflexible.
        /// </summary>
        None = 0,

        /// <summary>
        /// The image can only be shrunk to have a smaller size (or keep its original size), but can't be expanded.
        /// This ensures that the image will not get blurry (because it can't have a size larger than the original one),
        /// but when the element's size is much smaller than the original size, it can create some other artifacts,
        /// like more ragged edges.
        /// </summary>
        CanShrink = 1,

        /// <summary>
        /// The image can only grow (expand, or keep its original size), but can't be shrunk. Any growth can create
        /// blurry images, the effect might not be noticeable on small expansions or if the <see cref="ImageResizeQuality"/>
        /// is set to high quality, but it is there.
        /// </summary>
        CanGrow = 2,

        /// <summary>
        /// The image can both shrink and grow (expand); this is the most flexible solution, but it comes with the
        /// disadvantages of both <see cref="CanShrink"/> and <see cref="CanGrow"/>.
        /// </summary>
        CanShrinkAndGrow = 3,

        /// <summary>
        /// The element's lower size (either the width or the height) will be considered as the image's lower size.
        /// This makes the image to always cover the entire element, but also makes it always overflow the container
        /// (unless the image is a square (width == height)). It always obeys the aspect ratio. It also allows the image
        /// to both grow and shrink past its original size, just like <see cref="CanShrinkAndGrow"/> and
        /// comes with the same disadvantages as that setting.
        /// </summary>
        Cover = 4
    }
}
