using System.IO;
using SkiaSharp;

namespace CatUI.Data.Assets
{
    public class Image : Asset
    {
        public SKImage? SkiaImage { get; private set; }

        public float Width => SkiaImage?.Width ?? 0;
        public float Height => SkiaImage?.Height ?? 0;

        public override void LoadFromRawData(Stream stream)
        {
            SkiaImage = SKImage.FromEncodedData(stream);
        }

        public override void LoadFromRawData(byte[] rawData)
        {
            SkiaImage = SKImage.FromEncodedData(rawData);
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public override Image Duplicate()
        {
            return SkiaImage != null
                ? new Image { SkiaImage = SKImage.FromPixelCopy(SkiaImage.PeekPixels()) }
                : new Image();
        }
    }
}
