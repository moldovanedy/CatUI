using System.IO;
using System.Threading.Tasks;
using CatUI.Data.Exceptions;
using CatUI.Data.Theming.Typography;
using SkiaSharp;

namespace CatUI.Data.Assets
{
    public class FontAsset : Asset
    {
        /// <summary>
        /// Returns the SkiaSharp's internal font object.
        /// </summary>
        public SKTypeface? SkiaFont { get; private set; }

        /// <summary>
        /// Returns the weight of this font. It is generally one of the values from <see cref="FontWeightPreset"/>.
        /// Returns <see cref="FontWeightPreset.Invalid"/> if the font weight is unknown, or the font is not loaded yet.
        /// </summary>
        public FontWeightPreset FontWeight =>
            SkiaFont != null ? (FontWeightPreset)SkiaFont.FontWeight : FontWeightPreset.Invalid;

        /// <summary>
        /// Returns the width of this font. It is generally one of the values from <see cref="FontWidthPreset"/>.
        /// Returns <see cref="FontWidthPreset.Invalid"/> if the font width is unknown, or the font is not loaded yet.
        /// </summary>
        public FontWidthPreset FontWidth =>
            SkiaFont != null ? (FontWidthPreset)SkiaFont.FontWidth : FontWidthPreset.Invalid;

        /// <summary>
        /// Returns the slant type of this font. It is generally one of the values from <see cref="FontSlantPreset"/>.
        /// Returns <see cref="FontSlantPreset.Invalid"/> if the font slant type is unknown, or the font is not
        /// loaded yet.
        /// </summary>
        public FontSlantPreset FontSlant =>
            SkiaFont != null ? (FontSlantPreset)SkiaFont.FontSlant : FontSlantPreset.Invalid;

        /// <summary>
        /// Returns the font family name, usually localized (i.e. translated to the user's language). Returns an empty
        /// string if the family name is unknown, or the font is not loaded yet.
        /// </summary>
        public string FontFamily => SkiaFont?.FamilyName ?? "";

        /// <summary>
        /// Creates a new, empty font asset, without any data.
        /// </summary>
        public FontAsset()
        {
            IsLoaded = true;
        }

        public FontAsset(SKTypeface font)
        {
            SkiaFont = font;
            IsLoaded = true;
        }

        public FontAsset(Stream stream, bool loadAsync = false)
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

        public FontAsset(byte[] rawData)
        {
            LoadFromRawData(rawData);
        }

        protected internal sealed override void LoadFromStream(Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            //TODO: this creates a copy of the byte buffer, the only other solution for now is to use IntPtr
            SkiaFont =
                SKTypeface.FromData(SKData.CreateCopy(ms.ToArray())) ??
                throw new AssetLoadException("A font asset couldn't be loaded from the input stream.");
            IsLoaded = true;
        }

        protected internal sealed override async Task LoadFromStreamAsync(Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            //TODO: this creates a copy of the byte buffer, the only other solution for now is to use IntPtr
            SkiaFont =
                SKTypeface.FromData(SKData.CreateCopy(ms.ToArray())) ??
                throw new AssetLoadException("A font asset couldn't be loaded from the input stream.");
            IsLoaded = true;
        }

        protected internal sealed override void LoadFromRawData(byte[] rawData)
        {
            //TODO: this creates a copy of the byte buffer, the only other solution for now is to use IntPtr
            SkiaFont =
                SKTypeface.FromData(SKData.CreateCopy(rawData)) ??
                throw new AssetLoadException("A font asset couldn't be loaded from the binary data.");
            IsLoaded = true;
        }

        public override CatObject Duplicate()
        {
            if (SkiaFont == null)
            {
                return new FontAsset();
            }

            SKFontStyle style = SkiaFont.FontStyle;
            return new FontAsset(SKFontManager.Default.MatchTypeface(SkiaFont, style));
        }
    }
}
