using System;
using CatUI.Data.Assets;
using SkiaSharp;

namespace CatUI.Windowing.Common
{
    /// <summary>
    /// Represents a window icon. Almost all platforms can retrieve this at runtime; some (like Windows, WASM,
    /// or Linux X11) will also allow setting this at runtime.
    /// </summary>
    public class WindowIcon
    {
        public ImageAsset? Icon16X16 { get; private set; }
        public ImageAsset? Icon32X32 { get; private set; }
        public ImageAsset? Icon48X48 { get; private set; }
        public ImageAsset? Icon64X64 { get; private set; }
        public ImageAsset? Icon128X128 { get; private set; }
        public ImageAsset? Icon256X256 { get; private set; }
        public ImageAsset? Icon512X512 { get; private set; }

        /// <summary>
        /// Constructs an icon from other images of different sizes. All are optional.
        /// </summary>
        /// <remarks>
        /// If the image is not square, it will get resized as a square, potentially making it look "squished".
        /// </remarks>
        /// <param name="icon16X16"></param>
        /// <param name="icon32X32"></param>
        /// <param name="icon48X48"></param>
        /// <param name="icon64X64"></param>
        /// <param name="icon128X128"></param>
        /// <param name="icon256X256"></param>
        /// <param name="icon512X512"></param>
        public WindowIcon(
            ImageAsset? icon16X16 = null,
            ImageAsset? icon32X32 = null,
            ImageAsset? icon48X48 = null,
            ImageAsset? icon64X64 = null,
            ImageAsset? icon128X128 = null,
            ImageAsset? icon256X256 = null,
            ImageAsset? icon512X512 = null)
        {
            Icon16X16 = ResizeImage(icon16X16, 16);
            Icon32X32 = ResizeImage(icon32X32, 32);
            Icon48X48 = ResizeImage(icon48X48, 48);
            Icon64X64 = ResizeImage(icon64X64, 64);
            Icon128X128 = ResizeImage(icon128X128, 128);
            Icon256X256 = ResizeImage(icon256X256, 256);
            Icon512X512 = ResizeImage(icon512X512, 512);
        }

        /// <summary>
        /// Constructs an icon from a single image but creates resized copies for all sizes. If noUpscaling is true,
        /// it only creates downscaled copies (e.g. for a 54x54 image, will create for 48x48, 32x32, and 16x16).
        /// </summary>
        /// <remarks>
        /// If the image is not square, it will get resized as a square, potentially making it look "squished".
        /// </remarks>
        /// <param name="image"></param>
        /// <param name="noUpscaling">
        /// If true, will only create downscaled icons, otherwise will also create upscaled icons.
        /// </param>
        public WindowIcon(ImageAsset image, bool noUpscaling = false)
        {
            float referenceSize = Math.Min(image.Width, image.Height);
            if (!noUpscaling || referenceSize >= 16)
            {
                Icon16X16 = ResizeImage(image, 16);
            }

            if (!noUpscaling || referenceSize >= 32)
            {
                Icon32X32 = ResizeImage(image, 32);
            }

            if (!noUpscaling || referenceSize >= 48)
            {
                Icon48X48 = ResizeImage(image, 48);
            }

            if (!noUpscaling || referenceSize >= 64)
            {
                Icon64X64 = ResizeImage(image, 64);
            }

            if (!noUpscaling || referenceSize >= 128)
            {
                Icon128X128 = ResizeImage(image, 128);
            }

            if (!noUpscaling || referenceSize >= 256)
            {
                Icon256X256 = ResizeImage(image, 256);
            }

            if (!noUpscaling || referenceSize >= 512)
            {
                Icon512X512 = ResizeImage(image, 512);
            }
        }

        private static ImageAsset? ResizeImage(ImageAsset? image, int size)
        {
            const float epsilon = 0.5f;
            if (image?.SkiaImage == null)
            {
                return null;
            }

            if (Math.Abs(image.Width - size) <= epsilon &&
                Math.Abs(image.Height - size) <= epsilon)
            {
                return image;
            }

            var newImageInfo = new SKImageInfo(
                (int)MathF.Round(size, MidpointRounding.ToZero),
                (int)MathF.Round(size, MidpointRounding.ToZero),
                image.SkiaImage.ColorType);

            var outputImage = SKImage.Create(newImageInfo);
            bool success = image.SkiaImage.ScalePixels(outputImage.PeekPixels(), SKFilterQuality.High);
            return success
                ? new ImageAsset(outputImage)
                : throw new InvalidOperationException("Unable to resize image.");
        }
    }
}
