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
    }
}
