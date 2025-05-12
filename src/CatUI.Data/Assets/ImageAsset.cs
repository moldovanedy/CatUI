using System;
using System.IO;
using System.Threading.Tasks;
using CatUI.Data.Exceptions;
using SkiaSharp;

namespace CatUI.Data.Assets
{
    public class ImageAsset : Asset
    {
        /// <summary>
        /// Returns the SkiaSharp's internal image object.
        /// </summary>
        public SKImage? SkiaImage { get; private set; }

        public float Width => SkiaImage?.Width ?? 0;
        public float Height => SkiaImage?.Height ?? 0;

        /// <summary>
        /// Creates an empty image, without any kind of data.
        /// </summary>
        public ImageAsset()
        {
            IsLoaded = true;
        }

        public ImageAsset(SKImage image)
        {
            SkiaImage = image;
            IsLoaded = true;
        }

        public ImageAsset(Stream stream, bool loadAsync = false)
        {
            if (loadAsync)
            {
                _ = LoadFromStreamAsync(stream);
            }
            else
            {
                LoadFromStream(stream);
            }
        }

        public ImageAsset(byte[] rawData)
        {
            LoadFromRawData(rawData);
        }

        protected internal sealed override void LoadFromStream(Stream stream)
        {
            SkiaImage =
                SKImage.FromEncodedData(stream) ??
                throw new AssetLoadException("An image couldn't be loaded from the input stream.");
            IsLoaded = true;
        }

        protected internal sealed override void LoadFromRawData(byte[] rawData)
        {
            SkiaImage =
                SKImage.FromEncodedData(rawData) ??
                throw new AssetLoadException("An image couldn't be loaded from the binary data.");
            IsLoaded = true;
        }

        protected internal sealed override async Task LoadFromStreamAsync(Stream stream)
        {
            byte[] rawData = new byte[stream.Length];
            int currentReadBytes, totalReadBytes = 0;
            while ((currentReadBytes =
                       await stream.ReadAsync(
                           rawData.AsMemory(totalReadBytes, rawData.Length))) > 0)
            {
                totalReadBytes += currentReadBytes;
            }

            LoadFromRawData(rawData);
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public override ImageAsset Duplicate()
        {
            return SkiaImage != null
                ? new ImageAsset(SKImage.FromPixelCopy(SkiaImage.PeekPixels()))
                : new ImageAsset();
        }
    }
}
