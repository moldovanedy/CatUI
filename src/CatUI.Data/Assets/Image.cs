using System.IO;
using SkiaSharp;

namespace CatUI.Data.Assets
{
    public class Image : Asset
    {
        public SKImage? SkiaImage { get; private set; }

        public override void LoadFromRawData(Stream stream)
        {
            SkiaImage = SKImage.FromEncodedData(stream);
        }

        public override void LoadFromRawData(byte[] rawData)
        {
            SkiaImage = SKImage.FromEncodedData(rawData);
        }

        public override Image Duplicate()
        {
            return SkiaImage != null
                ? new Image() { SkiaImage = SKImage.FromPixelCopy(SkiaImage.PeekPixels()) }
                : new Image();
        }
    }
}
