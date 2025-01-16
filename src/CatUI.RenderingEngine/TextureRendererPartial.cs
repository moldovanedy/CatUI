using System;
using CatUI.Data;
using CatUI.Data.Enums;
using SkiaSharp;

namespace CatUI.RenderingEngine
{
    public partial class Renderer
    {
        private const float EPSILON = 0.5f;

        /// <summary>
        /// Draws an image without any resizing, so it is significantly faster than <see cref="DrawImage"/>,
        /// but you can't specify the image's size, only the top-left corner position.
        /// </summary>
        /// <param name="image">The image to use.</param>
        /// <param name="position">The image's top-left corner coordinates.</param>
        /// <param name="paint">The paint to use, or null.</param>
        public void DrawImageFast(SKImage image, Point2D position, SKPaint? paint = null)
        {
            Canvas?.DrawImage(image, new SKPoint(position.X, position.Y), paint);
        }

        /// <summary>
        /// Allows you to specify the width and height of the final image that will be drawn, but will be slower because
        /// it needs to copy the entire image and then resize it. This won't modify the given image in any way.
        /// </summary>
        /// <param name="image">The image to draw. It won't be modified.</param>
        /// <param name="position">The image's top-left corner coordinates.</param>
        /// <param name="size">
        /// The desired size. Setting this to 0 or the image's width and height will not apply any resizing
        /// for faster drawing (so it behaves exactly like <see cref="DrawImageFast"/> in this case).
        /// </param>
        /// <param name="resizeQuality">The resizing quality. Higher quality means slower drawing.</param>
        /// <param name="paint">The paint to use, or null.</param>
        public void DrawImage(
            SKImage image,
            Point2D position,
            Size size,
            ImageResizeQuality resizeQuality,
            SKPaint? paint)
        {
            SKImage outputImage = image;
            if (size.Width > EPSILON && size.Height > EPSILON &&
                Math.Abs(size.Width - image.Width) > EPSILON &&
                Math.Abs(size.Height - image.Height) > EPSILON)
            {
                var newImageInfo = new SKImageInfo((int)size.Width, (int)size.Height, image.ColorType);
                outputImage = SKImage.Create(newImageInfo);
                bool success = image.ScalePixels(outputImage.PeekPixels(), (SKFilterQuality)resizeQuality);
                if (!success)
                {
                    throw new InvalidOperationException("Unable to resize image.");
                }
            }

            DrawImageFast(outputImage, new Point2D(position.X, position.Y), paint);
        }

        /// <summary>
        /// Draws a bitmap which is much slower than the <see cref="DrawImageFast"/> and <see cref="DrawImage"/>
        /// counterparts, but allows resizing. If markBitmapImmutable is true, the bitmap will no longer be modifiable,
        /// such as writing pixels or other operations. If markBitmapImmutable is false,
        /// there will be a performance penalty involved in drawing, but the bitmap remains ediatable.
        /// </summary>
        /// <param name="bitmap">The bitmap to draw.</param>
        /// <param name="position">The image's top-left corner coordinates.</param>
        /// <param name="size">
        /// The desired size. Setting this to 0 or the bitmap's width and height will not apply any resizing for faster drawing.
        /// </param>
        /// <param name="resizeQuality">The resizing quality. Higher quality means slower drawing.</param>
        /// <param name="paint">The paint to use, or null.</param>
        /// <param name="resultingGpuImage">
        /// The resulting SKImage that can be used for subsequent drawing operations if the original bitmap
        /// won't be modified in any way (using <see cref="DrawImageFast"/>).
        /// </param>
        /// <param name="makeBitmapImmutable">
        /// If true, the bitmap will no longer be modifiable after this operation, so you would have to create
        /// a new bitmap for modifications. However, setting this to true will result in a significantly faster draw operation.
        /// </param>
        public void DrawImageBitmap(
            SKBitmap bitmap,
            Point2D position,
            Size size,
            ImageResizeQuality resizeQuality,
            SKPaint? paint,
            out SKImage resultingGpuImage,
            bool makeBitmapImmutable = true)
        {
            if (size.Width > EPSILON && size.Height > EPSILON &&
                Math.Abs(size.Width - bitmap.Width) > EPSILON &&
                Math.Abs(size.Height - bitmap.Height) > EPSILON)
            {
                bitmap = bitmap.Resize(new SKImageInfo((int)size.Width, (int)size.Height),
                    (SKFilterQuality)resizeQuality);

                if (makeBitmapImmutable)
                {
                    bitmap.SetImmutable();
                }
            }

            resultingGpuImage = SKImage.FromBitmap(bitmap);

            //it seems DrawBitmap will actually internally convert the bitmap to SKImage, so just use DrawImage at this point
            //https://github.com/mono/SkiaSharp/issues/2188#issuecomment-1214725476
            DrawImageFast(resultingGpuImage, new Point2D(position.X, position.Y), paint);
        }
    }
}
