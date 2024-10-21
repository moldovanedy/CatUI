using System.IO;
using SkiaSharp;

namespace CatUI.Data.Assets
{
    public class Image : Asset
    {
        public SKImage? SkiaImage { get; private set; }

        public override void LoadFromRawData(Stream stream)
        {
            this.SkiaImage = SKImage.FromEncodedData(stream);
        }

        public override void LoadFromRawData(byte[] rawData)
        {
            this.SkiaImage = SKImage.FromEncodedData(rawData);
        }
    }
}
